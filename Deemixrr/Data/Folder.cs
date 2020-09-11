using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Deemixrr.Data
{
    public class Folder : CreatedAndUpdated
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        [NotMapped]
        public string NamePath => $"{Name} - ({Path})";

        public Enums.ProcessingState State { get; set; }

        public long Size { get; set; }
    }
}