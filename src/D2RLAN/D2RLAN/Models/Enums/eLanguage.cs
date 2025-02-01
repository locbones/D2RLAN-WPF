using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Models.Enums
{
    public enum eLanguage
    {
        [Display(Name = "English (en)")]
        English,
        [Display(Name = "German (de)")]
        German,
        [Display(Name = "Spanish(Spain) (es-ES)")]
        SpanishSpain,
        [Display(Name = "Spanish(Mexico) (es-MX)")]
        SpanishMexico,
        [Display(Name = "French (fr)")]
        French,
        [Display(Name = "Italian (it)")]
        Italian,
        [Display(Name = "Japanese (ja-JP)")]
        Japanese,
        [Display(Name = "Korean (ko-KR)")]
        Korean,
        [Display(Name = "Polish (pl)")]
        Polish,
        [Display(Name = "Portugese (pt-BR)")]
        Portugese,
        [Display(Name = "Russian (ru)")]
        Russian,
        [Display(Name = "Chinese (zh-CN)")]
        Chinese,
        [Display(Name = "Chinese(Taiwan) (zh-TW)")]
        ChineseTaiwan
    }
}
