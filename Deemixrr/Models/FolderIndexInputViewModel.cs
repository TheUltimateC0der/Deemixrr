using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Deemixrr.Data;

namespace Deemixrr.Models
{
    public class FolderIndexInputViewModel
    {
        [Description("Searchterm")]
        [Display(Prompt = "Search for either Name or path ...")]
        public string SearchTerm { get; set; }

        public IList<Folder> Folders { get; set; }
    }
}