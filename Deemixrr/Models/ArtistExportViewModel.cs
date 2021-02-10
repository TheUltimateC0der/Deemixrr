using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Deemixrr.Data;

namespace Deemixrr.Models
{
    public class ArtistExportViewModel
    {
        public IList<Artist> Artists { get; set; }
        public string Names_csv { get; set; }
        public string Names_newline { get; set; }
        public string IDs_csv { get; set; }
    }
}
