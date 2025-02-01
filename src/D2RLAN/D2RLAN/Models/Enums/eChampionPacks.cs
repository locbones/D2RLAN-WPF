using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eChampionPacks
    {
        [Display(Name = "Standard")]
        Standard,
        [Display(Name = "Min/Max (+1)")]
        MinMaxOne,
        [Display(Name = "Min/Max (+2)")]
        MinMaxTwo,
        [Display(Name = "Min/Max (+3)")]
        MinMaxThree,
        [Display(Name = "Min/Max (+4)")]
        MinMaxFour,
        [Display(Name = "Min/Max (+5)")]
        MinMaxFive,
    }
}
