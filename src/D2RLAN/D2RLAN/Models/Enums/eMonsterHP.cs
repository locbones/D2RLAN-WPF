using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eMonsterHP
    {
        [Display(Name = "Retail")]
        Retail,
        [Display(Name = "Basic")]
        BasicNoP,
        [Display(Name = "Basic (w/ Percents)")]
        BasicP,
        [Display(Name = "Advanced")]
        AdvancedNoP,
        [Display(Name = "Advanced (w/ Percents)")]
        AdvancedP,
    }
}
