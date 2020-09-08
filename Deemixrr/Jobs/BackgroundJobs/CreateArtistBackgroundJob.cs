using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Deemixrr.Data;
using Deemixrr.Jobs.Models;
using Deemixrr.Repositories;
using Deemixrr.Services;

using E.Deezer.Api;

using Hangfire;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public class CreateArtistBackgroundJob : IBackgroundJob<CreateArtistBackgroundJobData>
    {
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _mapper;

        public CreateArtistBackgroundJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IMapper mapper)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute(CreateArtistBackgroundJobData param, bool queueNext = false)
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

            while (true)
            {
                foreach (var track in await playlist.Tracks(CancellationToken.None, offset))
                {
                    await CheckAndCreateArtist(track.Artist, folder);
                }

                if (offset >= playlist.NumberOfTracks)
                {
                    break;
                }
                else
                {
                    offset += 50;
                }

                await Task.Delay(500);
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

                        await Task.Delay(500);
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

                BackgroundJob.Enqueue<CheckArtistForUpdatesBackgroundJob>(x => x.Execute(deezerArtist.Id, false));
            }
        }
    }
}