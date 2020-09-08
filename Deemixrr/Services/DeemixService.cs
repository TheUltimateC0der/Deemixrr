using Deemixrr.Data;
using Deemixrr.Helpers;

namespace Deemixrr.Services
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