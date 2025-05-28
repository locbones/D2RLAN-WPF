--- Filter Title: Sheep's Base Filter 
--- Filter Type: (Generic base filter)
--- Filter Description: A generic base filter looking to keep the Diablo II aesthetic.\nFocuses mainly on notifying you of good drops and hiding complete junk.
return {
    rules = {
        -- Rule 1: Hide all 1 socket superior or lower items
        {
            codes = "allitems",
            sockets = "1",
            quality = "3-",
            hide = true,
            --prefix = "[hidden]", --left for testing reasons

        },
        --- Rule 2: hides zero skill paladin weapons
        {
            codes = { "scp", "wsp", "gsc", "7sc", "7ws", "7gs", "9sc", "9ws", "9gs" },
            hide = true,
            --prefix = "hidden",
            stat = { index = 83, op = "<=", value = 0, param = 3 } -- Value = How many +skills, Param = 3 for Paladin
        },
		--- rule 2b: Shows 3 skill scepters in red
		{
            codes = { "scp", "wsp", "gsc", "7sc", "7ws", "7gs", "9sc", "9ws", "9gs" },
            hide = False,
			prefix = "ÿcE",
			suffix = "3 Skill",
            stat = { index = 83, op = "==", value = 3, param = 3 } -- Value = How many +skills, Param = 3 for Paladin
        },
        -- rule 3:hiding scrolls on ground
        {
            codes = { "tsc", "isc" },
            hide = true
        },
        ---rule 4: hiding potions on ground (minus rejuvs) after level 25
        {
            codes = {"hp1", "hp2", "hp3", "hp4", "hp5", "mp1", "mp2", "mp3", "mp4", "mp5"},
            pstat = { index = 12, op = ">=", value = 25 }, -- Char Level is >= 25
            hide = true
        },
        ---rule 5: Prefix and notify unique amulets
        {
            code = "amu",           
            quality = "7",
            hide = false,
            prefix = "{red}**{gold}",
            suffix = "{red}**",
            notify = true,
            notify_message = "Unique Nearby"

        },
        ---rule 6: Prefix and notify unique Rings
        {
            code = "rin",           
            quality = "7",
            prefix = "{red}**{gold}",
            suffix = "{red}**",
            hide = false,
            notify = true,
            notify_message = "Unique Nearby"

        }, ---rule 7: Prefix and notify set amulets
        {
            code = "amu",          
            quality = "5",
            hide = false,
            prefix = "{red}**{green}",
            suffix = "{red}**",
            notify = true,
            notify_message = "Set Nearby"

        },
        ---rule 8: Prefix and notify set Rings
        {
            code = "rin",          
            quality = "5",
            prefix = "{red}**(green}",
            suffix = "{red}**",
            hide = false,
            notify = true,
            notify_message = "Set Nearby"

        },
        ---rule 9: High Runes (Ugh)
        {
            codes = {"r23","r24","r25","r26","r27","r28","r29","r30","r31","r32","r33"},            
            hide = false,
            notify = true,
            notify_message = "High Rune Nearby"

        },
        ---rule 9b: Ultra Runes
        {
            code = "r34",         
            hide = false,
            notify = true,
            prefix = "ÿcS|X|",
            notify_message = "Ultra Rune Nearby"

        },
        ---rule 9c: Ultra Runes
        {
            code = "r35",
            hide = false,
            notify = true,
            prefix = "ÿcS|X|",
            notify_message = "Ultra Rune Nearby"

        },
        ---rule 9d: Ultra Runes
        {
            code = "r36",           
            hide = false,
            notify = true,
            prefix = "ÿcS|X|",
            notify_message = "You won The Lottery"

        },
        ---rule 10: Unique Archons
        {
            code = "utp",
            quality = 7,
            hide = false,
            notify = true,
            prefix = "ÿcE",
            notify_message = "Rare Unique Nearby"

        },
        ---rule 11: Potential high quality uniques (ie hydra master)
        {
            codes = { "6ls", "ci1", "xld", "uhn", "aqv" },
            quality = 7,
            hide = false,
            notify = true,
            prefix = "ÿcO",
            notify_message = "Variable Unique Nearby"

        },
        ---rule 12: Unique Charms
        {
            codes = { "cm1", "cm2", "cm3", "m32", "m33", "m34", "m35" },
            quality = 7,
            hide = false,
            notify = true,
            notify_message = "Unique Nearby"

        },
        ---rule 13: Facet Charms
        {
            codes = { "j00", "jew" },
            quality = 7,
            hide = false,
            notify = true,
            prefix = "$",
            suffix = "$",
            notify_message = "Unique Nearby"

        },
        ---rule 14: Treasure Chest/gem vein
        {
            codes = {"Rgx","y09", "y10", "y11", "y12", "y13", "y14", "y15", "y16", "y17", "y18", "y19", "y20", "y21", "y22", "y23", "y24", "y25", "y26", "y27", "y28", "y29", "y30", "y31", "y32", "y33" },
            hide = false,
            notify = true,
            notify_message = "Treasure Nearby"
        },

        ---rule 15: Valuable Uniques (would be better to use unique indexing but lazy"
        {
            codes = { "amb", "8hx", "6cb", "6lw", "7ja", "7kr", "7bw", "7yw", "7gw", "obf", "oba", "ob7", "7gd", "upl", "ult", "uar", "uth", "ula", "ulc", "uhc", "utb", "xtb", "ci3", "umg", "nef", "pab", "pa9", "paa", "uit", "urg", "uuc", "vg3" },
            quality = 7,
            hide = false,
            notify = true,
            prefix = "$",
            suffix = "$",
            notify_message = "Quality Unique Nearby"
        },
		---rule 16: good superior bases no socket sorting
        {
            codes = {"7wa","7cr","ci0","ci1","ci2","ci3","utp","7b8","amc"},
            quality = "3",
            hide = false,
            notify = false,
            prefix = "{Red}|GB|{white}",

        },
		---rule 17: hiding non-superior/non-class bases by level (helms) still shows 3 socket ones
         {
            codes = {"cap","skp","hlm","fhl","ghm","crn","msk","bhm"},
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 18: hiding non-superior/non-class bases by level (Body Armors) still shows 3 socket ones
        {
            codes = {"qui","lea","hla","stu","rng","scl","chn","brs","spl","plt","fld","gth","ful","aar","ltp"},
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 19: hiding non-superior/non-class bases by level (Shields) still shows 3 socket ones (higher level will force 4os)
        {
            codes = {"buc","sml","lrg","kit","tow","gts","bsh","spk" },
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
           ---rule 20: hiding non-superior/non-class bases by level (gloves) still shows 3 socket ones
        {
            codes = {"lgl","vgl","hgl","mgl","tgl" },
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
                   ---rule 21: hiding non-superior/non-class bases by level (boots) still shows 3 socket ones
        {
            codes = {"lbt","vbt","mbt","tbt","hbt"},
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
                           ---rule 22: hiding non-superior/non-class bases by level (belts) still shows 2 socket ones
        {
            codes = {"lbl","vbl","mbl","tbl","hbl"},
            sockets = "1-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 23: hiding non-superior/non-class bases by level (helms) still shows 3 socket ones
        {
            codes = {"cap","skp","hlm","fhl","ghm","crn","msk","bhm","xap","xkp","xlm","xhl","xhm","xrn","xsk","xsh","xpk"},
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
        
        ---rule 24: hiding non-superior/non-class bases by level (Body Armors) still shows 4 socket ones 
        {
            codes = {"qui","lea","hla","stu","rng","scl","chn","brs","spl","plt","fld","gth","ful","aar","ltp","xui","xea","xla","xtu","xng","xcl","xhn","xrs","xpl","xlt","xld","xth","xul","xar","xtp"},
            sockets = "3-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
        ---rule 25: hiding non-superior/non-class bases by level (Shields) still shows 4 socket ones
        {
            codes = {"buc","sml","lrg","kit","tow","gts","bsh","spk","xuc","xml","xrg","xit","xow","xts","xsh","xpk"},
            sockets = "3-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
        ---rule 26: hiding non-superior/non-class bases by level (gloves) still shows 3 socket ones
        {
            codes = {"lgl","vgl","hgl","mgl","tgl","xlg","xvg","xhg","xmg","xtg" },
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
        ---rule 27: hiding non-superior/non-class bases by level (boots) still shows 3 socket ones
        {
            codes = {"lbt","vbt","mbt","tbt","hbt","xlb","xvb","xmb","xtb","xhb"},
            sockets = "2-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
         ---rule 28: hiding non-superior/non-class bases by level (belts) still shows 2 socket ones
        {
            codes = {"lbl","vbl","mbl","tbl","hbl","zlb","zvb","zmb","ztb","zhb"},
            sockets = "1-",
            quality = "2-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
         ---rule 29: hiding non-superior/non-class bases by level (weapons leaving runeword bases ie flails,crystal swords/ also skips bases that can roll skills) 
        {
            codes = {"hax","axe","2ax","wax","lax","bax","btx","gax","gix","mac","mst","whm","mau","gma","ssd","scm","sbr","flc","bsd","lsd","wsd","clm","gis","flb","gsd","dgr","dir","kri","bld","tkf","tax","bkf","bal","jav","pil","ssp","glv","tsp","spr","tri","brn","spt","pik","bar","vou","scy","pax","hal","wsc","sbw","hbw","lbw","cbw","sbb","lbb","swb","lwb","lxb","mxb","hxb","rxb"},
            quality = "3-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 30: hiding non-superior/non-class runeword bases lacking sockets (ie flails,crystal swords) 
        {
            codes = {"fla","crs","9fl","7fl","7cr","9cr"},
            sockets = "3-",
            quality = "3-",
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
         ---rule 29: hiding non-superior/non-class bases by level (weapons leaving runeword bases ie flails,crystal swords/ also skips bases that can roll skills) 
        {
            codes = {"9ha","9ax","92a","9wa","9la","9ba","9bt","9ga","9gi","9ma","9mt","9wh","9m9","9gm","9ss","9sm","9sb","9fc","9bs","9ls","9wd","9cm","9gs","9fb","9gd","9dg","9di","9kr","9bl","9tk","9ta","9bk","9b8","9ja","9pi","9s9","9gl","9ts","9sr","9tr","9br","9st","9p9","9b7","9vo","9s8","9pa","9h9","9wc","8sb","8hb","8lb","8cb","8s8","8l8","8sw","8lw","8lx","8mx","8hx","8rx"},
            quality = "3-",
            pstat = { index = 12, op = ">=", value = 80 }, -- Char Level is >= 80
            hide = true
        },
        ---rule 30: hiding non-class base magic items by level (helms)
         {
            codes = {"cap","skp","hlm","fhl","ghm","crn","msk","bhm"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 31:  hiding non-class base magic items by level (Body Armors) 
         {  codes = {"qui","lea","hla","stu","rng","scl","chn","brs","spl","plt","fld","gth","ful","aar","ltp"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 32:  hiding non-class base magic items by level (Shields) 
          {  
            codes = {"buc","sml","lrg","kit","tow","gts","bsh","spk" },
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
         ---rule 33:  hiding non-class base magic items by level(gloves)
         {  
            codes = {"lgl","vgl","hgl","mgl","tgl" },
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 34:  hiding non-class base magic items by level(boots) 
         {   
            codes = {"lbt","vbt","mbt","tbt","hbt"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 35:  hiding non-class base magic items by level (belts)
         {   
            codes = {"lbl","vbl","mbl","tbl","hbl","Bb1","Bb4"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 36: hiding non-class base magic items by level (helms) 
        {
            codes = {"cap","skp","hlm","fhl","ghm","crn","msk","bhm","xap","xkp","xlm","xhl","xhm","xrn","xsk","xsh","xpk"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        
        ---rule 37:  hiding non-class base magic items by level (Body Armors)
        {
            codes = {"qui","lea","hla","stu","rng","scl","chn","brs","spl","plt","fld","gth","ful","aar","ltp","xui","xea","xla","xtu","xng","xcl","xhn","xrs","xpl","xlt","xld","xth","xul","xar","xtp"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 38: hiding non-class base magic items by level (Shields) 
         {   
            codes = {"buc","sml","lrg","kit","tow","gts","bsh","spk","xuc","xml","xrg","xit","xow","xts","xsh","xpk"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 39:  hiding non-class base magic items by level (gloves)
         {   
            codes = {"lgl","vgl","hgl","mgl","tgl","xlg","xvg","xhg","xmg","xtg" },
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 40: hiding non-class base magic items by level(boots) es
        {
            codes = {"lbt","vbt","mbt","tbt","hbt","xlb","xvb","xmb","xtb","xhb"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
         ---rule 41:  hiding non-class base magic items by level (belts) 
          {   
            codes = {"lbl","vbl","mbl","tbl","hbl","zlb","zvb","zmb","ztb","zhb","Bb1","Bb2","Bb4","Bb5"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
         ---rule 42:  hiding non-class base magic items by level (weapons leaving runeword bases ie flails,crystal swords/ also skips bases that can roll skills) 
        {
            codes = {"hax","axe","2ax","wax","lax","bax","btx","gax","gix","mac","mst","whm","mau","gma","ssd","scm","sbr","flc","bsd","lsd","wsd","clm","gis","flb","gsd","dgr","dir","kri","bld","tkf","tax","bkf","bal","jav","pil","ssp","glv","tsp","spr","tri","brn","spt","pik","bar","vou","scy","pax","hal","wsc","sbw","hbw","lbw","cbw","sbb","lbb","swb","lwb","lxb","mxb","hxb","rxb"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 43:  hiding non-class base magic items by level (ie flails,crystal swords) 
        {
            codes = {"fla","crs","9fl"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
         ---rule 44:  hiding non-class base magic items by level (weapons leaving runeword bases ie flails,crystal swords/ also skips bases that can roll skills) 
        {
            codes = {"9ha","9ax","92a","9wa","9la","9ba","9bt","9ga","9gi","9ma","9mt","9wh","9m9","9gm","9ss","9sm","9sb","9fc","9bs","9ls","9wd","9cm","9gs","9fb","9gd","9dg","9di","9kr","9bl","9tk","9ta","9bk","9b8","9ja","9pi","9s9","9gl","9ts","9sr","9tr","9br","9st","9p9","9b7","9vo","9s8","9pa","9h9","9wc","8sb","8hb","8lb","8cb","8s8","8l8","8sw","8lw","8lx","8mx","8hx","8rx"},
            quality = 4,
            pstat = { index = 12, op = ">=", value = 40 }, -- Char Level is >= 40
            hide = true
        },
        ---rule 45: hide all non rare or greater arrows
        {
            code = "aqv",
            quality = "4-",
            hide = true
        },
         ---rule 46: hide low gold piles by level
        {
            code = "gld",
            stat = { index = 14, op = "<=", value=999}, --hides gold under 1k
            pstat = { index = 12, op = ">=", value = 60 }, -- Char Level is >= 60
            hide = true
        },
         ---rule 46b: hide low gold piles by level
        {
            code = "gld",
            stat = { index = 14, op = "<=", value=99}, --hides gold under 1k
            pstat = { index = 12, op = ">=", value = 20 }, -- Char Level is >= 60
            hide = true
        },
         --rule 47 Hides non superior items with less than 2 Sockets if the player is above 80 does not include staff mod items
        {  
            codes = {"cap", "skp", "hlm", "fhl", "ghm", "crn", "msk", "qui", "lea", "hla", "stu", "rng", "scl", "chn", "brs", "spl", "plt", "fld", "gth", "ful", "aar", "ltp", "buc", "sml", "lrg", "kit", "tow", "gts", "lgl", "vgl", "mgl", "tgl", "hgl", "lbt", "vbt", "mbt", "tbt", "hbt", "lbl", "vbl", "mbl", "tbl", "hbl", "bhm", "bsh", "spk", "xap", "xkp", "xlm", "xhl", "xhm", "xrn", "xsk", "xui", "xea", "xla", "xtu", "xng", "xcl", "xhn", "xrs", "xpl", "xlt", "xld", "xth", "xul", "xar", "xtp", "xuc", "xml", "xrg", "xit", "xow", "xts", "xlg", "xvg", "xmg", "xtg", "xhg", "xlb", "xvb", "xmb", "xtb", "xhb", "zlb", "zvb", "zmb", "ztb", "zhb", "xh9", "xsh", "xpk","ci0", "ci1", "ci2", "ci3", "uap", "ukp", "ulm", "uhl", "uhm", "urn", "usk", "uui", "uea", "ula", "utu", "ung", "ucl", "uhn", "urs", "upl", "ult", "uld", "uth", "uul", "uar", "utp", "uuc", "uml", "urg", "uit", "uow", "uts", "ulg", "uvg", "umg", "utg", "uhg", "ulb", "uvb", "umb", "utb", "uhb", "ulc", "uvc", "umc", "utc", "uhc", "uh9", "ush", "upk","Wp1", "Wp2", "Wp3", "Gg1", "Gg2", "Gg3", "Ab1", "Ab2", "Ab3", "Ab4", "Ab5", "Ab6", "Bp1", "Bp2", "Bp3", "Bp4", "Bp5", "Bp6", "Oa1", "Oa2", "Oa3", "Vg1", "Vg2", "Vg3", "Vg4", "Vg5", "Vg6", "Bb1", "Bb2", "Bb3", "Bb4", "Bb5", "Bb6", "Zc1", "Zc2", "Zc3", "Zc4", "Zc5", "Zc6", "St1", "St2", "Pc1", "Pc2", "Ag1", "Ag2", "Ag3", "Ag4", "Ag5", "Ag6", "Na1", "Na2", "Na3", "Na4", "Na5", "Na6", "Sa1", "Sa2", "Sa3", "Sa4", "Sa5", "Sa6", "St3", "St4", "St5", "St6", "St7", "St8", "St9", "St0", "hax", "axe", "2ax", "mpi", "wax", "lax", "bax", "btx", "gax", "gix","mac", "mst", "whm", "mau", "gma", "ssd", "scm", "sbr", "flc", "bsd", "lsd", "wsd", "clm", "gis", "bsw", "flb", "gsd", "dgr", "dir", "kri", "bld", "tkf", "tax", "bkf", "bal", "jav", "pil", "ssp", "glv", "tsp", "spr", "tri", "brn", "spt", "pik", "bar", "vou", "scy", "pax", "hal", "wsc", "sst", "lst", "cst", "bst", "sbw", "hbw", "lbw", "cbw", "sbb", "lbb", "swb", "lwb", "lxb", "mxb", "hxb", "rxb","9ha", "9ax", "92a", "9mp", "9wa", "9la", "9ba", "9bt", "9ga", "9gi", "9ma", "9mt", "9wh", "9m9", "9gm", "9ss", "9sm", "9sb", "9fc", "9cr", "9bs", "9ls", "9wd","9cm", "9gs", "9b9", "9fb", "9gd", "9dg", "9di", "9kr", "9bl", "9tk", "9ta", "9bk", "9b8", "9ja", "9pi", "9s9", "9gl", "9ts", "9sr", "9tr", "9br", "9st", "9p9", "9b7", "9vo", "9s8", "9pa", "9h9", "9wc", "8ss", "8ls", "8cs", "8bs","8sb", "8hb", "8lb", "8cb", "8s8", "8l8", "8sw", "8lw", "8lx", "8mx", "8hx", "8rx","7ha", "7ax", "72a", "7mp", "7la", "7ba", "7bt", "7ga", "7gi","7ma", "7mt", "7wh", "7m7", "7gm", "7ss", "7sm", "7sb", "7fc", "7bs", "7ls", "7wd", "7cm", "7gs", "7b7", "7fb", "7gd", "7dg", "7di", "7kr", "7bl", "7tk", "7ta", "7bk", "7ja", "7pi", "7s7", "7gl", "7ts", "7sr", "7tr", "7br", "7st", "7p7", "7o7", "7vo", "7s8", "7pa", "7h7", "7wc", "6ss", "6ls", "6cs", "6bs", "6sb", "6hb", "6lb", "6cb", "6s7", "6l7", "6sw", "6lw", "6lx", "6mx", "6hx", "6rx","am1", "am2", "am3", "am4", "am6", "am7", "am8", "am9","amd", "ame","Pm1", "Pm2", "Pm3", "Bm1", "Bm2", "Bm3", "Bm4", "Bm5", "Bm6", "Bm7", "Bm8", "Bm9", "Bf1", "Bf2", "Bf3", "Bf4", "Bf5", "Bf6", "D00", "Ss1", "Ss2"},
            quality = "2-",
            sockets = "1-",
            pstat = { index = 12, op = ">=", value = 80 }, -- value is character level
            hide = true
        }

    }
}




--[[

Exposed/Available Arguments:
code or codes = Define the 3-character item code the game uses for the item
index = Define the Set or Unique iitem index to search (useful for unid items)
name_override = Change name to w/e you want, overriding it's normal display name
prefix = Apply the designated text before the item name
suffix = Apply the designated text after the item name
hide = True or False. Hide or Display all items that match the provided code(s)
notify = True or False. Send a screen text message when item has dropped
notify_message = Optional. Change the default notification message to a custom one
location (optional) = onground, equipped, onplayer. Only applies while item is in certain locations

index = 0+, Unique numeric identifiers for set/unique items, useful for determining set/unique while still unid. Can be used as both a condition and an output
quality = 1-7, Inferior through Unique. Can be used as both a condition and an output. Can use values such as 3+ or 3- for greater than or less than
rarity = 0-2, Normal, Exceptional, Elite. Can be used as both a condition and an output
sockets = 0-6, None to Max. Can be used as both a condition and an output. Can use values such as 3+ or 3- for greater than or less than
ethereal = True or False. Can be used as both a condition and an output


stat = Check the item for specific stats, can define multiple stat requirements.  Can be used as both a condition and an output
stat Example Usage: stat = { index = 39, op = ">=", value = 25 }
This checks stat #39, which is fire resistance, and confirms whether or not it's greater than or equal to 25
You may also use this stat check to compare a range of stats, or stats special parameters. Conditional range:
stat Example Usage: stat = { index = 39, op = "between", value = {25,35} }. This checks to verify the stat is between the min and max values
You can also use it for parameters such as:
stat Example Usage: stat = { index = 97, op = ">=", value = 2, param = 36}
This checks stat#97, which grans a non-class skill, and sees if the value is 2 or higher.
It also checks the param of the skill, to make sure it's giving +2 or more specifically to Fire Bolt, which is skill id 36.

------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------

Available Armor, Misc and Weapon Item Codes:

--Armors--
cap -- Cap
skp -- Skull Cap
hlm -- Helm
fhl -- Full Helm
ghm -- Great Helm
crn -- Crown
msk -- Mask
qui -- Quilted Armor
lea -- Leather Armor
hla -- Hard Leather Armor
stu -- Studded Leather
rng -- Ring Mail
scl -- Scale Mail
chn -- Chain Mail
brs -- Breast Plate
spl -- Splint Mail
plt -- Plate Mail
fld -- Field Plate
gth -- Gothic Plate
ful -- Full Plate Mail
aar -- Ancient Armor
ltp -- Light Plate
buc -- Buckler
sml -- Small Shield
lrg -- Large Shield
kit -- Kite Shield
tow -- Tower Shield
gts -- Gothic Shield
lgl -- Leather Gloves
vgl -- Heavy Gloves
mgl -- Chain Gloves
tgl -- Light Gauntlets
hgl -- Gauntlets
lbt -- Boots
vbt -- Heavy Boots
mbt -- Chain Boots
tbt -- Light Plated Boots
hbt -- Greaves
lbl -- Sash
vbl -- Light Belt
mbl -- Belt
tbl -- Heavy Belt
hbl -- Plated Belt
bhm -- Bone Helm
bsh -- Bone Shield
spk -- Spiked Shield
xap -- War Hat
xkp -- Sallet
xlm -- Casque
xhl -- Basinet
xhm -- Winged Helm
xrn -- Grand Crown
xsk -- Death Mask
xui -- Ghost Armor
xea -- Serpentskin Armor
xla -- Demonhide Armor
xtu -- Trellised Armor
xng -- Linked Mail
xcl -- Tigulated Mail
xhn -- Mesh Armor
xrs -- Cuirass
xpl -- Russet Armor
xlt -- Templar Coat
xld -- Sharktooth Armor
xth -- Embossed Plate
xul -- Chaos Armor
xar -- Ornate Armor
xtp -- Mage Plate
xuc -- Defender
xml -- Round Shield
xrg -- Scutum
xit -- Dragon Shield
xow -- Pavise
xts -- Ancient Shield
xlg -- Demonhide Gloves
xvg -- Sharkskin Gloves
xmg -- Heavy Bracers
xtg -- Battle Gauntlets
xhg -- War Gauntlets
xlb -- Demonhide Boots
xvb -- Sharkskin Boots
xmb -- Mesh Boots
xtb -- Battle Boots
xhb -- War Boots
zlb -- Demonhide Sash
zvb -- Sharkskin Belt
zmb -- Mesh Belt
ztb -- Battle Belt
zhb -- War Belt
xh9 -- Grim Helm
xsh -- Grim Shield
xpk -- Barbed Shield
dr1 -- Wolf Head
dr2 -- Hawk Helm
dr3 -- Antlers
dr4 -- Cub Cap
dr5 -- Spirit Mask
ba1 -- Jawbone Cap
ba2 -- Fanged Helm
ba3 -- Horned Helm
ba4 -- Assault Helmet
ba5 -- Avenger Guard
pa1 -- Targe
pa2 -- Rondache
pa3 -- Heraldic Shield
pa4 -- Aerin Shield
pa5 -- Crown Shield
ne1 -- Preserved Head
ne2 -- Zombie Head
ne3 -- Unraveller Head
ne4 -- Gargoyle Head
ne5 -- Demon Head
ci0 -- Circlet
ci1 -- Coronet
ci2 -- Tiara
ci3 -- Diadem
uap -- Shako
ukp -- Hydraskull
ulm -- Armet
uhl -- Giant Conch
uhm -- Spired Helm
urn -- Corona
usk -- Demonhead
uui -- Dusk Shroud
uea -- Wyrmhide
ula -- Scarab Husk
utu -- Wire Fleece
ung -- Diamond Mail
ucl -- Loricated Mail
uhn -- Boneweave
urs -- Great Hauberk
upl -- Balrog Skin
ult -- Hellforge Plate
uld -- Kraken Shell
uth -- Lacquered Plate
uul -- Shadow Plate
uar -- Sacred Armor
utp -- Archon Plate
uuc -- Heater
uml -- Luna
urg -- Hyperion
uit -- Monarch
uow -- Aegis
uts -- Ward
ulg -- Bramble Mitts
uvg -- Vampirebone Gloves
umg -- Vambraces
utg -- Crusader Gauntlets
uhg -- Ogre Gauntlets
ulb -- Wyrmhide Boots
uvb -- Scarabshell Boots
umb -- Boneweave Boots
utb -- Mirrored Boots
uhb -- Myrmidon Greaves
ulc -- Spiderweb Sash
uvc -- Vampirefang Belt
umc -- Mithril Coil
utc -- Troll Belt
uhc -- Colossus Girdle
uh9 -- Bone Visage
ush -- Troll Nest
upk -- Blade Barrier
dr6 -- Alpha Helm
dr7 -- Griffon Headress
dr8 -- Hunter's Guise
dr9 -- Cavernous Covering
dra -- Totemic Mask
ba6 -- Jawbone Visor
ba7 -- Lion Helm
ba8 -- Rage Mask
ba9 -- Savage Helmet
baa -- Slayer Guard
pa6 -- Akaran Targe
pa7 -- Akaran Rondache
pa8 -- Protector Shield
pa9 -- Gilded Shield
paa -- Royal Shield
ne6 -- Mummified Trophy
ne7 -- Fetish Trophy
ne8 -- Sexton Trophy
ne9 -- Cantor Trophy
nea -- Heirophant Trophy
drb -- Blood Spirit
drc -- Sun Spirit
drd -- Earth Spirit
dre -- Grizzly Visor
drf -- Dream Spirit
bab -- Carnage Helm
bac -- Fury Visor
bad -- Destroyer Helm
bae -- Conqueror Crown
baf -- Guardian Crown
pab -- Sacred Targe
pac -- Sacred Rondache
pad -- Kurast Shield
pae -- Zakarum Shield
paf -- Vortex Shield
neb -- Minion Skull
neg -- Hellspawn Skull
ned -- Overseer Skull
nee -- Succubus Skull
nef -- Bloodlord Skull
Ca1 -- Cape
Ca2 -- Mantle
Ca3 -- Cloak
Ca4 -- Assassin Cloak 1A
Ca5 -- Assassin Cloak 1B
Ca6 -- Assassin Cloak 1C
Wp1 -- Wolf Cloth
Wp2 -- Dire Pelt
Wp3 -- Fenris Fur
Gg1 -- Bear Hide
Gg2 -- Sleuth Slough
Gg3 -- Grizzly Gear
Ab1 -- Silent Sneakers
Ab2 -- Combat Kicks
Ab3 -- Bladed Boots
Ab4 -- Assassin Boots 1A
Ab5 -- Assassin Boots 1B
Ab6 -- Assassin Boots 1C
Bp1 -- Blessed Plate
Bp2 -- Hallowed Armor
Bp3 -- Enlightened Plate
Bp4 -- Paladin Armor 1A
Bp5 -- Paladin Armor 1B
Bp6 -- Paladin Armor 1C
Oa1 -- Bark Armor
Oa2 -- Cedar Chest
Oa3 -- Oaken Armor
Vg1 -- Arcanic Touch
Vg2 -- Elemental Outreach
Vg3 -- Vizjerei Vestige
Vg4 -- Sorceress Gloves 1A
Vg5 -- Sorceress Gloves 1B
Vg6 -- Sorceress Gloves 1C
Bb1 -- Strong Belt
Bb2 -- Mighty Belt
Bb3 -- Arreat Belt
Bb4 -- Barbarian Belt 1A
Bb5 -- Barbarian Belt 1A
Bb6 -- Barbarian Belt 1A
Zc1 -- Skovos Circle
Zc2 -- Island Crown
Zc3 -- Sister's Stone
Zc4 -- Amazon Circlet 1A
Zc5 -- Amazon Circlet 1B
Zc6 -- Amazon Circlet 1C
St1 -- Soul Summon Torso 1
St2 -- Soul Summon Torso 2
Pc1 -- Palisade Helm
Pc2 -- Anointed Crown
Pc3 -- Treasured Headdress
Ag1 -- Amazon Gloves 1A
Ag2 -- Amazon Gloves 1B
Ag3 -- Amazon Gloves 1C
Ag4 -- Amazon Gloves 2A
Ag5 -- Amazon Gloves 2B
Ag6 -- Amazon Gloves 2C
Na1 -- Necromancer Armor 1A
Na2 -- Necromancer Armor 1B
Na3 -- Necromancer Armor 1C
Na4 -- Necromancer Armor 2A
Na5 -- Necromancer Armor 2B
Na6 -- Necromancer Armor 2C
Sa1 -- Sorceress Armor 1A
Sa2 -- Sorceress Armor 1B
Sa3 -- Sorceress Armor 1C
Sa4 -- Sorceress Armor 2A
Sa5 -- Sorceress Armor 2B
Sa6 -- Sorceress Armor 2C
St3 -- Soul Summon Torso 3
St4 -- Shadow Plate Ama
St5 -- Shadow Plate Sor
St6 -- Shadow Plate Nec
St7 -- Shadow Plate Pal
St8 -- Shadow Plate Bar
St9 -- Shadow Plate Dru
St0 -- Shadow Plate Ass
D01 -- Demon Armor
D03 -- Demon Cap
D04 -- Demon Monarch
D05 -- Demon Fleece
D08 -- Demon Diadem
D09 -- Demon Coif
D11 -- Demon Mask
D12 -- Demon Boots
D17 -- Demon Skin
D19 -- Demon Visage
D20 -- Demon Spired
D21 -- Demon Corona
D23 -- Demon Sash
D29 -- Demon Gauntlets
D35 -- Demon Boots
D36 -- Demon Belt
D37 -- Demon Luna
D38 -- Demon Aegis
D45 -- Demon Boots


--Weapons--
hax -- Hand Axe
axe -- Axe
2ax -- Double Axe
mpi -- Sickle
wax -- War Axe
lax -- Large Axe
bax -- Broad Axe
btx -- Battle Axe
gax -- Great Axe
gix -- Giant Axe
wnd -- Wand
ywn -- Yew Wand
bwn -- Bone Wand
gwn -- Grim Wand
clb -- Club
scp -- Scepter
gsc -- Grand Scepter
wsp -- War Scepter
spc -- Spiked Club
mac -- Mace
mst -- Morning Star
fla -- Flail
whm -- War Hammer
mau -- Maul
gma -- Great Maul
ssd -- Short Sword
scm -- Scimitar
sbr -- Saber
flc -- Falchion
crs -- Crystal Sword
bsd -- Broad Sword
lsd -- Long Sword
wsd -- War Sword
2hs -- Katana
clm -- Claymore
gis -- Giant Sword
bsw -- Bastard Sword
flb -- Flamberge
gsd -- Great Sword
dgr -- Dagger
dir -- Dirk
kri -- Kriss
bld -- Blade
tkf -- Throwing Knife
tax -- Throwing Axe
bkf -- Balanced Knife
bal -- Balanced Axe
jav -- Javelin
pil -- Pilum
ssp -- Short Spear
glv -- Glaive
tsp -- Throwing Spear
spr -- Spear
tri -- Trident
brn -- Brandistock
spt -- Spetum
pik -- Pike
bar -- Bardiche
vou -- Voulge
scy -- Scythe
pax -- Poleaxe
hal -- Halberd
wsc -- Thresher
sst -- Short Staff
lst -- Long Staff
cst -- Gnarled Staff
bst -- Battle Staff
wst -- War Staff
sbw -- Short Bow
hbw -- Hunter's Bow
lbw -- Long Bow
cbw -- Composite Bow
sbb -- Short Battle Bow
lbb -- Long Battle Bow
swb -- Short War Bow
lwb -- Long War Bow
lxb -- Light Crossbow
mxb -- Crossbow
hxb -- Heavy Crossbow
rxb -- Repeating Crossbow
gps -- Rancid Gas Potion
ops -- Oil Potion
gpm -- Choking Gas Potion
opm -- Exploding Potion
gpl -- Strangling Gas Potion
opl -- Fulminating Potion
d33 -- Decoy Gidbinn
g33 -- The Gidbinn
leg -- Wirt's Leg
hdm -- Horadric Malus
hfh -- Hellforge Hammer
hst -- Horadric Staff
msf -- Staff of Kings
9ha -- Hatchet
9ax -- Cleaver
92a -- Twin Axe
9mp -- Battle Sickle
9wa -- Naga
9la -- Military Axe
9ba -- Bearded Axe
9bt -- Tabar
9ga -- Gothic Axe
9gi -- Ancient Axe
9wn -- Burnt Wand
9yw -- Petrified Wand
9bw -- Tomb Wand
9gw -- Grave Wand
9cl -- Cudgel
9sc -- Rune Scepter
9qs -- Holy Water Sprinkler
9ws -- Divine Scepter
9sp -- Barbed Club
9ma -- Flanged Mace
9mt -- Jagged Star
9fl -- Knout
9wh -- Battle Hammer
9m9 -- War Club
9gm -- Martel de Fer
9ss -- Gladius
9sm -- Cutlass
9sb -- Shamshir
9fc -- Tulwar
9cr -- Dimensional Blade
9bs -- Battle Sword
9ls -- Rune Sword
9wd -- Ancient Sword
92h -- Nihonto
9cm -- Dacian Falx
9gs -- Tusk Sword
9b9 -- Gothic Sword
9fb -- Zweihander
9gd -- Executioner Sword
9dg -- Poignard
9di -- Rondel
9kr -- Cinquedeas
9bl -- Stilleto
9tk -- Battle Dart
9ta -- Francisca
9bk -- War Dart
9b8 -- Hurlbat
9ja -- War Javelin
9pi -- Great Pilum
9s9 -- Simbilan
9gl -- Spiculum
9ts -- Harpoon
9sr -- War Spear
9tr -- Fuscina
9br -- War Fork
9st -- Yari
9p9 -- Lance
9b7 -- Lochaber Axe
9vo -- Bill
9s8 -- Grim Scythe
9pa -- Partizan
9h9 -- Bec-de-Corbin
9wc -- Giant Thresher
8ss -- Jo Staff
8ls -- Quarterstaff
8cs -- Cedar Staff
8bs -- Gothic Staff
8ws -- Rune Staff
8sb -- Edge Bow
8hb -- Razor Bow
8lb -- Cedar Bow
8cb -- Double Bow
8s8 -- Short Siege Bow
8l8 -- Long Siege Bow
8sw -- Rune Bow
8lw -- Gothic Bow
8lx -- Arbalest
8mx -- Siege Crossbow
8hx -- Ballista
8rx -- Chu-Ko-Nu
qf1 -- Khalim's Flail
qf2 -- Khalim's Will
ktr -- Katar
wrb -- Wrist Blade
axf -- Hatchet Hands
ces -- Cestus
clw -- Claws
btl -- Blade Talons
skr -- Scissors Katar
9ar -- Quhab
9wb -- Wrist Spike
9xf -- Fascia
9cs -- Hand Scythe
9lw -- Greater Claws
9tw -- Greater Talons
9qr -- Scissors Quhab
7ar -- Suwayyah
7wb -- Wrist Sword
7xf -- War Fist
7cs -- Battle Cestus
7lw -- Feral Claws
7tw -- Runic Talons
7qr -- Scissors Suwayyah
7ha -- Tomahawk
7ax -- Small Crescent
72a -- Ettin Axe
7mp -- Reaper Sickle
7wa -- Berserker Axe
7la -- Feral Axe
7ba -- Silver-edged Axe
7bt -- Decapitator
7ga -- Champion Axe
7gi -- Glorious Axe
7wn -- Polished Wand
7yw -- Ghost Wand
7bw -- Lich Wand
7gw -- Unearthed Wand
7cl -- Truncheon
7sc -- Mighty Scepter
7qs -- Seraph Rod
7ws -- Caduceus
7sp -- Tyrant Club
7ma -- Reinforced Mace
7mt -- Devil Star
7fl -- Scourge
7wh -- Legendary Mallet
7m7 -- Ogre Maul
7gm -- Thunder Maul
7ss -- Falcata
7sm -- Ataghan
7sb -- Elegant Blade
7fc -- Hydra Edge
7cr -- Phase Blade
7bs -- Conquest Sword
7ls -- Cryptic Sword
7wd -- Mythical Sword
72h -- Shinogi
7cm -- Highland Blade
7gs -- Balrog Blade
7b7 -- Champion Sword
7fb -- Colossal Sword
7gd -- Colossus Blade
7dg -- Bone Knife
7di -- Mithral Point
7kr -- Fanged Knife
7bl -- Legend Spike
7tk -- Flying Knife
7ta -- Flying Axe
7bk -- Winged Knife
7b8 -- Winged Axe
7ja -- Hyperion Javelin
7pi -- Stygian Pilum
7s7 -- Balrog Spear
7gl -- Ghost Glaive
7ts -- Winged Harpoon
7sr -- Hyperion Spear
7tr -- Stygian Pike
7br -- Mancatcher
7st -- Ghost Spear
7p7 -- War Pike
7o7 -- Ogre Axe
7vo -- Colossus Voulge
7s8 -- Reaper Scythe
7pa -- Cryptic Axe
7h7 -- Great Poleaxe
7wc -- Reaper Thresher
6ss -- Walking Stick
6ls -- Stalagmite
6cs -- Elder Staff
6bs -- Shillelagh
6ws -- Archon Staff
6sb -- Spider Bow
6hb -- Blade Bow
6lb -- Shadow Bow
6cb -- Great Bow
6s7 -- Diamond Bow
6l7 -- Crusader Bow
6sw -- Ward Bow
6lw -- Hydra Bow
6lx -- Pellet Bow
6mx -- Gorgon Crossbow
6hx -- Colossus Crossbow
6rx -- Demon Crossbow
ob1 -- Eagle Orb
ob2 -- Sacred Globe
ob3 -- Smoked Sphere
ob4 -- Clasped Orb
ob5 -- Jared's Stone
am1 -- Stag Bow
am2 -- Reflex Bow
am3 -- Maiden Spear
am4 -- Maiden Pike
am5 -- Maiden Javelin
ob6 -- Glowing Orb
ob7 -- Crystalline Globe
ob8 -- Cloudy Sphere
ob9 -- Sparkling Ball
oba -- Swirling Crystal
am6 -- Ashwood Bow
am7 -- Ceremonial Bow
am8 -- Ceremonial Spear
am9 -- Ceremonial Pike
ama -- Ceremonial Javelin
obb -- Heavenly Stone
obc -- Eldritch Orb
obd -- Demon Heart
obe -- Vortex Orb
obf -- Dimensional Shard
amb -- Matriarchal Bow
amc -- Grand Matron Bow
amd -- Matriarchal Spear
ame -- Matriarchal Pike
amf -- Matriarchal Javelin
k01 -- Katana
k02 -- Katana
k03 -- Katana
Ds1 -- Druid Staff 1A
Ds2 -- Druid Staff 1B
Ds3 -- Druid Staff 1C
Ds4 -- Druid Staff 2A
Ds5 -- Druid Staff 2B
Ds6 -- Druid Staff 2C
Pm1 -- Paladin Mace 1A
Pm2 -- Paladin Mace 1B
Pm3 -- Paladin Mace 1C
Bm1 -- Barbarian M Axe 1A
Bm2 -- Barbarian M Axe 1B
Bm3 -- Barbarian M Axe 1C
Bm4 -- Barbarian M Sword 1A
Bm5 -- Barbarian M Sword 1B
Bm6 -- Barbarian M Sword 1C
Bm7 -- Barbarian M Hammer 1A
Bm8 -- Barbarian M Hammer 1B
Bm9 -- Barbarian M Hammer 1C
Bf1 -- Barbarian F Axe 1A
Bf2 -- Barbarian F Axe 1B
Bf3 -- Barbarian F Axe 1C
Bf4 -- Barbarian F Sword 1A
Bf5 -- Barbarian F Sword 1B
Bf6 -- Barbarian F Sword 1C
D00 -- Demon Sword
Ss1 -- Champion Axe SS
Ss2 -- Phase Blade SS
Ss3 -- Shinogi SS
Ss4 -- ShinogiB SS
D02 -- Demon Bow
D13 -- Demon Staff
D14 -- Demon Tomahawk
D15 -- Demon Knife
D16 -- Demon Knife
D24 -- Demon Ettin Axe
D25 -- Demon Cryptic Axe
D26 -- Demon Balrog Spear
D27 -- Demon Javelin
D28 -- Demon Javelin
D30 -- Demon Maul
D31 -- Demon Shard
D33 -- Demon Vorpal Axes
D34 -- Demon Shard
D39 -- Demon Ataghan
D40 -- Demon Sword
D41 -- Demon Thresher
D42 -- Demon Ogre Axe
D43 -- Demon Scythe
D44 -- Demon Staff
Ev9 -- Snowball


--Misc--
elx -- Elixir
hpo -- Healing Potionx
mpo -- Mana Potionx
hpf -- Full Healing Potion
mpf -- Full Mana Potion
vps -- Stamina Potion
yps -- Antidote Potion
rvs -- Rejuvenation Potion
rvl -- Full Rejuvenation Potion
wms -- Thawing Potion
tbk -- Tome of Town Portal
ibk -- Tome of Identify
amu -- Amulet
vip -- Amulet of the Viper
rin -- Ring
gld -- Gold
bks -- Scroll of Inifuss
bkd -- Key to the Cairn Stones
aqv -- Arrows
tch -- Torch
cqv -- Bolts
tsc -- Scroll of Town Portal
isc -- Scroll of Identify
hrt -- Heart
brz -- Brain
jaw -- Jawbone
eyz -- Eye
hrn -- Horn
tal -- Tail
flg -- Flag
fng -- Fang
qll -- Quill
sol -- Soul
scz -- Scalp
spe -- Spleen
key -- Key
luv -- The Black Tower Key
xyz -- Potion of Life
j34 -- A Jade Figurine
g34 -- The Golden Bird
bbb -- Lam Esen's Tome
box -- Horadric Cube
tr1 -- Horadric Scroll
mss -- Mephisto's Soulstone
ass -- Book of Skill
qey -- Khalim's Eye
qhr -- Khalim's Heart
qbr -- Khalim's Brain
ear -- Ear
gcv -- Chipped Amethyst
gfv -- Flawed Amethyst
gsv -- Amethyst
gzv -- Flawless Amethyst
gpv -- Perfect Amethyst
gcy -- Chipped Topaz
gfy -- Flawed Topaz
gsy -- Topaz
gly -- Flawless Topaz
gpy -- Perfect Topaz
gcb -- Chipped Sapphire
gfb -- Flawed Sapphire
gsb -- Sapphire
glb -- Flawless Sapphire
gpb -- Perfect Sapphire
gcg -- Chipped Emerald
gfg -- Flawed Emerald
gsg -- Emerald
glg -- Flawless Emerald
gpg -- Perfect Emerald
gcr -- Chipped Ruby
gfr -- Flawed Ruby
gsr -- Ruby
glr -- Flawless Ruby
gpr -- Perfect Ruby
gcw -- Chipped Diamond
gfw -- Flawed Diamond
gsw -- Diamond
glw -- Flawless Diamond
gpw -- Perfect Diamond
hp1 -- Minor Healing Potion
hp2 -- Light Healing Potion
hp3 -- Healing Potion
hp4 -- Greater Healing Potion
hp5 -- Super Healing Potion
mp1 -- Minor Mana Potion
mp2 -- Light Mana Potion
mp3 -- Mana Potion
mp4 -- Greater Mana Potion
mp5 -- Super Mana Potion
skc -- Chipped Skull
skf -- Flawed Skull
sku -- Skull
skl -- Flawless Skull
skz -- Perfect Skull
 -- Expansion
hrb -- Herb
cm1 -- Small Charm
cm2 -- Large Charm
cm3 -- Grand Charm
rps -- Small Red Potion
rpl -- Large Red Potion
bps -- Small Blue Potion
bpl -- Large Blue Potion
r01 -- El Rune
r02 -- Eld Rune
r03 -- Tir Rune
r04 -- Nef Rune
r05 -- Eth Rune
r06 -- Ith Rune
r07 -- Tal Rune
r08 -- Ral Rune
r09 -- Ort Rune
r10 -- Thul Rune
r11 -- Amn Rune
r12 -- Sol Rune
r13 -- Shael Rune
r14 -- Dol Rune
r15 -- Hel Rune
r16 -- Io Rune
r17 -- Lum Rune
r18 -- Ko Rune
r19 -- Fal Rune
r20 -- Lem Rune
r21 -- Pul Rune
r22 -- Um Rune
r23 -- Mal Rune
r24 -- Ist Rune
r25 -- Gul Rune
r26 -- Vex Rune
r27 -- Ohm Rune
r28 -- Lo Rune
r29 -- Sur Rune
r30 -- Ber Rune
r31 -- Jah Rune
r32 -- Cham Rune
r33 -- Zod Rune
jew -- Jewel
ice -- Malah's Potion
0sc -- Scroll of Knowledge
tr2 -- Scroll of Resistance
pk1 -- Key of Terror
pk2 -- Key of Hate
pk3 -- Key of Destruction
dhn -- Diablo's Horn
bey -- Baal's Eye
mbr -- Mephisto's Brain
toa -- Token of Absolution
tes -- Twisted Essence of Suffering
ceh -- Charged Essense of Hatred
bet -- Burning Essence of Terror
fed -- Festering Essence of Destruction
std -- Standard of Heroes
S01 -- Soul Fragment
Gu1 -- GoldUp
Gu2 -- GoldUp
Gu3 -- GoldUp
Gu4 -- GoldUp
Gu5 -- GoldUpBar
Rgx -- Ramaladni's Gift
Cr1 -- Sparking Crystal
Cr2 -- Burning Crystal
Cr3 -- Frigid Crystal
Cr4 -- Toxic Crystal
Cr5 -- Hardened Crystal
Cr6 -- Shining Crystal
UCR -- Unique Core
Bbp -- Basic Blueprint
Xbp -- Exceptional Blueprint
Ebp -- Elite Blueprint
y08 -- Mythos Log
BoH -- Beacon
y09 -- TreasureChestArmo4
y10 -- TreasureChestArmo3
y11 -- TreasureChestArmo2
y12 -- TreasureChestArmo1
y13 -- TreasureChestWeap4
y14 -- TreasureChestWeap3
y15 -- TreasureChestWeap2
y16 -- TreasureChestWeap1
y17 -- TreasureChestRing4
y18 -- TreasureChestRing3
y19 -- TreasureChestRing2
y20 -- TreasureChestRing1
y21 -- TreasureChestAmul4
y22 -- TreasureChestAmul3
y23 -- TreasureChestAmul2
y24 -- TreasureChestAmul1
y25 -- TreasureChestBowq4
y26 -- TreasureChestBowq3
y27 -- TreasureChestBowq2
y28 -- TreasureChestBowq1
y29 -- TreasureChestRune4
y30 -- TreasureChestRune3
y31 -- TreasureChestRune2
y32 -- TreasureChestRune1
Y20 -- ShopVoucher
EcW -- Enchanted Wood
IN1 -- Ashy Bone
IN2 -- Broken Bone
IN3 -- Soulless Skull
IN4 -- Frail Skull
IN5 -- Nevermelt Ice
IN6 -- Charge Vein
IN7 -- Life Spark
TK0 -- Global Token
TK1 -- Amazon Token
TK2 -- Sorceress Token
TK3 -- Necromancer Token
TK4 -- Paladin Token
TK5 -- Barbarian Token
TK6 -- Druid Token
TK7 -- Assassin Token
Z01 -- Storage Bag
Z02 -- Gem Remover
Z03 -- Gem Remover
Z04 -- Gem Remover
Z05 -- Gem Remover
Z06 -- Gem Remover
Z07 -- Gem Remover
Z08 -- Gem Remover
Z09 -- Rune Remover
Z10 -- Rune Remover
Z11 -- Rune Remover
Z12 -- Rune Remover
Z13 -- Rune Remover
Z14 -- Rune Remover
Z15 -- Rune Remover
Z16 -- Rune Remover
Z17 -- Rune Remover
Z18 -- Rune Remover
Z19 -- Rune Remover
Z20 -- Rune Remover
Z21 -- Rune Remover
Z22 -- Rune Remover
Z23 -- Rune Remover
Z24 -- Rune Remover
Z25 -- Rune Remover
Z26 -- Rune Remover
Z27 -- Rune Remover
Z28 -- Rune Remover
Z29 -- Rune Remover
Z30 -- Rune Remover
Z31 -- Rune Remover
Z32 -- Rune Remover
Z33 -- Rune Remover
Z34 -- Rune Remover
Z35 -- Rune Remover
Z36 -- Rune Remover
Z37 -- Rune Remover
Z38 -- Rune Remover
Z39 -- Rune Remover
Z40 -- Rune Remover
Z41 -- Rune Remover
Z42 -- Key Remover
Z43 -- Key Remover
Z44 -- Key Remover
Z45 -- Organ Remover
Z46 -- Organ Remover
Z47 -- Organ Remover
Z48 -- Rune Converter
Z49 -- Rune Converter
Z50 -- Rune Converter
Z51 -- Rune Converter
GBd -- Gold Bar Downgrader
TK8 -- Soul Forger
Z52 -- Empty Bottle
Z53 -- White Bottle
Z54 -- Black Bottle
Z55 -- Blue Bottle
Z56 -- Red Bottle
Z57 -- Green Bottle
Z58 -- Yellow Bottle
Z59 -- Purple Bottle
Z60 -- Enhance Crystal
Z61 -- Enhance Crystal
Z62 -- Enhance Crystal
Z63 -- Enhance Crystal
Z64 -- Enhance Crystal
Z65 -- Enhance Crystal
Z66 -- Enhance Crystal
Z67 -- Enhance Crystal
Z68 -- Enhance Crystal
Z69 -- Enhance Crystal
Z70 -- Enhance Crystal
Z71 -- Enhance Crystal
Z72 -- Enhance Crystal
Z73 -- Enhance Crystal
Z74 -- Enhance Crystal
Z75 -- Enhance Crystal
Z76 -- Enhance Crystal
Z77 -- Enhance Crystal
Z78 -- Enhance Crystal
Z79 -- Enhance Crystal
Z80 -- Enhance Crystal
Z81 -- Enhance Crystal
Z82 -- Enhance Crystal
Z83 -- Enhance Crystal
Z84 -- Enhance Crystal
Z85 -- Enhance Crystal
Z86 -- Enhance Crystal
Z87 -- Enhance Crystal
Z88 -- Enhance Crystal
Z89 -- Enhance Crystal
Z90 -- Enhance Crystal
Z91 -- Enhance Crystal
Z92 -- Enhance Crystal
Z93 -- Enhance Crystal
Z94 -- Enhance Crystal
Z95 -- Enhance Crystal
Z96 -- Enhance Crystal
Z97 -- Enhance Crystal
Z98 -- Enhance Crystal
Z99 -- Enhance Crystal
z00 -- Enhance Crystal
z01 -- Enhance Crystal
z02 -- Enhance Crystal
z03 -- Enhance Crystal
z04 -- Enhance Crystal
z05 -- Enhance Crystal
z06 -- Enhance Crystal
z07 -- Enhance Crystal
z08 -- Enhance Crystal
z09 -- Enhance Crystal
z10 -- Enhance Crystal
z11 -- Enhance Crystal
z12 -- Enhance Crystal
z13 -- Enhance Crystal
z14 -- Enhance Crystal
z15 -- Enhance Crystal
z16 -- Enhance Crystal
z17 -- Enhance Crystal
z18 -- Enhance Crystal
z19 -- Enhance Crystal Remover
z20 -- Enhance Crystal Remover
z21 -- Enhance Crystal Remover
z22 -- Enhance Crystal Remover
z23 -- Enhance Crystal Remover
z24 -- Enhance Crystal Remover
z25 -- Enhance Crystal Remover
z26 -- Enhance Crystal Remover
z27 -- Enhance Crystal Remover
z28 -- Enhance Crystal Remover
z29 -- Enhance Crystal Remover
z30 -- Enhance Crystal Remover
z31 -- Enhance Crystal Remover
z32 -- Enhance Crystal Remover
z33 -- Enhance Crystal Remover
z34 -- Enhance Crystal Remover
z35 -- Enhance Crystal Remover
z36 -- Enhance Crystal Remover
z37 -- Enhance Crystal Remover
z38 -- Enhance Crystal Remover
z39 -- Enhance Crystal Remover
z40 -- Enhance Crystal Remover
z41 -- Enhance Crystal Remover
z42 -- Enhance Crystal Remover
z43 -- Enhance Crystal Remover
z44 -- Enhance Crystal Remover
z45 -- Enhance Crystal Remover
z46 -- Enhance Crystal Remover
z47 -- Enhance Crystal Remover
z48 -- Enhance Crystal Remover
z49 -- Enhance Crystal Remover
z50 -- Enhance Crystal Remover
z51 -- Enhance Crystal Remover
z52 -- Enhance Crystal Remover
z53 -- Enhance Crystal Remover
z54 -- Enhance Crystal Remover
z55 -- Enhance Crystal Remover
z56 -- Enhance Crystal Remover
z57 -- Enhance Crystal Remover
z58 -- Enhance Crystal Remover
z59 -- Enhance Crystal Remover
z60 -- Enhance Crystal Remover
z61 -- Enhance Crystal Remover
z62 -- Enhance Crystal Remover
z63 -- Enhance Crystal Remover
z64 -- Enhance Crystal Remover
z65 -- Enhance Crystal Remover
z66 -- Enhance Crystal Remover
z67 -- Enhance Crystal Remover
z68 -- Enhance Crystal Remover
z69 -- Enhance Crystal Remover
z70 -- Enhance Crystal Remover
z71 -- Enhance Crystal Remover
z72 -- Enhance Crystal Remover
z73 -- Enhance Crystal Remover
z74 -- Enhance Crystal Remover
z75 -- Enhance Crystal Remover
z76 -- Enhance Crystal Remover
z77 -- Enhance Crystal Remover
y66 -- Starter Package Set
y67 -- Belial's Scroll
a00 -- Blood Contract
a01 -- Blood Contract
a02 -- Blood Contract
a03 -- Blood Contract
a04 -- Blood Contract
a05 -- Blood Contract
a06 -- Scroll of Triumph
a07 -- Blood Contract
a08 -- Blood Contract
a09 -- Blood Contract
a10 -- Trial Charm 1
a11 -- Trial Charm 2
a12 -- Trial Charm 3
a13 -- Trial Charm 4
a14 -- Trial Charm 5
a15 -- Trial Charm 6
a16 -- Trial Charm 7
a17 -- Trial Charm 8
a18 -- Trial Charm 9
a19 -- Trial Charm 10
a20 -- Trial Charm 11
a21 -- Trial Charm 12
a22 -- Trial Charm 13
a23 -- Trial Charm 14
a24 -- Trial Charm 15
a25 -- Trial Charm 16
a26 -- Trial Charm 17
a27 -- Trial Charm 18
a28 -- Trial Charm 19
a29 -- Trial Charm 20
a30 -- Trial Charm 21
a31 -- Trial Charm 22
a32 -- Trial Charm 23
a33 -- Trial Charm 24
a34 -- Trial Charm 25
a35 -- Trial Charm 26
a36 -- Trial Charm 27
a37 -- Trial Charm 28
a38 -- Trial Charm 29
a39 -- Trial Charm 30
a40 -- Trial Charm 31
a41 -- Trial Charm 32
a42 -- Trial Charm 33
a43 -- Trial Charm 34
a44 -- Trial Charm 35
a45 -- Trial Charm 36
a46 -- Trial Charm 37
a47 -- Trial Charm 38
a48 -- Trial Charm 39
a49 -- Trial Charm 40
a50 -- Trial Charm 41
a51 -- Trial Charm 42
a52 -- Trial Charm 43
a53 -- Trial Charm 44
a54 -- Trial Charm 45
a55 -- Trial Charm 46
a56 -- Trial Charm 47
a57 -- Trial Charm 48
a58 -- Trial Charm 49
a59 -- Trial Charm C1
a60 -- Trial Charm C2
a61 -- Trial Charm C3
a62 -- Trial Charm C4
a63 -- Trial Charm C5
a64 -- Trial Charm C6
a65 -- Trial Charm C7
a66 -- Trial Charm Mega
a67 -- Blood Contract
a68 -- Blood Contract
a69 -- Blood Contract
a70 -- Blood Contract
a71 -- Blood Contract
a72 -- Blood Contract
a73 -- Blood Contract
a74 -- Blood Contract
a75 -- Blood Contract
a76 -- Blood Contract
a77 -- Blood Contract
a78 -- Blood Contract
a79 -- Blood Contract
a80 -- Blood Contract
a81 -- Blood Contract
a82 -- Blood Contract
a83 -- Blood Contract
a84 -- Blood Contract
a85 -- Blood Contract
a86 -- Blood Contract
a87 -- Blood Contract
a88 -- Blood Contract
a89 -- Blood Contract
a90 -- Blood Contract
a91 -- Blood Contract
a92 -- Blood Contract
a93 -- Blood Contract
a94 -- Blood Contract
a95 -- Blood Contract
a96 -- Blood Contract
a97 -- Blood Contract
a98 -- Blood Contract
a99 -- Blood Contract
A00 -- Blood Contract
A01 -- Blood Contract
A02 -- Blood Contract
A03 -- Blood Contract
A04 -- Blood Contract
A05 -- Blood Contract
A06 -- Blood Contract
A07 -- Blood Contract
A08 -- Blood Contract
A09 -- Blood Contract
A10 -- Blood Contract
A11 -- Blood Contract
A12 -- Blood Contract
A13 -- Blood Contract
A14 -- Blood Contract
A15 -- Blood Contract
A16 -- Blood Contract
A17 -- Blood Contract
A18 -- Blood Contract
A19 -- Blood Contract
A20 -- Blood Contract
A21 -- Blood Contract
A22 -- Blood Contract
A23 -- Blood Contract
A24 -- Blood Contract
A25 -- Blood Contract
A26 -- Blood Contract
A27 -- Blood Contract
A28 -- Blood Contract
A29 -- Blood Contract
A30 -- Blood Contract
A31 -- Blood Contract
A32 -- Blood Contract
A33 -- Blood Contract
A34 -- Blood Contract
A35 -- Blood Contract
A36 -- Blood Contract
A37 -- Blood Contract
A38 -- Blood Contract
A39 -- Blood Contract
A40 -- Blood Contract
A41 -- Blood Contract
A42 -- Blood Contract
A43 -- Blood Contract
A44 -- Blood Contract
A45 -- Blood Contract
A46 -- Blood Contract
A47 -- Blood Contract
A48 -- Blood Contract
A49 -- Blood Contract
A50 -- Blood Contract
A51 -- Blood Contract
A52 -- Blood Contract
A53 -- Blood Contract
A54 -- Blood Contract
A55 -- Blood Contract
A56 -- Blood Contract
A57 -- Blood Contract
A58 -- Blood Contract
A59 -- Blood Contract
A60 -- Blood Contract
A61 -- Blood Contract
A62 -- Blood Contract
A63 -- Blood Contract
A64 -- Blood Contract
A65 -- Blood Contract
A66 -- Blood Contract
A67 -- Blood Contract
A68 -- Blood Contract
A69 -- Blood Contract
A70 -- Blood Contract
A71 -- Blood Contract
A72 -- Blood Contract
A73 -- Blood Contract
A74 -- Blood Contract
A75 -- Blood Contract
A76 -- Blood Contract
A77 -- Blood Contract
A78 -- Blood Contract
A79 -- Blood Contract
A80 -- Blood Contract
A81 -- Blood Contract
A82 -- Blood Contract
A83 -- Blood Contract
A84 -- Blood Contract
A85 -- Blood Contract
A86 -- Blood Contract
A87 -- Blood Contract
A88 -- Blood Contract
A89 -- Blood Contract
A90 -- Blood Contract
A91 -- Blood Contract
A92 -- Blood Contract
A93 -- Blood Contract
A94 -- Blood Contract
A95 -- Blood Contract
A96 -- Blood Contract
A97 -- Blood Contract
A98 -- Blood Contract
A99 -- Blood Contract
b00 -- Blood Contract
b01 -- Blood Contract
b02 -- Blood Contract
b03 -- Blood Contract
b04 -- Blood Contract
b05 -- Blood Contract
b06 -- Blood Contract
b07 -- Blood Contract
b08 -- Blood Contract
b09 -- Blood Contract
b10 -- Blood Contract
b11 -- Blood Contract
b12 -- Blood Contract
b13 -- Blood Contract
b14 -- Blood Contract
b15 -- Blood Contract
b16 -- Blood Contract
b17 -- Blood Contract
b18 -- Blood Contract
b19 -- Blood Contract
b20 -- Blood Contract
b21 -- Blood Contract
b22 -- Blood Contract
b23 -- Blood Contract
b24 -- Blood Contract
b25 -- Blood Contract
b26 -- Blood Contract
b27 -- Blood Contract
b28 -- Blood Contract
b29 -- Blood Contract
b30 -- Blood Contract
b31 -- Blood Contract
b32 -- Blood Contract
b33 -- Blood Contract
b34 -- Blood Contract
b35 -- Blood Contract
b36 -- Blood Contract
b37 -- Blood Contract
b38 -- Blood Contract
b39 -- Blood Contract
b40 -- Blood Contract
b41 -- Blood Contract
b42 -- Blood Contract
b43 -- Blood Contract
b44 -- Blood Contract
b45 -- Blood Contract
b46 -- Blood Contract
b47 -- Blood Contract
b48 -- Blood Contract
b49 -- Blood Contract
b50 -- Blood Contract
b51 -- Blood Contract
b52 -- Blood Contract
b53 -- Blood Contract
b54 -- Blood Contract
b55 -- Blood Contract
b56 -- Blood Contract
b57 -- Blood Contract
b58 -- Blood Contract
b59 -- Blood Contract
b60 -- Blood Contract
b61 -- Blood Contract
b62 -- Blood Contract
b63 -- Blood Contract
y33 -- GemVein
b64 -- Socket Remover
b65 -- Super Socket Remover
K01 -- TT Ticket
K02 -- TT Ticket
K03 -- TT Ticket
K04 -- TT Ticket
K05 -- TT Ticket
K06 -- TT Ticket
K07 -- TT Ticket
K08 -- TT Ticket
K09 -- TT Ticket
K10 -- TT Ticket
K11 -- TT Ticket
K12 -- TT Ticket
K13 -- TT Ticket
K14 -- TT Ticket
K15 -- TT Ticket
K16 -- TT Ticket
K17 -- TT Ticket
K18 -- TT Ticket
K19 -- TT Ticket
K20 -- TT Ticket
K21 -- TT Ticket
K22 -- TT Ticket
K23 -- TT Ticket
K24 -- TT Ticket
K25 -- TT Ticket
K26 -- TT Ticket
K27 -- TT Ticket
K28 -- TT Ticket
K29 -- TT Ticket
K30 -- TT Ticket
K31 -- TT Ticket
K32 -- TT Ticket
K33 -- TT Ticket
K34 -- TT Ticket
K35 -- TT Ticket
K36 -- TT Ticket
K37 -- TT Ticket
K38 -- TT Ticket
K39 -- TT Ticket
K40 -- TT Ticket
K41 -- TT Ticket
K42 -- TT Ticket
K43 -- TT Ticket
K44 -- TT Ticket
K45 -- TT Ticket
K46 -- TT Ticket
K47 -- TT Ticket
K48 -- TT Ticket
K49 -- TT Ticket
K50 -- TT Ticket
K51 -- TT Ticket
K52 -- TT Ticket
K53 -- TT Ticket
K54 -- TT Ticket
K55 -- TT Ticket
K56 -- TT Ticket
K57 -- TT Ticket
K58 -- TT Ticket
K59 -- TT Ticket
K60 -- TT Ticket
K61 -- TT Ticket
K62 -- TT Ticket
K63 -- TT Ticket
K64 -- TT Ticket
K65 -- TT Ticket
K66 -- TT Ticket
K67 -- TT Ticket
K68 -- TT Ticket
K69 -- TT Ticket
K70 -- TT Ticket
K71 -- TT Ticket
K72 -- TT Ticket
K73 -- TT Ticket
K74 -- TT Ticket
K75 -- TT Ticket
K76 -- TT Ticket
K77 -- TT Ticket
K78 -- TT Ticket
K79 -- TT Ticket
K80 -- TT Ticket
r34 -- Di Rune
r35 -- Ab Rune
L00 -- Set Voucher
L01 -- Set Voucher
L02 -- Set Voucher
L03 -- Set Voucher
L04 -- Set Voucher
L05 -- Set Voucher
L06 -- Set Voucher
L07 -- Set Voucher
L08 -- Set Voucher
L09 -- Set Voucher
L10 -- Set Voucher
L11 -- Set Voucher
L12 -- Set Voucher
L13 -- Set Voucher
L14 -- Set Voucher
L15 -- Set Voucher
L16 -- Set Voucher
L17 -- Set Voucher
L18 -- Set Voucher
L19 -- Set Voucher
L20 -- Set Voucher
L21 -- Set Voucher
L22 -- Set Voucher
L23 -- Set Voucher
L24 -- Set Voucher
L25 -- Set Voucher
L26 -- Set Voucher
L27 -- Set Voucher
L28 -- Set Voucher
L29 -- Set Voucher
L30 -- Set Voucher
L31 -- Set Voucher
L32 -- Set Voucher
L33 -- Set Voucher
L34 -- Set Voucher
L35 -- Set Voucher
L36 -- Set Voucher
L37 -- Set Voucher
L38 -- Set Voucher
L39 -- Set Voucher
L40 -- Set Voucher
L41 -- Set Voucher
L42 -- Set Voucher
L43 -- Set Voucher
L44 -- Set Voucher
L45 -- Set Voucher
L46 -- Set Voucher
L47 -- Set Voucher
L48 -- Set Voucher
L49 -- Set Voucher
L50 -- Set Voucher
L51 -- Set Voucher
L52 -- Set Voucher
L53 -- Set Voucher
L54 -- Set Voucher
L55 -- Set Voucher
L56 -- Set Voucher
L57 -- Set Voucher
L58 -- Set Voucher
L59 -- Set Voucher
L60 -- Set Voucher
L61 -- Set Voucher
L62 -- Set Voucher
L63 -- Set Voucher
L64 -- Set Voucher
L65 -- Set Voucher
L66 -- Set Voucher
L67 -- Set Voucher
L68 -- Set Voucher
L69 -- Set Voucher
L70 -- Set Voucher
L71 -- Set Voucher
L72 -- Set Voucher
L73 -- Set Voucher
L74 -- Set Voucher
L75 -- Set Voucher
L76 -- Set Voucher
L77 -- Set Voucher
L78 -- Set Voucher
L79 -- Set Voucher
L80 -- Set Voucher
L81 -- Set Voucher
L82 -- Set Voucher
L83 -- Set Voucher
L84 -- Set Voucher
L85 -- Set Voucher
L86 -- Set Voucher
L87 -- Set Voucher
L88 -- Set Voucher
L89 -- Set Voucher
L90 -- Set Voucher
L91 -- Set Voucher
L92 -- Set Voucher
L93 -- Set Voucher
L94 -- Set Voucher
L95 -- Set Voucher
L96 -- Set Voucher
L97 -- Set Voucher
L98 -- Set Voucher
L99 -- Set Voucher
C00 -- Flag1
C01 -- Flag2
C02 -- Flag3
C03 -- Flag4
C04 -- Flag5
C05 -- Flag6
C06 -- Flag7
C07 -- Flag8
C08 -- Flag9
C09 -- Flag10
C10 -- Soul1
C11 -- Soul2
C12 -- Soul3
C13 -- Soul4
C14 -- Soul5
C15 -- Soul6
C16 -- Soul7
C17 -- Soul8
C18 -- Soul9
C19 -- Jawbone1
C20 -- Jawbone2
C21 -- Jawbone3
C22 -- Jawbone4
C23 -- Jawbone5
C24 -- Jawbone6
C25 -- Horn1
C26 -- Horn2
C27 -- Horn3
C28 -- Horn4
C29 -- Horn5
C30 -- Tail1
C31 -- Tail2
C32 -- Tail3
C33 -- Tail4
C34 -- Tail5
C35 -- Fang1
C36 -- Fang2
C37 -- Fang3
y01 -- Mythos Log1
y02 -- Mythos Log2
y03 -- Mythos Log3
y04 -- Mythos Log4
y05 -- Mythos Log5
y06 -- Mythos Log6
y07 -- Mythos Log7
Ev1 -- Easter Egg
Ev2 -- Easter Egg
Ev3 -- Easter Egg
Ev4 -- Easter Egg
Ev5 -- Easter Egg
Ev6 -- Easter Egg
Ev7 -- Easter Egg
Ev8 -- Easter Egg
m00 -- Map - Basic
m01 -- Map - Average
m02 -- Map - Superior
m03 -- Map - Basic
m04 -- Map - Average
m05 -- Map - Superior
m06 -- Map - Basic
m07 -- Map - Average
m08 -- Map - Superior
m09 -- Map - Basic
m10 -- Map - Average
m11 -- Map - Superior
m12 -- Map - Basic
m13 -- Map - Average
m14 -- Map - Superior
m15 -- Map - Basic
m16 -- Map - Average
m17 -- Map - Superior
m18 -- Map - Basic
m19 -- Map - Average
m20 -- Map - Superior
m21 -- Map - Basic
m22 -- Map - Average
m23 -- Map - Superior
m24 -- Map - Basic
m25 -- Map - Average
m26 -- Map - Superior
m27 -- Map - Master
m28 -- Surveyor's Scribbles
m29 -- Parchment of Potency
m30 -- Quill of Quality
m31 -- Ink of Insanity
m32 -- Small Charm Clone
m33 -- Large Charm Clone
m34 -- Grand Charm Clone
m35 -- Grand Charm Clone
j00 -- Sacred Jewel
r36 -- Er Rune
z80 -- Rune Converter
z81 -- Rune Converter
z82 -- Rune Remover
z83 -- Rune Remover
z84 -- Rune Remover
D06 -- Demon Ring
D07 -- Demon Ring
D10 -- Demon Large Charm
D18 -- Demon Amulet
D22 -- Demon Amulet
D32 -- Demon Small Charm
m36 -- Small Charm Low
Ev0 -- Broken Gift
m37 -- LB Respec Fixer
y34 -- Mythos LogA1
y35 -- Mythos LogA2
y36 -- Mythos LogA3
y37 -- Mythos LogA4
y38 -- Mythos LogA5
y39 -- Mythos LogA6
y40 -- Mythos LogA7


------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------


Available Stat ID's:

0 -- strength
1 -- energy
2 -- dexterity
3 -- vitality
4 -- statpts
5 -- newskills
6 -- hitpoints
7 -- maxhp
8 -- mana
9 -- maxmana
10 -- stamina
11 -- maxstamina
12 -- level
13 -- experience
14 -- gold
15 -- goldbank
16 -- item_armor_percent
17 -- item_maxdamage_percent
18 -- item_mindamage_percent
19 -- tohit
20 -- toblock
21 -- mindamage
22 -- maxdamage
23 -- secondary_mindamage
24 -- secondary_maxdamage
25 -- damagepercent
26 -- manarecovery
27 -- manarecoverybonus
28 -- staminarecoverybonus
29 -- lastexp
30 -- nextexp
31 -- armorclass
32 -- armorclass_vs_missile
33 -- armorclass_vs_hth
34 -- normal_damage_reduction
35 -- magic_damage_reduction
36 -- damageresist
37 -- magicresist
38 -- maxmagicresist
39 -- fireresist
40 -- maxfireresist
41 -- lightresist
42 -- maxlightresist
43 -- coldresist
44 -- maxcoldresist
45 -- poisonresist
46 -- maxpoisonresist
47 -- damageaura
48 -- firemindam
49 -- firemaxdam
50 -- lightmindam
51 -- lightmaxdam
52 -- magicmindam
53 -- magicmaxdam
54 -- coldmindam
55 -- coldmaxdam
56 -- coldlength
57 -- poisonmindam
58 -- poisonmaxdam
59 -- poisonlength
60 -- lifedrainmindam
61 -- lifedrainmaxdam
62 -- manadrainmindam
63 -- manadrainmaxdam
64 -- stamdrainmindam
65 -- stamdrainmaxdam
66 -- stunlength
67 -- velocitypercent
68 -- attackrate
69 -- other_animrate
70 -- quantity
71 -- value
72 -- durability
73 -- maxdurability
74 -- hpregen
75 -- item_maxdurability_percent
76 -- item_maxhp_percent
77 -- item_maxmana_percent
78 -- item_attackertakesdamage
79 -- item_goldbonus
80 -- item_magicbonus
81 -- item_knockback
82 -- item_timeduration
83 -- item_addclassskills
84 -- unsentparam1
85 -- item_addexperience
86 -- item_healafterkill
87 -- item_reducedprices
88 -- item_doubleherbduration
89 -- item_lightradius
90 -- item_lightcolor
91 -- item_req_percent
92 -- item_levelreq
93 -- item_fasterattackrate
94 -- item_levelreqpct
95 -- lastblockframe
96 -- item_fastermovevelocity
97 -- item_nonclassskill
98 -- state
99 -- item_fastergethitrate
100 -- monster_playercount
101 -- skill_poison_override_length
102 -- item_fasterblockrate
103 -- skill_bypass_undead
104 -- skill_bypass_demons
105 -- item_fastercastrate
106 -- skill_bypass_beasts
107 -- item_singleskill
108 -- item_restinpeace
109 -- curse_resistance
110 -- item_poisonlengthresist
111 -- item_normaldamage
112 -- item_howl
113 -- item_stupidity
114 -- item_damagetomana
115 -- item_ignoretargetac
116 -- item_fractionaltargetac
117 -- item_preventheal
118 -- item_halffreezeduration
119 -- item_tohit_percent
120 -- item_damagetargetac
121 -- item_demondamage_percent
122 -- item_undeaddamage_percent
123 -- item_demon_tohit
124 -- item_undead_tohit
125 -- item_throwable
126 -- item_elemskill
127 -- item_allskills
128 -- item_attackertakeslightdamage
129 -- ironmaiden_level
130 -- lifetap_level
131 -- thorns_percent
132 -- bonearmor
133 -- bonearmormax
134 -- item_freeze
135 -- item_openwounds
136 -- item_crushingblow
137 -- item_kickdamage
138 -- item_manaafterkill
139 -- item_healafterdemonkill
140 -- item_extrablood
141 -- item_deadlystrike
142 -- item_absorbfire_percent
143 -- item_absorbfire
144 -- item_absorblight_percent
145 -- item_absorblight
146 -- item_absorbmagic_percent
147 -- item_absorbmagic
148 -- item_absorbcold_percent
149 -- item_absorbcold
150 -- item_slow
151 -- item_aura
152 -- item_indesctructible
153 -- item_cannotbefrozen
154 -- item_staminadrainpct
155 -- item_reanimate
156 -- item_pierce
157 -- item_magicarrow
158 -- item_explosivearrow
159 -- item_throw_mindamage
160 -- item_throw_maxdamage
161 -- skill_handofathena
162 -- skill_staminapercent
163 -- skill_passive_staminapercent
164 -- skill_concentration
165 -- skill_enchant
166 -- skill_pierce
167 -- skill_conviction
168 -- skill_chillingarmor
169 -- skill_frenzy
170 -- skill_decrepify
171 -- skill_armor_percent
172 -- alignment
173 -- target0
174 -- target1
175 -- goldlost
176 -- conversion_level
177 -- conversion_maxhp
178 -- unit_dooverlay
179 -- attack_vs_montype
180 -- damage_vs_montype
181 -- fade
182 -- armor_override_percent
183 -- killtrack
184 -- deathtrack
185 -- bonus_mindamage
186 -- bonus_maxdamage
187 -- item_pierce_cold_immunity
188 -- item_addskill_tab
189 -- item_pierce_fire_immunity
190 -- item_pierce_light_immunity
191 -- item_pierce_poison_immunity
192 -- item_pierce_damage_immunity
193 -- item_pierce_magic_immunity
194 -- item_numsockets
195 -- item_skillonattack
196 -- item_skillonkill
197 -- item_skillondeath
198 -- item_skillonhit
199 -- item_skillonlevelup
200 -- item_charge_noconsume
201 -- item_skillongethit
202 -- modifierlist_castid
203 -- unused203
204 -- item_charged_skill
205 -- item_noconsume
206 -- passive_mastery_noconsume
207 -- passive_mastery_replenish_oncrit
208 -- Item_Enhanced
209 -- pali_killtrack
210 -- unused210
211 -- unused211
212 -- passive_mastery_gethit_rate
213 -- passive_mastery_attack_speed
214 -- item_armor_perlevel
215 -- item_armorpercent_perlevel
216 -- item_hp_perlevel
217 -- item_mana_perlevel
218 -- item_maxdamage_perlevel
219 -- item_maxdamage_percent_perlevel
220 -- item_strength_perlevel
221 -- item_dexterity_perlevel
222 -- item_energy_perlevel
223 -- item_vitality_perlevel
224 -- item_tohit_perlevel
225 -- item_tohitpercent_perlevel
226 -- item_cold_damagemax_perlevel
227 -- item_fire_damagemax_perlevel
228 -- item_ltng_damagemax_perlevel
229 -- item_pois_damagemax_perlevel
230 -- item_resist_cold_perlevel
231 -- item_resist_fire_perlevel
232 -- item_resist_ltng_perlevel
233 -- item_resist_pois_perlevel
234 -- item_absorb_cold_perlevel
235 -- item_absorb_fire_perlevel
236 -- item_absorb_ltng_perlevel
237 -- item_absorb_mag_perlevel
238 -- item_thorns_perlevel
239 -- item_find_gold_perlevel
240 -- item_find_magic_perlevel
241 -- item_regenstamina_perlevel
242 -- item_stamina_perlevel
243 -- item_damage_demon_perlevel
244 -- item_damage_undead_perlevel
245 -- item_tohit_demon_perlevel
246 -- item_tohit_undead_perlevel
247 -- item_crushingblow_perlevel
248 -- item_openwounds_perlevel
249 -- item_kick_damage_perlevel
250 -- item_deadlystrike_perlevel
251 -- item_find_gems_perlevel
252 -- item_replenish_durability
253 -- item_replenish_quantity
254 -- item_extra_stack
255 -- item_find_item
256 -- item_openwounds_perstr
257 -- item_deadlystrike_perdex
258 -- item_deadlystrike_perstr
259 -- item_armor_perstr
260 -- item_fastercastrate_perenr
261 -- item_crushingblow_perstr
262 -- item_strength_percent
263 -- item_dexterity_percent
264 -- item_vitality_percent
265 -- item_energy_percent
266 -- item_splash
267 -- item_skillonmisshit
268 -- skill_freezesyn
269 -- skill_chillsyn
270 -- cheatcheck
271 -- item_attackertakesfiredamage
272 -- dont_use_me_272
273 -- dont_use_me_273
274 -- mana_per_gethit
275 -- life_per_gethit
276 -- ex_missiles
277 -- item_attackertakescolddamage
278 -- ex_attacks
279 -- map_quality
280 -- curse_resistance_perenr
281 -- sum_dmg_perc
282 -- sum_speed
283 -- sum_ex
284 -- sum_hp_perc
285 -- sum_cr
286 -- item_cooldownreduction
287 -- item_pierce_perdex
288 -- skill_bonesyn
289 -- skill_rage
290 -- skill_focus
291 -- GB_Total
292 -- RB_Low
293 -- RB_Mid
294 -- RB_High
295 -- KB_Terror
296 -- KB_Hate
297 -- KB_Dest
298 -- OB_Diablo
299 -- OB_Baal
300 -- OB_Meph
301 -- unused301
302 -- relic_tracker
303 -- oskill_display
151 -- aura_display
305 -- item_pierce_cold
306 -- item_pierce_fire
307 -- item_pierce_ltng
308 -- item_pierce_pois
309 -- item_damage_vs_monster
310 -- item_damage_percent_vs_monster
311 -- item_tohit_vs_monster
312 -- item_tohit_percent_vs_monster
313 -- item_ac_vs_monster
314 -- item_ac_percent_vs_monster
315 -- firelength
316 -- burningmin
317 -- burningmax
318 -- progressive_damage
319 -- progressive_steal
320 -- progressive_other
321 -- progressive_fire
322 -- progressive_cold
323 -- progressive_lightning
324 -- item_extra_charges
325 -- progressive_tohit
326 -- poison_count
327 -- damage_framerate
328 -- pierce_idx
329 -- passive_fire_mastery
330 -- passive_ltng_mastery
331 -- passive_cold_mastery
332 -- passive_pois_mastery
333 -- passive_fire_pierce
334 -- passive_ltng_pierce
335 -- passive_cold_pierce
336 -- passive_pois_pierce
337 -- passive_critical_strike
338 -- passive_dodge
339 -- passive_avoid
340 -- passive_evade
341 -- passive_warmth
342 -- passive_mastery_melee_th
343 -- passive_mastery_melee_dmg
344 -- passive_mastery_melee_crit
345 -- passive_mastery_throw_th
346 -- passive_mastery_throw_dmg
347 -- passive_mastery_throw_crit
348 -- passive_weaponblock
349 -- passive_summon_resist
350 -- modifierlist_skill
351 -- modifierlist_level
352 -- last_sent_hp_pct
353 -- source_unit_type
354 -- source_unit_id
355 -- shortparam1
356 -- questitemdifficulty
357 -- passive_mag_mastery
358 -- passive_mag_pierce
359 -- skill_cooldown
360 -- skill_missile_damage_scale
361 -- CD_White
362 -- CD_Black
363 -- CD_Blue
364 -- CD_Red
365 -- CD_Green
366 -- CD_Yellow
367 -- CD_Purple
368 -- CD_Tracker
369 -- score_tracker
370 -- Soul_Ama
371 -- Soul_Sor
372 -- Soul_Nec
373 -- Soul_Pal
374 -- Soul_Bar
375 -- Soul_Dru
376 -- Soul_Ass
377 -- RW_Tracker
378 -- Celestial_Aid
379 -- boneadded
380 -- vitality_hidden
381 -- hsbonus
382 -- item_slow_perdex
383 -- chest_treasure
384 -- enhance_low
385 -- enhance_high
386 -- ucore_tracker
387 -- portals_unlock
388 -- smite
389 -- kill_dummy
390 -- kill_display
391 -- item_killregister
392 -- death_dummy
393 -- death_display
394 -- item_deathregister
395 -- sorceress
396 -- mana_per_hit
397 -- life_per_hit
398 -- body_level
399 -- unused399
400 -- incant_duration
401 -- incant_radius
402 -- demontemper_check
403 -- item_ravendamage
404 -- item_maulsteal
405 -- item_feralsteal
406 -- Soul_Tracker
407 -- item_fireskill
408 -- item_lightskill
409 -- item_coldskill
410 -- item_poisskill
411 -- item_magskill
412 -- Soul_Level
413 -- item_openwounds_perdex
414 -- Weight_Tracker
415 -- Weight_Capacity
416 -- Weight_CapacityBase
417 -- hitpoints_hidden
418 -- fireball_radius
419 -- JB_Rare
420 -- PB_Rejuv
421 -- KillBonus
422 -- KillBonus_Dummy
423 -- KillBonus_Display
424 -- item_fasterattackrate_perstr
425 -- item_tohit_perenr
426 -- item_fasterattackrate_perenr
427 -- buff_length_modifier
428 -- skel_commander
429 -- Weight_Dummy
430 -- Weight_Display
431 -- Freeze_HalfToFull
432 -- map_e_defper
433 -- map_e_hpper
434 -- map_e_level
435 -- map_e_dmgres
436 -- map_e_magres
437 -- map_e_fireres
438 -- map_e_lightres
439 -- map_e_coldres
440 -- map_e_psnres
441 -- map_e_speeda
442 -- map_e_speedm
443 -- map_e_cnbf
444 -- map_e_allskills
445 -- map_e_crush
446 -- map_p_defper
447 -- map_p_hpper
448 -- map_p_dmgres
449 -- map_p_magres
450 -- map_p_fireres
451 -- map_p_lightres
452 -- map_p_coldres
453 -- map_p_psnres
454 -- map_p_speedm
455 -- map_p_speeda
456 -- map_p_speedc
457 -- map_p_cnbf
458 -- map_p_demon
459 -- map_p_undead
460 -- map_p_maek
461 -- map_p_haek
462 -- map_p_gold
463 -- map_p_magic
464 -- map_p_exp
465 -- map_p_itd
466 -- map_p_cdr
467 -- map_p_crush
468 -- item_reanimate_display
469 -- item_skillonhit_noctc
470 -- item_skillonhit_hidden
471 -- item_skillongethit_hidden
472 -- item_numsockets_percent
473 -- energy_hidden
474 -- RW_Tier
475 -- RW_TierDisplay
476 -- RB_Ultra
477 -- item_skillonhit_sweeping
478 -- item_skillonhit_cleaving
479 -- item_skillonhit_piercing
480 -- item_skillonmondeath
481 -- xp_hidden
482 -- LB_Unlock
483 -- LB_One
484 -- LB_Two
485 -- LB_Three

]] --
