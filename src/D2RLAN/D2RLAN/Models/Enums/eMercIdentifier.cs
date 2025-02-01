using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eMercIdentifier
    {
        [Display(Name = "Disabled")]
        Disabled,
        [Display(Name = "Enabled")]
        Enabled,
        [Display(Name = "Enabled (Mini)")]
        EnabledMini,
    }
}
