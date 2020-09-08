using Microsoft.AspNetCore.Mvc.Rendering;

namespace Deemixrr.Models
{
    public class ArtistCreateInputModel
    {
        public ulong ArtistDeezerId { get; set; }
        public ulong PlaylistDeezerId { get; set; }

        public string ArtistDeezerIds { get; set; }

        public string FolderId { get; set; }
        public SelectList Folders { get; set; }

    }
}