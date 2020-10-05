using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Deemixrr.Configuration;
using Deemixrr.Data;
using Deemixrr.Jobs.Models;
using Deemixrr.Repositories;
using Deemixrr.Services;

using E.Deezer.Api;

using Hangfire;
using Hangfire.Server;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public class CreateArtistBackgroundJob : IBackgroundJob<CreateArtistBackgroundJobData>
    {
        private readonly DelayConfiguration _delayConfiguration;
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _mapper;

        public CreateArtistBackgroundJob(DelayConfiguration delayConfiguration, IDeezerApiService deezerApiService, IDataRepository dataRepository, IMapper mapper)
        {
            _delayConfiguration = delayConfiguration ?? throw new ArgumentNullException(nameof(delayConfiguration));
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute(CreateArtistBackgroundJobData param, PerformContext context)
        {
            var folder = await _dataRepository.GetFolder(param.FolderId);
            if (folder == null) return;

            if (param.ArtistDeezerId != 0)
            {
                await FromArtist(param.ArtistDeezerId, folder);
                return;
            }

            if (param.PlaylistDeezerId != 0)
            {
                await FromPlaylist(param.PlaylistDeezerId, folder);
                return;
            }

            if (param.UserProfileId != 0)
            {
                await FromUser(param.UserProfileId, folder);
                return;
            }

            if (string.IsNullOrEmpty(param.ArtistDeezerIds) == false)
            {
                await FromCsv(param.ArtistDeezerIds, folder);
                return;
            }
        }


        private async Task FromArtist(ulong artistId, Folder folder)
        {
            await CheckAndCreateArtist(await _deezerApiService.GetDeezerApi().Artists.GetById(artistId, CancellationToken.None), folder);
        }

        private async Task FromPlaylist(ulong playlistId, Folder folder)
        {
            var playlist = await _deezerApiService.GetDeezerApi().Playlists.GetById(playlistId, CancellationToken.None);

            uint offset = 0;
            uint fetchCount = 50;

            while (true)
            {
                foreach (var track in await playlist.Tracks(CancellationToken.None, offset, fetchCount))
                {
                    await CheckAndCreateArtist(track.Artist, folder);
                }

                if (offset >= playlist.NumberOfTracks)
                {
                    break;
                }

                offset += fetchCount;
                await Task.Delay(_delayConfiguration.CreateArtistBackgroundJob_FromPlaylistDelay);
            }
        }

        private async Task FromUser(ulong userId, Folder folder)
        {
            uint offset = 0;
            uint fetchCount = 250;

            while (true)
            {
                var artists = await _deezerApiService.GetDeezerApi().User.GetFavouriteArtists(userId, CancellationToken.None, offset, fetchCount);
                if (!artists.Any()) break;

                foreach (var artist in artists)
                {
                    await CheckAndCreateArtist(artist, folder);
                }

                offset += fetchCount;
                await Task.Delay(_delayConfiguration.CreateArtistBackgroundJob_FromUserDelay);
            }
        }

        private async Task FromCsv(string csv, Folder folder)
        {
            if (csv.Contains(","))
            {
                var artistIds = csv.Split(",");

                foreach (var artistIdPart in artistIds)
                {
                    var parsed = ulong.TryParse(artistIdPart, out var artistId);
                    if (parsed)
                    {
                        await CheckAndCreateArtist(await _deezerApiService.GetDeezerApi().Artists.GetById(artistId, CancellationToken.None), folder);

                        await Task.Delay(_delayConfiguration.CreateArtistBackgroundJob_FromCsvDelay);
                    }
                }
            }
        }


        private async Task CheckAndCreateArtist(IArtist deezerArtist, Folder folder)
        {
            if (deezerArtist == null) return;

            var dbArtist = await _dataRepository.GetArtist(deezerArtist.Id);
            if (dbArtist == null)
            {
                var newArtist = _mapper.Map<Artist>(deezerArtist);
                newArtist.FolderId = folder.Id;

                await _dataRepository.CreateArtist(newArtist);

                BackgroundJob.Enqueue<CheckArtistForUpdatesBackgroundJob>(x => x.Execute(deezerArtist.Id, null));
            }
        }
    }
}