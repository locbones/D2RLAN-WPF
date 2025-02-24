using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eHudDesign
    {
        [Display(Name = "ReMoDDeD")]
        ReMoDDeD,
        [Display(Name = "Standard")]
        Standard,
        [Display(Name = "Merged")]
        Merged,
        [Display(Name = "Merged Mini")]
        MergedMini,
    }
}
