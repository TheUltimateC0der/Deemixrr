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
    public class CreatePlaylistBackgroundJob : IBackgroundJob<CreatePlaylistBackgroundJobData>
    {
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _mapper;

        public CreatePlaylistBackgroundJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IMapper mapper)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute(CreatePlaylistBackgroundJobData param, bool queueNext = false)
        {
            var folder = await _dataRepository.GetFolder(param.FolderId);
            if (folder == null) return;

            if (param.DeezerId != 0)
            {
                await SinglePlaylist(param.DeezerId, folder);
                return;
            }

            if (string.IsNullOrEmpty(param.DeezerIds) == false)
            {
                await MultiplePlaylist(param.DeezerIds, folder);
                return;
            }
        }


        private async Task SinglePlaylist(ulong playlistId, Folder folder)
        {
            await CheckAndCreatePlaylist(await _deezerApiService.GetDeezerApi().Playlists.GetById(playlistId, CancellationToken.None), folder);
        }

        private async Task MultiplePlaylist(string csv, Folder folder)
        {
            if (csv.Contains(","))
            {
                var playlistIds = csv.Split(",");

                foreach (var playlistIdPart in playlistIds)
                {
                    var parsed = ulong.TryParse(playlistIdPart, out var artistId);
                    if (parsed)
                    {
                        await CheckAndCreatePlaylist(await _deezerApiService.GetDeezerApi().Playlists.GetById(artistId, CancellationToken.None), folder);

                        await Task.Delay(500);
                    }
                }
            }
        }


        private async Task CheckAndCreatePlaylist(IPlaylist deezerPlaylist, Folder folder)
        {
            if (deezerPlaylist == null) return;

            var dbPlaylist = await _dataRepository.GetPlaylist(deezerPlaylist.Id);
            if (dbPlaylist == null)
            {
                var newPlaylist = _mapper.Map<Playlist>(deezerPlaylist);
                newPlaylist.FolderId = folder.Id;

                await _dataRepository.CreatePlaylist(newPlaylist);

                BackgroundJob.Enqueue<CheckPlaylistForUpdatesBackgroundJob>(x => x.Execute(deezerPlaylist.Id, false));
            }
        }
    }
}