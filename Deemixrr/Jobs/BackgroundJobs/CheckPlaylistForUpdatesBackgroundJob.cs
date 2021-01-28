using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Deemixrr.Configuration;
using Deemixrr.Data;
using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;
using Hangfire.Server;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public class CheckPlaylistForUpdatesBackgroundJob : IBackgroundJob<ulong>
    {
        private readonly DelayConfiguration _delayConfiguration;
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IDeemixService _deemixService;
        private readonly IMapper _mapper;

        public CheckPlaylistForUpdatesBackgroundJob(DelayConfiguration delayConfiguration, IDeezerApiService deezerApiService, IDataRepository dataRepository, IDeemixService deemixService, IMapper mapper)
        {
            _delayConfiguration = delayConfiguration ?? throw new ArgumentNullException(nameof(delayConfiguration));
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _deemixService = deemixService ?? throw new ArgumentNullException(nameof(deemixService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute(ulong param, PerformContext context)
        {
            var dbPlaylist = await _dataRepository.GetPlaylist(param);
            if (dbPlaylist == null)
            {
                var apiPlaylist = await _deezerApiService.GetDeezerApi().Playlists.GetById(param, CancellationToken.None);

                await _dataRepository.CreatePlaylist(_mapper.Map<Playlist>(apiPlaylist));
                dbPlaylist = await _dataRepository.GetPlaylist(apiPlaylist.Id);

                await Task.Delay(_delayConfiguration.CheckPlaylistForUpdatesBackgroundJob_ExecuteDelay);
            }

            if (dbPlaylist != null)
            {
                var apiPlaylist = await _deezerApiService.GetDeezerApi().Playlists.GetById(param, CancellationToken.None);

                if (dbPlaylist.NumberOfTracks != apiPlaylist.NumberOfTracks)
                {
                    _deemixService.DownloadPlaylist(dbPlaylist);

                    dbPlaylist.NumberOfTracks = apiPlaylist.NumberOfTracks;

                    await _dataRepository.UpdatePlaylist(dbPlaylist);
                }
            }
        }
    }
}