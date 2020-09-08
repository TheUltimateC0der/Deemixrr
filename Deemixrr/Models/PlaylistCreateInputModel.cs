using Microsoft.AspNetCore.Mvc.Rendering;

namespace Deemixrr.Models
{
    public class PlaylistCreateInputModel
    {
        public ulong DeezerId { get; set; }

        public string DeezerIds { get; set; }

        public string FolderId { get; set; }
        public SelectList Folders { get; set; }
    }
}