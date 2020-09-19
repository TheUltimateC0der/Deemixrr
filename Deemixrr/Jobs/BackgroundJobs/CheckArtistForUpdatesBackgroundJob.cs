using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Deemixrr.Configuration;
using Deemixrr.Data;
using Deemixrr.Repositories;
using Deemixrr.Services;

using E.Deezer.Api;

using Hangfire;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public class CheckArtistForUpdatesBackgroundJob : IBackgroundJob<ulong>
    {
        private readonly DelayConfiguration _delayConfiguration;
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IDeemixService _deemixService;
        private readonly IMapper _mapper;

        public CheckArtistForUpdatesBackgroundJob(DelayConfiguration delayConfiguration, IDeezerApiService deezerApiService, IDataRepository dataRepository, IDeemixService deemixService, IMapper mapper)
        {
            _delayConfiguration = delayConfiguration ?? throw new ArgumentNullException(nameof(delayConfiguration));
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _deemixService = deemixService ?? throw new ArgumentNullException(nameof(deemixService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [MaximumConcurrentExecutions(1, 18000)]
        public async Task Execute(ulong param, bool queueNext = false)
        {
            var dbArtist = await _dataRepository.GetArtist(param);
            if (dbArtist == null)
            {
                var apiArtist = await _deezerApiService.GetDeezerApi().Artists.GetById(param, CancellationToken.None);

                await _dataRepository.CreateArtist(_mapper.Map<Artist>(apiArtist));
                dbArtist = await _dataRepository.GetArtist(apiArtist.Id);

                await Task.Delay(_delayConfiguration.CheckArtistForUpdatesBackgroundJob_ExecuteDelay);
            }

            if (dbArtist != null)
            {
                var apiArtist = await _deezerApiService.GetDeezerApi().Artists.GetById(param, CancellationToken.None);
                var numberOfTracks = await GetTrackCount(apiArtist);

                if (dbArtist.NumberOfAlbums != apiArtist.NumberOfAlbums ||
                    dbArtist.NumberOfTracks != numberOfTracks)
                {
                    _deemixService.DownloadArtist(dbArtist);

                    dbArtist.NumberOfTracks = numberOfTracks;
                    dbArtist.NumberOfAlbums = apiArtist.NumberOfAlbums;

                    await _dataRepository.UpdateArtist(dbArtist);
                }
            }
        }


        private async Task<uint> GetTrackCount(IArtist artist)
        {
            uint offset = 0;
            uint fetchCount = 50;
            uint trackCount = 0;

            while (true)
            {
                foreach (var album in await artist.Albums(CancellationToken.None, offset, fetchCount))
                {
                    await Task.Delay(_delayConfiguration.CheckArtistForUpdatesBackgroundJob_GetTrackCountDelay);

                    var apiAlbum = await _deezerApiService.GetDeezerApi().Albums.GetById(album.Id, CancellationToken.None);

                    trackCount += apiAlbum.TrackCount;
                }

                if (offset >= artist.NumberOfAlbums)
                {
                    break;
                }

                offset += fetchCount;
            }

            return trackCount;
        }
    }
}