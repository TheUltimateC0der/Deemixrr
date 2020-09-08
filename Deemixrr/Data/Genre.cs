using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Deemixrr.Data
{
    public class Genre : CreatedAndUpdated
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string Id { get; set; }

        public string Name { get; set; }

        public ulong DeezerId { get; set; }
    }
}