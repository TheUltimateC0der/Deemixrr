using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deemix.AutoLoader.Configuration;
using Deemix.AutoLoader.Services;

using E.Deezer;
using E.Deezer.Api;

using Microsoft.Extensions.Logging;

namespace Deemix.AutoLoader.Jobs.RecurringJobs
{
    public class FavoriteArtistsRecurringJob : IRecurringJob
    {
        private readonly DeezerSession _deezerSession;
        private readonly IDeemixService _deemixService;
        private readonly DeezerApiConfiguration _deezerApiConfiguration;
        private readonly ILogger<FavoriteArtistsRecurringJob> _logger;

        public FavoriteArtistsRecurringJob(IDeezerApiService deezerApiService, IDeemixService deemixService, DeezerApiConfiguration deezerApiConfiguration, ILogger<FavoriteArtistsRecurringJob> logger)
        {
            _deemixService = deemixService;
            _deezerApiConfiguration = deezerApiConfiguration;
            _logger = logger;
            _deezerSession = deezerApiService.GetDeezerApi();
        }

        public async Task Execute()
        {
            var artists = new List<IArtist>();

            uint index = 0;
            uint count = 100;

            while (true)
            {
                var artistResponse = (await _deezerSession.User.GetFavouriteArtists(_deezerApiConfiguration.UserId, CancellationToken.None, index, count)).ToList();

                if (!artistResponse.Any()) break;

                artists.AddRange(artistResponse);
                index += count;

                _logger.LogInformation($"Got {artistResponse.Count} items ...");

                await Task.Delay(1000);
            }


            foreach (var artist in artists)
            {
                _logger.LogInformation($"Downloading '{artist.Name}' from {artist.Link} ...");

                _deemixService.DownloadArtist(artist.Link);
            }

        }
    }
}