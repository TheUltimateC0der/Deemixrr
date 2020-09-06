using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Deemix.AutoLoader.Data;
using Deemix.AutoLoader.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Deemix.Autoloader.Tests.Repositories
{
    public class DataRepositoryTests : IClassFixture<DataRepositoryFixture>
    {
        private readonly DataRepositoryFixture _dataRepositoryFixture;

        public DataRepositoryTests(DataRepositoryFixture dataRepositoryFixture)
        {
            _dataRepositoryFixture = dataRepositoryFixture;
        }


        [Fact]
        public void CreateDataRepository()
        {
            //Arrange
            //Act
            var repo = new DataRepository(new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("AppDbContext").Options));
            //Assert
            Assert.NotNull(repo);
        }

        [Fact]
        public void CreateDataRepositoryWithoutDbContextThrowsArgumentNullException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new DataRepository(null));
        }

        #region Artist

        [Fact]
        public async Task CreateArtist()
        {
            //Arrange
            var artist = new Artist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateArtist(artist);

            //Assert
            Assert.NotNull(artist.Id);
            Assert.Equal((ulong)1337, artist.DeezerId);
            Assert.Equal("Eminem", artist.Name);
        }

        [Fact]
        public async Task DeleteArtist()
        {
            //Arrange
            var artist = new Artist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateArtist(artist);

            var artistId = artist.Id;

            await repo.DeleteArtist(artist);

            artist = await repo.GetArtist(artistId);

            //Assert
            Assert.Null(artist);
        }

        [Fact]
        public async Task GetArtistByStringTest()
        {
            //Arrange
            var artist = new Artist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateArtist(artist);

            var dbArtist = await repo.GetArtist(artist.Id);

            //Assert
            Assert.NotNull(dbArtist.Id);
            Assert.Equal((ulong)1337, dbArtist.DeezerId);
            Assert.Equal("Eminem", dbArtist.Name);
        }

        [Fact]
        public async Task GetArtistByULongTest()
        {
            //Arrange
            var artist = new Artist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateArtist(artist);

            var dbArtist = await repo.GetArtist((ulong)1337);

            //Assert
            Assert.NotNull(dbArtist.Id);
            Assert.Equal((ulong)1337, dbArtist.DeezerId);
            Assert.Equal("Eminem", dbArtist.Name);
        }

        [Fact]
        public async Task GetArtistsWithSkipAndTake()
        {
            //Arrange
            IList<Artist> artists = new List<Artist>
            {
                new Artist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Artist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Artist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var artist in artists)
            {
                await repo.CreateArtist(artist);
            }

            var dbArtists = await repo.GetArtists(1, 2);

            //Assert
            Assert.Equal(2, dbArtists.Count);

            Assert.Equal((ulong)1338, dbArtists[0].DeezerId);
            Assert.Equal("I like trains", dbArtists[0].Name);
            Assert.NotNull(dbArtists[0].Id);

            Assert.Equal((ulong)1339, dbArtists[1].DeezerId);
            Assert.Equal("Banana!", dbArtists[1].Name);
            Assert.NotNull(dbArtists[1].Id);
        }

        [Fact]
        public async Task GetLastUpdatedArtists()
        {
            //Arrange
            IList<Artist> artists = new List<Artist>
            {
                new Artist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Artist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Artist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var artist in artists)
            {
                await repo.CreateArtist(artist);

                await Task.Delay(1500);
            }

            var dbArtists = await repo.GetLastUpdatedArtists(1, 2);

            //Assert
            Assert.Equal(2, dbArtists.Count);

            Assert.True(dbArtists[1].Updated > dbArtists[0].Updated);
        }

        [Fact]
        public async Task GetArtists()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Test folder",
                Path = "/mnt/mount/media",
                Size = 5000
            };
            IList<Artist> artists = new List<Artist>
            {
                new Artist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Artist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Artist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            foreach (var artist in artists)
            {
                artist.FolderId = folder.Id;

                await repo.CreateArtist(artist);
            }

            var dbArtists = await repo.GetArtists();

            //Assert
            Assert.Equal(3, dbArtists.Count);

            Assert.Equal((ulong)1337, dbArtists[0].DeezerId);
            Assert.Equal("Eminem", dbArtists[0].Name);
            Assert.NotNull(dbArtists[0].Id);

            Assert.Equal((ulong)1338, dbArtists[1].DeezerId);
            Assert.Equal("I like trains", dbArtists[1].Name);
            Assert.NotNull(dbArtists[1].Id);

            Assert.Equal((ulong)1339, dbArtists[2].DeezerId);
            Assert.Equal("Banana!", dbArtists[2].Name);
            Assert.NotNull(dbArtists[2].Id);
        }

        [Fact]
        public async Task GetArtistCount()
        {
            //Arrange
            IList<Artist> artists = new List<Artist>
            {
                new Artist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Artist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Artist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var artist in artists)
            {
                await repo.CreateArtist(artist);
            }

            var dbArtists = await repo.GetArtistCount();

            //Assert
            Assert.Equal(3, dbArtists);
        }

        [Fact]
        public async Task GetArtistsBySearchTerm()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Test folder",
                Path = "/mnt/mount/media",
                Size = 5000
            };
            IList<Artist> artists = new List<Artist>
            {
                new Artist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Artist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Artist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            foreach (var artist in artists)
            {
                artist.FolderId = folder.Id;

                await repo.CreateArtist(artist);
            }

            var dbArtists = await repo.GetArtists("in");

            //Assert
            Assert.Equal(2, dbArtists.Count);

            Assert.Equal((ulong)1337, dbArtists[0].DeezerId);
            Assert.Equal("Eminem", dbArtists[0].Name);
            Assert.NotNull(dbArtists[0].Id);

            Assert.Equal((ulong)1338, dbArtists[1].DeezerId);
            Assert.Equal("I like trains", dbArtists[1].Name);
            Assert.NotNull(dbArtists[1].Id);
        }

        [Fact]
        public async Task UpdateArtist()
        {
            //Arrange
            var artist = new Artist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateArtist(artist);

            artist.Name = "Eminem2";

            artist = await repo.UpdateArtist(artist);

            //Assert
            Assert.NotNull(artist.Id);
            Assert.Equal((ulong)1337, artist.DeezerId);
            Assert.Equal("Eminem2", artist.Name);
        }

        #endregion

        #region Playlist

        [Fact]
        public async Task CreatePlaylistTest()
        {
            //Arrange
            var playlist = new Playlist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreatePlaylist(playlist);

            //Assert
            Assert.NotNull(playlist.Id);
            Assert.Equal((ulong)1337, playlist.DeezerId);
            Assert.Equal("Eminem", playlist.Name);
        }

        [Fact]
        public async Task DeletePlaylist()
        {
            //Arrange
            var playlist = new Playlist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreatePlaylist(playlist);

            var playlistId = playlist.Id;

            await repo.DeletePlaylist(playlist);

            playlist = await repo.GetPlaylist(playlistId);

            //Assert
            Assert.Null(playlist);
        }

        [Fact]
        public async Task GetPlaylistByStringTest()
        {
            //Arrange
            var playlist = new Playlist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreatePlaylist(playlist);

            var dbPlaylist = await repo.GetPlaylist(playlist.Id);

            //Assert
            Assert.NotNull(dbPlaylist.Id);
            Assert.Equal((ulong)1337, dbPlaylist.DeezerId);
            Assert.Equal("Eminem", dbPlaylist.Name);
        }

        [Fact]
        public async Task GetPlaylistByULongTest()
        {
            //Arrange
            var playlist = new Playlist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreatePlaylist(playlist);

            var dbPlaylist = await repo.GetPlaylist((ulong)1337);

            //Assert
            Assert.NotNull(dbPlaylist.Id);
            Assert.Equal((ulong)1337, dbPlaylist.DeezerId);
            Assert.Equal("Eminem", dbPlaylist.Name);
        }

        [Fact]
        public async Task GetPlaylistsWithSkipAndTake()
        {
            //Arrange
            IList<Playlist> playlists = new List<Playlist>
            {
                new Playlist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Playlist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Playlist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var playlist in playlists)
            {
                await repo.CreatePlaylist(playlist);
            }

            var dbPlaylists = await repo.GetPlaylists(1, 2);

            //Assert
            Assert.Equal(2, dbPlaylists.Count);

            Assert.Equal((ulong)1338, dbPlaylists[0].DeezerId);
            Assert.Equal("I like trains", dbPlaylists[0].Name);
            Assert.NotNull(dbPlaylists[0].Id);

            Assert.Equal((ulong)1339, dbPlaylists[1].DeezerId);
            Assert.Equal("Banana!", dbPlaylists[1].Name);
            Assert.NotNull(dbPlaylists[1].Id);
        }

        [Fact]
        public async Task GetLastUpdatedPlaylists()
        {
            //Arrange
            IList<Playlist> playlists = new List<Playlist>
            {
                new Playlist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Playlist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Playlist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var playlist in playlists)
            {
                await repo.CreatePlaylist(playlist);

                await Task.Delay(1500);
            }

            var dbPlaylists = await repo.GetLastUpdatedPlaylists(1, 2);

            //Assert
            Assert.Equal(2, dbPlaylists.Count);

            Assert.True(dbPlaylists[1].Updated > dbPlaylists[0].Updated);
        }

        [Fact]
        public async Task GetPlaylists()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Test folder",
                Path = "/mnt/mount/media",
                Size = 5000
            };
            IList<Playlist> playlists = new List<Playlist>
            {
                new Playlist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Playlist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Playlist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            foreach (var playlist in playlists)
            {
                playlist.FolderId = folder.Id;

                await repo.CreatePlaylist(playlist);
            }

            var dbPlaylists = await repo.GetPlaylists();

            //Assert
            Assert.Equal(3, dbPlaylists.Count);

            Assert.Equal((ulong)1337, dbPlaylists[0].DeezerId);
            Assert.Equal("Eminem", dbPlaylists[0].Name);
            Assert.NotNull(dbPlaylists[0].Id);

            Assert.Equal((ulong)1338, dbPlaylists[1].DeezerId);
            Assert.Equal("I like trains", dbPlaylists[1].Name);
            Assert.NotNull(dbPlaylists[1].Id);

            Assert.Equal((ulong)1339, dbPlaylists[2].DeezerId);
            Assert.Equal("Banana!", dbPlaylists[2].Name);
            Assert.NotNull(dbPlaylists[2].Id);
        }

        [Fact]
        public async Task GetPlaylistCount()
        {
            //Arrange
            IList<Playlist> playlists = new List<Playlist>
            {
                new Playlist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Playlist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Playlist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var playlist in playlists)
            {
                await repo.CreatePlaylist(playlist);
            }

            var dbPlaylists = await repo.GetPlaylistCount();

            //Assert
            Assert.Equal(3, dbPlaylists);
        }

        [Fact]
        public async Task GetPlaylistsBySearchTerm()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Test folder",
                Path = "/mnt/mount/media",
                Size = 5000
            };
            IList<Playlist> playlists = new List<Playlist>
            {
                new Playlist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Playlist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Playlist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            foreach (var playlist in playlists)
            {
                playlist.FolderId = folder.Id;

                await repo.CreatePlaylist(playlist);
            }

            var dbPlaylists = await repo.GetPlaylists("in");

            //Assert
            Assert.Equal(2, dbPlaylists.Count);

            Assert.Equal((ulong)1337, dbPlaylists[0].DeezerId);
            Assert.Equal("Eminem", dbPlaylists[0].Name);
            Assert.NotNull(dbPlaylists[0].Id);

            Assert.Equal((ulong)1338, dbPlaylists[1].DeezerId);
            Assert.Equal("I like trains", dbPlaylists[1].Name);
            Assert.NotNull(dbPlaylists[1].Id);
        }

        [Fact]
        public async Task UpdatePlaylist()
        {
            //Arrange
            var playlist = new Playlist
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreatePlaylist(playlist);

            playlist.Name = "Eminem2";

            playlist = await repo.UpdatePlaylist(playlist);

            //Assert
            Assert.NotNull(playlist.Id);
            Assert.Equal((ulong)1337, playlist.DeezerId);
            Assert.Equal("Eminem2", playlist.Name);
        }

        #endregion

        #region Folder

        [Fact]
        public async Task CreateFolder()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Eminem",
                Path = "/mnt/unionfs/Media/Audio/Music"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            //Assert
            Assert.NotNull(folder.Id);
            Assert.Equal("/mnt/unionfs/Media/Audio/Music", folder.Path);
            Assert.Equal("Eminem", folder.Name);
        }

        [Fact]
        public async Task DeleteFolder()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Eminem",
                Path = "/mnt/unionfs/Media/Audio/Music"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            var folderId = folder.Id;

            await repo.DeleteFolder(folder);

            folder = await repo.GetFolder(folderId);

            //Assert
            Assert.Null(folder);
        }

        [Fact]
        public async Task GetFolderByStringTest()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Eminem",
                Path = "/mnt/unionfs/Media/Audio/Music"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            var dbFolder = await repo.GetFolder(folder.Id);

            //Assert
            Assert.NotNull(dbFolder.Id);
            Assert.Equal("/mnt/unionfs/Media/Audio/Music", dbFolder.Path);
            Assert.Equal("Eminem", dbFolder.Name);
        }

        [Fact]
        public async Task GetFolderByULongTest()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Eminem",
                Path = "/mnt/unionfs/Media/Audio/Music"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            var dbFolder = await repo.GetFolder(folder.Id);

            //Assert
            Assert.NotNull(dbFolder.Id);
            Assert.Equal("/mnt/unionfs/Media/Audio/Music", dbFolder.Path);
            Assert.Equal("Eminem", dbFolder.Name);
        }

        [Fact]
        public async Task GetFoldersWithSkipAndTake()
        {
            //Arrange
            IList<Folder> folders = new List<Folder>
            {
                new Folder
                {
                    Name = "Eminem",
                    Path = "/mnt/unionfs/Media/Audio/Music"
                },
                new Folder
                {
                    Name = "I like trains",
                    Path = "/mnt/unionfs/Media/Audio/Music/I like trains"
                },
                new Folder
                {
                    Name = "Banana!",
                    Path = "/mnt/unionfs/Media/Audio/Music/Banana"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var folder in folders)
            {
                await repo.CreateFolder(folder);
            }

            var dbFolders = await repo.GetFolders(1, 2);

            //Assert
            Assert.Equal(2, dbFolders.Count);
        }

        [Fact]
        public async Task GetLastUpdatedFolders()
        {
            //Arrange
            IList<Folder> folders = new List<Folder>
            {
                new Folder
                {
                    Name = "Eminem",
                    Path = "/mnt/unionfs/Media/Audio/Music"
                },
                new Folder
                {
                    Name = "I like trains",
                    Path = "/mnt/unionfs/Media/Audio/Music/I like trains"
                },
                new Folder
                {
                    Name = "Banana!",
                    Path = "/mnt/unionfs/Media/Audio/Music/Banana"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var folder in folders)
            {
                await repo.CreateFolder(folder);

                await Task.Delay(1500);
            }

            var dbFolders = await repo.GetLastUpdatedFolders(1, 2);

            //Assert
            Assert.Equal(2, dbFolders.Count);

            Assert.True(dbFolders[1].Updated > dbFolders[0].Updated);
        }

        [Fact]
        public async Task GetFolders()
        {
            //Arrange
            IList<Folder> folders = new List<Folder>
            {
                new Folder
                {
                    Name = "Eminem",
                    Path = "/mnt/unionfs/Media/Audio/Music"
                },
                new Folder
                {
                    Name = "I like trains",
                    Path = "/mnt/unionfs/Media/Audio/Music/I like trains"
                },
                new Folder
                {
                    Name = "Banana!",
                    Path = "/mnt/unionfs/Media/Audio/Music/Banana"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            foreach (var folder in folders)
            {
                await repo.CreateFolder(folder);
            }

            var dbFolders = await repo.GetFolders();

            //Assert
            Assert.Equal(3, dbFolders.Count);

            Assert.Equal("/mnt/unionfs/Media/Audio/Music", dbFolders[0].Path);
            Assert.Equal("Eminem", dbFolders[0].Name);
            Assert.NotNull(dbFolders[0].Id);

            Assert.Equal("/mnt/unionfs/Media/Audio/Music/I like trains", dbFolders[1].Path);
            Assert.Equal("I like trains", dbFolders[1].Name);
            Assert.NotNull(dbFolders[1].Id);

            Assert.Equal("/mnt/unionfs/Media/Audio/Music/Banana", dbFolders[2].Path);
            Assert.Equal("Banana!", dbFolders[2].Name);
            Assert.NotNull(dbFolders[2].Id);
        }

        [Fact]
        public async Task GetFoldersBySearchTerm()
        {
            //Arrange
            IList<Folder> folders = new List<Folder>
            {
                new Folder
                {
                    Name = "Eminem",
                    Path = "/mnt/unionfs/Media/Audio/Music"
                },
                new Folder
                {
                    Name = "I like trains",
                    Path = "/mnt/unionfs/Media/Audio/Music/I like trains"
                },
                new Folder
                {
                    Name = "Banana!",
                    Path = "/mnt/unionfs/Media/Audio/Music/Banana"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            foreach (var folder in folders)
            {
                await repo.CreateFolder(folder);
            }

            var dbFolders = await repo.GetFolders("Eminem");

            //Assert
            Assert.Equal(1, dbFolders.Count);

            Assert.Equal("/mnt/unionfs/Media/Audio/Music", dbFolders[0].Path);
            Assert.Equal("Eminem", dbFolders[0].Name);
            Assert.NotNull(dbFolders[0].Id);

        }

        [Fact]
        public async Task GetFolderCount()
        {
            //Arrange
            IList<Folder> folders = new List<Folder>
            {
                new Folder
                {
                    Name = "Eminem",
                    Path = "/mnt/unionfs/Media/Audio/Music"
                },
                new Folder
                {
                    Name = "I like trains",
                    Path = "/mnt/unionfs/Media/Audio/Music/I like trains"
                },
                new Folder
                {
                    Name = "Banana!",
                    Path = "/mnt/unionfs/Media/Audio/Music/Banana"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();
            foreach (var folder in folders)
            {
                await repo.CreateFolder(folder);
            }

            var dbFolders = await repo.GetFolderCount();

            //Assert
            Assert.Equal(3, dbFolders);
        }

        [Fact]
        public async Task UpdateFolder()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Eminem",
                Path = "/mnt/unionfs/Media/Audio/Music"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);

            folder.Name = "Eminem2";

            folder = await repo.UpdateFolder(folder);

            //Assert
            Assert.NotNull(folder.Id);
            Assert.Equal("/mnt/unionfs/Media/Audio/Music", folder.Path);
            Assert.Equal("Eminem2", folder.Name);
        }

        [Fact]
        public async Task UpdaGetFolderArtistCountteFolder()
        {
            //Arrange
            var folder = new Folder
            {
                Name = "Eminem",
                Path = "/mnt/unionfs/Media/Audio/Music"
            };
            IList<Artist> artists = new List<Artist>
            {
                new Artist
                {
                    DeezerId = 1337,
                    Name = "Eminem"
                },
                new Artist
                {
                    DeezerId = 1338,
                    Name = "I like trains"
                },
                new Artist
                {
                    DeezerId = 1339,
                    Name = "Banana!"
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateFolder(folder);
            foreach (var artist in artists)
            {
                artist.FolderId = folder.Id;

                await repo.CreateArtist(artist);
            }

            var artistCount = await repo.GetFolderArtistCount(folder);

            //Assert
            Assert.Equal(3, artistCount);
        }

        [Fact]
        public async Task GetFolderSizeCumulated()
        {
            //Arrange
            var folders = new List<Folder>
            {
                new Folder
                {
                    Name = "Eminem",
                    Path = "/mnt/unionfs/Media/Audio/Music",
                    Size = 22335
                },
                new Folder
                {
                    Name = "Eminem1",
                    Path = "/mnt/unionfs/Media/Audio/Music",
                    Size = 154668
                },
                new Folder
                {
                    Name = "Eminem2",
                    Path = "/mnt/unionfs/Media/Audio/Music",
                    Size = 4654566
                }
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            foreach (var folder in folders)
            {
                await repo.CreateFolder(folder);
            }

            var bytes = await repo.GetFolderSizeCumulated();

            //Assert
            Assert.Equal(4831569, bytes);
        }

        #endregion

        #region Genre

        [Fact]
        public async Task CreateGenre()
        {
            //Arrange
            var genre = new Genre
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateGenre(genre);

            //Assert
            Assert.NotNull(genre.Id);
            Assert.Equal((ulong)1337, genre.DeezerId);
            Assert.Equal("Eminem", genre.Name);
        }

        [Fact]
        public async Task GetGenreByStringTest()
        {
            //Arrange
            var genre = new Genre
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateGenre(genre);

            var dbGenre = await repo.GetGenre(genre.Id);

            //Assert
            Assert.NotNull(dbGenre.Id);
            Assert.Equal((ulong)1337, dbGenre.DeezerId);
            Assert.Equal("Eminem", dbGenre.Name);
        }

        [Fact]
        public async Task GetGenreByULongTest()
        {
            //Arrange
            var genre = new Genre
            {
                DeezerId = 1337,
                Name = "Eminem"
            };

            //Act
            var repo = _dataRepositoryFixture.GetCleanRepo();

            await repo.CreateGenre(genre);

            var dbGenre = await repo.GetGenre((ulong)1337);

            //Assert
            Assert.NotNull(dbGenre.Id);
            Assert.Equal((ulong)1337, dbGenre.DeezerId);
            Assert.Equal("Eminem", dbGenre.Name);
        }

        #endregion

    }
}