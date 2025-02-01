using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eMapLayouts
    {
        [Display(Name = "Default")]
        Default,
        [Display(Name = "Tower")]
        Tower,
        [Display(Name = "Catacombs")]
        Catacombs,
        [Display(Name = "Ancient Tunnels")]
        AncientTunnels,
        [Display(Name = "Lower Kurast (V++)")]
        LowerKurast,
        [Display(Name = "Durance of Hate")]
        DuranceOfHate,
        [Display(Name = "Hellforge")]
        Hellforge,
        [Display(Name = "Worldstone Keep")]
        WorldstoneKeep,
        [Display(Name = "I'm a Cheater (V++)")]
        Cheater,
    }
}
