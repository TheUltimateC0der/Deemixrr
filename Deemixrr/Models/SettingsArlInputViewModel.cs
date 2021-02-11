using System;
using System.ComponentModel.DataAnnotations;

namespace Deemixrr.Models
{
    public class SettingsArlInputViewModel
    {
        [Required]
        public string Arl { get; set; }

        public DateTime LastWrite { get; set; }
    }
}