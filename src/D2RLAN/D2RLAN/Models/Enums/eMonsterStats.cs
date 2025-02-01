using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eMonsterStats
    {
        [Display(Name = "Disabled")]
        Disabled,
        [Display(Name = "Background")]
        Background,
        [Display(Name = "No Background")]
        NoBackground,
    }
}
