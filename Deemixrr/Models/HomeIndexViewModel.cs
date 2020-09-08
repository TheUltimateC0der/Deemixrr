using System.Collections.Generic;

using Deemixrr.Data;

namespace Deemixrr.Models
{
    public class HomeIndexViewModel
    {
        public int ArtistCount { get; set; }

        public int PlaylistCount { get; set; }

        public int FolderCount { get; set; }

        public long FolderSizeCumulated { get; set; }


        public IList<Artist> Artists { get; set; }

        public IList<Playlist> Playlists { get; set; }

        public IList<Folder> Folders { get; set; }
    }
}