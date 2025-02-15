using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eUiThemes
    {
        [Display(Name = "Don't Modify")]
        Disabled,
        [Display(Name = "Standard UI")]
        Standard,
        [Display(Name = "RMD (Red)")]
        ReMoDDeD1,
        [Display(Name = "RMD (Blue)")]
        ReMoDDeD2,
        [Display(Name = "RMD (Purple)")]
        ReMoDDeD3,
        [Display(Name = "RMD (Gold)")]
        ReMoDDeD4,
        [Display(Name = "RMD (Green)")]
        ReMoDDeD5,
        [Display(Name = "RMD (Dark)")]
        ReMoDDeD6,
    }
}
