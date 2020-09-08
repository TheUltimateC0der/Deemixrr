using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Deemixrr.Data;

namespace Deemixrr.Models
{
    public class ArtistIndexInputViewModel
    {
        [Description("Searchterm")]
        [Display(Prompt = "Search for either Name or Deezer Id ...")]
        public string SearchTerm { get; set; }

        public IList<Artist> Artists { get; set; }
    }
}