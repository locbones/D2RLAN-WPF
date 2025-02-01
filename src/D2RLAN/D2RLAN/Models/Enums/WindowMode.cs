using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eWindowMode
    {
        [Display(Name = "Fullscreen")]
        Fullscreen,
        [Display(Name = "Windowed")]
        Window,
        [Display(Name = "Borderless Full")]
        BorderlessFullscreen,
        [Display(Name = "Borderless Window")]
        BorderlessWindowed,
    }
}
