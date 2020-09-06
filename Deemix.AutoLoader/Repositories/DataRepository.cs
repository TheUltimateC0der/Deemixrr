using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Deemix.AutoLoader.Data;

using Microsoft.EntityFrameworkCore;

namespace Deemix.AutoLoader.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly AppDbContext _appDbContext;

        public DataRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        #region Artist

        public async Task<Artist> CreateArtist(Artist model)
        {
            await _appDbContext.Artists.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task DeleteArtist(Artist model)
        {
            _appDbContext.Artists.Remove(model);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Artist> GetArtist(string id)
        {
            return await _appDbContext.Artists
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Artist> GetArtist(ulong id)
        {
            return await _appDbContext.Artists
                .FirstOrDefaultAsync(x => x.DeezerId == id);
        }

        public async Task<IList<Artist>> GetArtists(int skip, int take)
        {
            return await _appDbContext.Artists
                .Include(x => x.Folder)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<Artist>> GetLastUpdatedArtists(int skip, int take)
        {
            return await _appDbContext.Artists
                .Include(x => x.Folder)
                .OrderBy(x => x.Updated)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<Artist>> GetArtists()
        {
            return await _appDbContext.Artists
                .Include(x => x.Folder)
                .ToListAsync();
        }

        public async Task<int> GetArtistCount()
        {
            return await _appDbContext.Artists.CountAsync();
        }

        public async Task<IList<Artist>> GetArtists(string searchTerm)
        {
            return await _appDbContext.Artists
                .Include(x => x.Folder)
                .Where(x => x.Name.Contains(searchTerm) || x.DeezerId.ToString() == searchTerm)
                .ToListAsync();
        }

        public async Task<Artist> UpdateArtist(Artist model)
        {
            _appDbContext.Artists.Update(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        #endregion

        #region Folder

        public async Task<Folder> CreateFolder(Folder model)
        {
            await _appDbContext.Folders.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task DeleteFolder(Folder model)
        {
            _appDbContext.Folders.Remove(model);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Folder> GetFolder(string id)
        {
            return await _appDbContext.Folders
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<Folder>> GetFolders(int skip, int take)
        {
            return await _appDbContext.Folders
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<Folder>> GetLastUpdatedFolders(int skip, int take)
        {
            return await _appDbContext.Folders
                .OrderBy(x => x.Updated)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<Folder>> GetFolders()
        {
            return await _appDbContext.Folders
                .ToListAsync();
        }

        public async Task<IList<Folder>> GetFolders(string searchTerm)
        {
            return await _appDbContext.Folders
                .Where(x => x.Name.Contains(searchTerm) || x.Path.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<int> GetFolderCount()
        {
            return await _appDbContext.Folders.CountAsync();
        }

        public async Task<Folder> UpdateFolder(Folder model)
        {
            _appDbContext.Folders.Update(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<int> GetFolderArtistCount(Folder model)
        {
            return await _appDbContext.Artists.Where(x => x.FolderId == model.Id).CountAsync();
        }

        #endregion

        #region Playlist

        public async Task<Playlist> CreatePlaylist(Playlist model)
        {
            await _appDbContext.Playlists.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task DeletePlaylist(Playlist model)
        {
            _appDbContext.Playlists.Remove(model);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Playlist> GetPlaylist(string id)
        {
            return await _appDbContext.Playlists
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Playlist> GetPlaylist(ulong id)
        {
            return await _appDbContext.Playlists
                .FirstOrDefaultAsync(x => x.DeezerId == id);
        }

        public async Task<IList<Playlist>> GetPlaylists(int skip, int take)
        {
            return await _appDbContext.Playlists
                .Include(x => x.Folder)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<Playlist>> GetLastUpdatedPlaylists(int skip, int take)
        {
            return await _appDbContext.Playlists
                .Include(x => x.Folder)
                .OrderBy(x => x.Updated)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<Playlist>> GetPlaylists()
        {
            return await _appDbContext.Playlists
                .Include(x => x.Folder)
                .ToListAsync();
        }

        public async Task<int> GetPlaylistCount()
        {
            return await _appDbContext.Playlists.CountAsync();
        }

        public async Task<IList<Playlist>> GetPlaylists(string searchTerm)
        {
            return await _appDbContext.Playlists
                .Include(x => x.Folder)
                .Where(x => x.Name.Contains(searchTerm) || x.DeezerId.ToString() == searchTerm)
                .ToListAsync();
        }

        public async Task<Playlist> UpdatePlaylist(Playlist model)
        {
            _appDbContext.Playlists.Update(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<long> GetFolderSizeCumulated()
        {
            return await _appDbContext.Folders.Select(x => x.Size).SumAsync();
        }

        #endregion

        #region Genre

        public async Task<Genre> CreateGenre(Genre model)
        {
            await _appDbContext.Genres.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<Genre> GetGenre(string id)
        {
            return await _appDbContext.Genres
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Genre> GetGenre(ulong id)
        {
            return await _appDbContext.Genres
                .FirstOrDefaultAsync(x => x.DeezerId == id);
        }

        #endregion

    }
}