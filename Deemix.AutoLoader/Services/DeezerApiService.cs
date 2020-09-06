
using System.Net.Http;

using Deemix.AutoLoader.Configuration;

using E.Deezer;

namespace Deemix.AutoLoader.Services
{
    public class DeezerApiService : IDeezerApiService
    {
        private DeezerSession _deezerSession;
        private readonly DeezerApiConfiguration _deezerApiConfiguration;

        public DeezerApiService(DeezerApiConfiguration deezerApiConfiguration)
        {
            _deezerApiConfiguration = deezerApiConfiguration;
        }

        public DeezerSession GetDeezerApi()
        {
            if (_deezerSession == null)
            {
                _deezerSession = new DeezerSession(new HttpClientHandler());
            }

            return _deezerSession;
        }
    }
}