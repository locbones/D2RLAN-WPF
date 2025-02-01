using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eFont
    {
        [Display(Name = "Exocet (Default)")]
        Exocet,
        [Display(Name = "Akaya Telivigala")]
        AkayaTelivigala,
        [Display(Name = "ReggaeOne")]
        ReggaeOne,
        [Display(Name = "SansitaSwashed")]
        SansitaSwashed,
        [Display(Name = "DM Mono")]
        DMMono,
        [Display(Name = "Girassol")]
        Girassol,
        [Display(Name = "Turret Road")]
        TurretRoad,
        [Display(Name = "Literata")]
        Literata,
        [Display(Name = "Zilla Slab)")]
        ZillaSlab,
        [Display(Name = "Aref Ruqaa")]
        ArefRuqaa,
        [Display(Name = "Formal 436 BT")]
        Formal436BT,
        [Display(Name = "FontIn (PoE)")]
        FontInPoE,
    }
}
