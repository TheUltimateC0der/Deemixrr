using System.Collections.Generic;
using System.Threading.Tasks;

using Deemix.AutoLoader.Data;

namespace Deemix.AutoLoader.Repositories
{
    public interface IDataRepository
    {

        Task<Artist> CreateArtist(Artist model);


        Task<Artist> GetArtist(string id);

        Task<Artist> GetArtist(ulong id);

        Task<IList<Artist>> GetArtists(int skip, int take);
        Task<IList<Artist>> GetLastUpdatedArtists(int skip, int take);

        Task<IList<Artist>> GetArtists();

        Task<int> GetArtistCount();

        Task<IList<Artist>> GetArtists(string searchTerm);


        Task<Artist> UpdateArtist(Artist model);


        Task DeleteArtist(Artist model);



        Task<Playlist> CreatePlaylist(Playlist model);


        Task<Playlist> GetPlaylist(string id);
        Task<Playlist> GetPlaylist(ulong id);

        Task<IList<Playlist>> GetPlaylists(int skip, int take);

        Task<IList<Playlist>> GetLastUpdatedPlaylists(int skip, int take);

        Task<IList<Playlist>> GetPlaylists();

        Task<int> GetPlaylistCount();

        Task<IList<Playlist>> GetPlaylists(string searchTerm);


        Task<Playlist> UpdatePlaylist(Playlist model);


        Task DeletePlaylist(Playlist model);



        Task<Folder> CreateFolder(Folder model);


        Task<Folder> GetFolder(string id);

        Task<int> GetFolderArtistCount(Folder model);

        Task<IList<Folder>> GetFolders(int skip, int take);

        Task<IList<Folder>> GetLastUpdatedFolders(int skip, int take);

        Task<IList<Folder>> GetFolders();

        Task<IList<Folder>> GetFolders(string searchTerm);

        Task<int> GetFolderCount();

        Task<long> GetFolderSizeCumulated();

        Task<Folder> UpdateFolder(Folder model);

        Task DeleteFolder(Folder model);




        Task<Genre> CreateGenre(Genre model);


        Task<Genre> GetGenre(string id);

        Task<Genre> GetGenre(ulong id);

    }
}