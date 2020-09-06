
using Deemix.AutoLoader.Helpers;

namespace Deemix.AutoLoader.Services
{
    public class DeemixService : IDeemixService
    {
        private readonly string BaseCommand = "deemix {0}";

        public void DownloadArtist(string artistUrl)
        {
            string.Format(BaseCommand, artistUrl).Bash();
        }

        public void DownloadPlaylist(string playlistUrl)
        {
            string.Format(BaseCommand, playlistUrl).Bash();
        }
    }
}