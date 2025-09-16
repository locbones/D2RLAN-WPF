using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eBeaconStartup
    {
        [Display(Name = "Start Minimized")]
        EnabledMin,
        [Display(Name = "Start on Launch")]
        Enabled,
        [Display(Name = "Disabled")]
        Disabled,
    }
}
