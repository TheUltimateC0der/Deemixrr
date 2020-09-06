using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Deemix.AutoLoader.Data;
using Deemix.AutoLoader.Repositories;
using Deemix.AutoLoader.Services;

using Hangfire;

namespace Deemix.AutoLoader.Jobs.BackgroundJobs
{
    public class CheckPlaylistForUpdatesBackgroundJob : IBackgroundJob<ulong>
    {
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IDeemixService _deemixService;
        private readonly IMapper _mapper;

        public CheckPlaylistForUpdatesBackgroundJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IDeemixService deemixService, IMapper mapper)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _deemixService = deemixService ?? throw new ArgumentNullException(nameof(deemixService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [DisableConcurrentExecution(Int32.MaxValue)]
        public async Task Execute(ulong param, bool queueNext = false)
        {
            var dbPlaylist = await _dataRepository.GetPlaylist(param);
            if (dbPlaylist == null)
            {
                var apiPlaylist = _deezerApiService.GetDeezerApi().Playlists.GetById(param, CancellationToken.None);

                dbPlaylist = await _dataRepository.CreatePlaylist(_mapper.Map<Playlist>(apiPlaylist));
            }

            if (dbPlaylist != null)
            {
                var apiPlaylist = await _deezerApiService.GetDeezerApi().Playlists.GetById(param, CancellationToken.None);

                if (dbPlaylist.NumberOfTracks != apiPlaylist.NumberOfTracks)
                {
                    //_deemixService.DownloadPlaylist(apiPlaylist.Link);

                    dbPlaylist.NumberOfTracks = apiPlaylist.NumberOfTracks;

                    await _dataRepository.UpdatePlaylist(dbPlaylist);
                }
            }
        }
    }
}