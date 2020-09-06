
using Deemix.AutoLoader.Data;
using Deemix.AutoLoader.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Deemix.Autoloader.Tests
{
    public class DataRepositoryFixture
    {
        private readonly AppDbContext _appDbContext;
        private readonly IDataRepository _dataRepository;

        public DataRepositoryFixture()
        {
            _appDbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("AppDbContext").Options);
            _dataRepository = new DataRepository(_appDbContext);
        }

        public IDataRepository GetCleanRepo()
        {
            _appDbContext.Playlists.RemoveRange(_appDbContext.Playlists);
            _appDbContext.Artists.RemoveRange(_appDbContext.Artists);
            _appDbContext.Folders.RemoveRange(_appDbContext.Folders);
            _appDbContext.Genres.RemoveRange(_appDbContext.Genres);

            _appDbContext.SaveChanges();

            return _dataRepository;
        }

        public void Dispose()
        {
            _appDbContext.Dispose();
        }


    }
}