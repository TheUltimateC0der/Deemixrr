using E.Deezer;

using System.Net.Http;

namespace Deemixrr.Services
{
    public class DeezerApiService : IDeezerApiService
    {
        private DeezerSession _deezerSession;

        public DeezerApiService()
        {
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