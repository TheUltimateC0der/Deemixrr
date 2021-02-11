using Deemixrr.Data;
using Deemixrr.Models.Dto;

using System.Threading.Tasks;

namespace Deemixrr.Services
{
    public interface IDeemixService
    {

        void DownloadArtist(Artist artist);
        void DownloadPlaylist(Playlist playlist);
        void Download(string url, Folder folder);

        Task<ArlInfo> GetArl();
        Task SetArl(string arl);

    }
}