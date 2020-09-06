using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Deemix.AutoLoader.Data;

namespace Deemix.AutoLoader.Models
{
    public class FolderIndexInputViewModel
    {
        [Description("Searchterm")]
        [Display(Prompt = "Search for either Name or path ...")]
        public string SearchTerm { get; set; }

        public IList<Folder> Folders { get; set; }
    }
}