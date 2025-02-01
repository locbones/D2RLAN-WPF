using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eBackup
    {
        [Display(Name = "Disabled")]
        Disabled,
        [Display(Name = "Backup every 5 min")]
        FiveMinutes,
        [Display(Name = "Backup every 15 min")]
        FifteenMinutes,
        [Display(Name = "Backup every 30 min")]
        ThirtyMinutes,
        [Display(Name = "Backup every 1 hour")]
        OneHour,
    }
}
