namespace Deemix.AutoLoader.Services
{
    public interface IDeemixService
    {

        void DownloadArtist(string artistUrl);
        void DownloadPlaylist(string playlistUrl);

    }
}