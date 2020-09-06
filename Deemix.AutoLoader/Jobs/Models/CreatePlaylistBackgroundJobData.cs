namespace Deemix.AutoLoader.Jobs.Models
{
    public class CreatePlaylistBackgroundJobData
    {
        public ulong DeezerId { get; set; }
        public string DeezerIds { get; set; }

        public string FolderId { get; set; }
    }
}