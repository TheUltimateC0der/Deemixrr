using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Deemixrr.Data;

namespace Deemixrr.Models
{
    public class PlaylistUpdateViewModel
    {
        public IList<Playlist> Playlists { get; set; }
    }
}