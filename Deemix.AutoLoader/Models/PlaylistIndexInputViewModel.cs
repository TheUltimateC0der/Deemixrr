using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Deemix.AutoLoader.Data;

namespace Deemix.AutoLoader.Models
{
    public class PlaylistIndexInputViewModel
    {
        [Description("Searchterm")]
        [Display(Prompt = "Search for either Name or Deezer Id ...")]
        public string SearchTerm { get; set; }

        public IList<Playlist> Playlists { get; set; }
    }
}