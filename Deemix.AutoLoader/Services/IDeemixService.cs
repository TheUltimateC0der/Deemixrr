using Deemix.AutoLoader.Data;

namespace Deemix.AutoLoader.Services
{
    public interface IDeemixService
    {

        void DownloadArtist(Artist artist);
        void DownloadPlaylist(Playlist playlist);

    }
}