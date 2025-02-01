using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eExpRate
    {
        [Display(Name = "1x")]
        OneX,
        [Display(Name = "2x")]
        TwoX,
        [Display(Name = "3x")]
        ThreeX,
        [Display(Name = "4x")]
        FourX,
        [Display(Name = "5x")]
        FiveX,
    }
}
