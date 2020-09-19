using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;

namespace Deemixrr.Jobs.RecurringJobs
{
    public class GetUpdatesRecurringJob : IRecurringJob
    {
        private readonly IConfigurationService _configurationService;
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IDeemixService _deemixService;

        public GetUpdatesRecurringJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IConfigurationService configurationService, IDeemixService deemixService)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            _deemixService = deemixService ?? throw new ArgumentException(nameof(deemixService));
        }

        [MaximumConcurrentExecutions(1, 1800)]
        public async Task Execute()
        {
            var updates = await _deezerApiService.GetDeezerApi().Genre.GetNewReleasesForGenre(0, CancellationToken.None, 0, 200);
            if (updates != null)
            {
                var updatesList = updates.ToList();
                var latestAlbum = updatesList.FirstOrDefault();
                var lastChangeId = await _configurationService.Get<ulong>("LastChangeId");

                if (latestAlbum != null)
                {
                    if (latestAlbum.Id != lastChangeId)
                    {
                        foreach (var album in updatesList)
                        {
                            if (latestAlbum.Id == lastChangeId)
                                break;

                            var dbArtist = await _dataRepository.GetArtist(album.Artist.Id);
                            if (dbArtist != null)
                            {
                                _deemixService.Download($"https://www.deezer.com/en/album/{album.Id}", dbArtist.Folder);
                            }
                        }
                    }

                    await _configurationService.Set("LastChangeId", latestAlbum.Id);
                }
            }
        }
    }
}