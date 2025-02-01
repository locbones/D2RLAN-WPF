using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eGroupSizes
    {
        [Display(Name = "0")]
        Zero,
        [Display(Name = "+1")]
        One,
        [Display(Name = "+2")]
        Two,
        [Display(Name = "+3")]
        Three,
        [Display(Name = "+4")]
        Four,
        [Display(Name = "+5")]
        Five,
    }
}
