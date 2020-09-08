using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deemixrr.Helpers;
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

        public GetUpdatesRecurringJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IConfigurationService configurationService)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        [MaximumConcurrentExecutions(1)]
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
                                var apiAlbum = await _deezerApiService.GetDeezerApi().Albums.GetById(album.Id, CancellationToken.None);

                                album.Link.Deemix();

                                dbArtist.NumberOfAlbums += 1;
                                dbArtist.NumberOfTracks += apiAlbum.TrackCount;

                                await _dataRepository.UpdateArtist(dbArtist);
                            }
                        }
                    }

                    await _configurationService.Set("LastChangeId", latestAlbum.Id);
                }
            }
        }
    }
}