using Deemixrr.Data;
using Deemixrr.Helpers;
using Deemixrr.Models.Dto;

using System.IO;
using System.Threading.Tasks;

namespace Deemixrr.Services
{
    public class DeemixService : IDeemixService
    {
        private readonly string BaseCommand = "{0} -p {1}";
        private string _arlPath = "/config/.config/deemix/.arl";

        public void DownloadArtist(Artist artist)
        {
            string.Format(BaseCommand, $"https://www.deezer.com/en/artist/{artist.DeezerId}", artist.Folder.Path).Deemix();
        }

        public void DownloadPlaylist(Playlist playlist)
        {
            string.Format(BaseCommand, $"https://www.deezer.com/en/playlist/{playlist.DeezerId}", playlist.Folder.Path).Deemix();
        }

        public void Download(string url, Folder folder)
        {
            string.Format(BaseCommand, url, folder.Path).Deemix();
        }



        public async Task<ArlInfo> GetArl()
        {
            if (File.Exists(_arlPath))
                return new ArlInfo
                {
                    Content = await File.ReadAllTextAsync(_arlPath),
                    LastWrite = File.GetLastWriteTimeUtc(_arlPath)
                };

            return null;
        }

        public async Task SetArl(string arl)
        {
            await File.WriteAllTextAsync(_arlPath, arl);
        }
    }
}