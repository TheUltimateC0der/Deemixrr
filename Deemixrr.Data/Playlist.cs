using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Deemixrr.Data
{
    public class Playlist : CreatedAndUpdated
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string Id { get; set; }

        public string Name { get; set; }

        public ulong DeezerId { get; set; }

        public uint NumberOfTracks { get; set; }

        [Required]
        public string FolderId { get; set; }
        public Folder Folder { get; set; }
    }
}