using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deemixrr.Helpers;
using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;

using Microsoft.Extensions.Configuration;

namespace Deemixrr.Jobs.RecurringJobs
{
    public class GetUpdatesRecurringJob : IRecurringJob
    {
        private readonly IDeezerApiService _deezerApiService;
        private readonly IDataRepository _dataRepository;
        private readonly IConfiguration _configuration;

        public GetUpdatesRecurringJob(IDeezerApiService deezerApiService, IDataRepository dataRepository, IConfiguration configuration)
        {
            _deezerApiService = deezerApiService ?? throw new ArgumentNullException(nameof(deezerApiService));
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [MaximumConcurrentExecutions(1)]
        public async Task Execute()
        {
            var updates = await _deezerApiService.GetDeezerApi().Genre.GetNewReleasesForGenre(0, CancellationToken.None, 0, 200);
            if (updates != null)
            {
                var updatesList = updates.ToList();
                var latestAlbum = updatesList.FirstOrDefault();

                if (latestAlbum != null)
                {
                    if (latestAlbum.Id.ToString() != _configuration["LastChangeId"])
                    {
                        foreach (var album in updatesList)
                        {
                            if (latestAlbum.Id.ToString() == _configuration["LastChangeId"])
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

                    _configuration["LastChangeId"] = latestAlbum.Id.ToString();
                }
            }
        }
    }
}