using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eItemDisplay
    {
        [Display(Name = "No Icons")]
        NoIcons,
        [Display(Name = "Item + Rune Icons")]
        ItemRuneIcons,
        [Display(Name = "Item Icons Only")]
        ItemIconsOnly,
        [Display(Name = "Rune Icons Only")]
        RuneIconsOnly,
    }
}
