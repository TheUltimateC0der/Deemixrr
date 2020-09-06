namespace Deemix.AutoLoader.Jobs.Models
{
    public class CreateArtistBackgroundJobData
    {
        public ulong ArtistDeezerId { get; set; }
        public ulong PlaylistDeezerId { get; set; }

        public string ArtistDeezerIds { get; set; }

        public string FolderId { get; set; }
    }
}