using Deemixrr.Data;

namespace Deemixrr.Services
{
    public interface IDeemixService
    {

        void DownloadArtist(Artist artist);
        void DownloadPlaylist(Playlist playlist);

    }
}