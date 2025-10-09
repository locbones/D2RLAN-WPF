using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eStringColoring
    {
        [Display(Name = "Default")]
        Default,
        [Display(Name = "Enhanced Clarity")]
        Clarity,
        [Display(Name = "Clarity + Element Colors")]
        ClarityElement
    }
}
