using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eSkillIconPack
    {
        [Display(Name = "Retail")]
        Disabled,
        [Display(Name = "ReMoDDeD")]
        ReMoDDeD,
        [Display(Name = "Dize")]
        Dize,
    }
}
