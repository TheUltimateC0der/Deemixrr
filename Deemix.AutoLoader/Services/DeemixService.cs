using Deemix.AutoLoader.Data;
using Deemix.AutoLoader.Helpers;

namespace Deemix.AutoLoader.Services
{
    public class DeemixService : IDeemixService
    {
        private readonly string BaseCommand = "{0} -p {1}";

        public void DownloadArtist(Artist artist)
        {
            string.Format(BaseCommand, $"https://www.deezer.com/en/artist/{artist.DeezerId}", artist.Folder.Path).Deemix();
        }

        public void DownloadPlaylist(Playlist playlist)
        {
            string.Format(BaseCommand, $"https://www.deezer.com/en/playlist/{playlist.DeezerId}", playlist.Folder.Path).Deemix();
        }
    }
}