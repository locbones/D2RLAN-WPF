-- Clear any cached version and reload the config fresh
package.loaded["lootfilter_config"] = nil
local config = dofile("lootfilter_config.lua")
local version = "1.0.0"
local mod = "RMD"

-- Mapping tables for user-convenience
local qualityMap = { [1] = "Inferior", [2] = "Normal", [3] = "Superior", [4] = "Magic", [5] = "Set", [6] = "Rare", [7] =
"Unique" }
local rarityMap = { [0] = "Normal", [1] = "Exceptional", [2] = "Elite" }
local difficultyMap = { [0] = "Normal", [1] = "Nightmare", [2] = "Hell" }
local locationMap = { onplayer = 0, equipped = 1, onground = 3 }
local colorMap = { white = "ÿc0", red = "ÿc1", green = "ÿc2", blue = "ÿc3", gold = "ÿc4", grey = "ÿc5", gray = "ÿc5", black =
"ÿc6", tan = "ÿc7", orange = "ÿc8", yellow = "ÿc9", purple = "ÿc;", ["dark green"] = "ÿcA", turquoise = "ÿcN", pink =
"ÿcO" }

RMDAreaIDs = {
    -- Act 1
    ["Rogue Encampment"] = 1,
    ["Blood Moor"] = 2,
    ["Cold Plains"] = 3,
    ["Stony Field"] = 4,
    ["Dark Wood"] = 5,
    ["Black Marsh"] = 6,
    ["Tamoe Highland"] = 7,
    ["Den Of Evil"] = 8,
    ["Cave Level 1"] = 9,
    ["Underground Passage Level 1"] = 10,
    ["Hole Level 1"] = 11,
    ["Pit Level 1"] = 12,
    ["Cave Level 2"] = 13,
    ["Underground Passage Level 2"] = 14,
    ["Hole Level 2"] = 15,
    ["Pit Level 2"] = 16,
    ["Burial Grounds"] = 17,
    ["Crypt"] = 18,
    ["Mausoleum"] = 19,
    ["Forgotten Tower"] = 20,
    ["Tower Cellar Level 1"] = 21,
    ["Tower Cellar Level 5"] = 25,
    ["Monastery Gate"] = 26,
    ["Outer Cloister"] = 27,
    ["Barracks"] = 28,
    ["Jail Level 3"] = 31,
    ["Inner Cloister"] = 32,
    ["Cathedral"] = 33,
    ["Catacombs Level 2"] = 35,
    ["Catacombs Level 4"] = 37,
    ["Tristram"] = 38,
    ["Moo Moo Farm"] = 39,
    -- Act 2
    ["Lut Gholein"] = 40,
    ["Rocky Waste"] = 41,
    ["Dry Hills"] = 42,
    ["Far Oasis"] = 43,
    ["Lost City"] = 44,
    ["Valley Of Snakes"] = 45,
    ["Canyon Of The Magi"] = 46,
    ["A2 Sewers Level 1"] = 47,
    ["A2 Sewers Level 2"] = 48,
    ["A2 Sewers Level 3"] = 49,
    ["Palace Cellar Level 3"] = 54,
    ["Stony Tomb Level 1"] = 55,
    ["Halls Of The Dead Level 1"] = 56,
    ["Halls Of The Dead Level 2"] = 57,
    ["Claw Viper Temple Level 1"] = 58,
    ["Stony Tomb Level 2"] = 59,
    ["Halls Of The Dead Level 3"] = 60,
    ["Claw Viper Temple Level 2"] = 61,
    ["Maggot Lair Level 3"] = 64,
    ["Ancient Tunnels"] = 65,
    ["Tal Rashas Tomb level 1"] = 66,
    ["Tal Rashas Tomb level 2"] = 67,
    ["Tal Rashas Tomb level 3"] = 68,
    ["Tal Rashas Tomb level 4"] = 69,
    ["Tal Rashas Tomb level 5"] = 70,
    ["Tal Rashas Tomb level 6"] = 71,
    ["Tal Rashas Tomb level 7"] = 72,
    ["Duriels Lair"] = 73,
    ["Arcane Sanctuary"] = 74,
    -- Act 3
    ["Kurast Docktown"] = 75,
    ["Spider Forest"] = 76,
    ["Great Marsh"] = 77,
    ["Flayer Jungle"] = 78,
    ["Lower Kurast"] = 79,
    ["Kurast Bazaar"] = 80,
    ["Upper Kurast"] = 81,
    ["Kurast Causeway"] = 82,
    ["Travincal"] = 83,
    ["Spider Cave"] = 84,
    ["Spider Cavern"] = 85,
    ["Swampy Pit Level 1"] = 86,
    ["Swampy Pit Level 2"] = 87,
    ["Flayer Dungeon Level 1"] = 88,
    ["Flayer Dungeon Level 2"] = 89,
    ["Swampy Pit Level 3"] = 90,
    ["Flayer Dungeon Level 3"] = 91,
    ["A3 Sewers Level 1"] = 92,
    ["A3 Sewers Level 2"] = 93,
    ["Ruined Temple"] = 94,
    ["Disused Fane"] = 95,
    ["Forgotten Reliquary"] = 96,
    ["Forgotten Temple"] = 97,
    ["Ruined Fane"] = 98,
    ["Disused Reliquary"] = 99,
    ["Durance Of Hate Level 1"] = 100,
    ["Durance Of Hate Level 2"] = 101,
    ["Durance Of Hate Level 3"] = 102,
    -- Act 4
    ["The Pandemonium Fortress"] = 103,
    ["Outer Steppes"] = 104,
    ["Plains Of Despair"] = 105,
    ["City Of The Damned"] = 106,
    ["River Of Flame"] = 107,
    ["Chaos Sanctuary"] = 108,
    -- Act 5
    ["Harrogath"] = 109,
    ["Bloody Foothills"] = 110,
    ["Frigid Highlands"] = 111,
    ["Arreat Plateau"] = 112,
    ["Crystalized Passage"] = 113,
    ["Frozen River"] = 114,
    ["Glacial Trail"] = 115,
    ["Drifter Cavern"] = 116,
    ["Frozen Tundra"] = 117,
    ["Ancients Way"] = 118,
    ["Icy Cellar"] = 119,
    ["Arreat Summit"] = 120,
    ["Nihlathaks Temple"] = 121,
    ["Halls Of Anguish"] = 122,
    ["Halls Of Pain"] = 123,
    ["Halls Of Vaught"] = 124,
    ["Abaddon"] = 125,
    ["Pit Of Acheron"] = 126,
    ["Infernal Pit"] = 127,
    ["The Worldstone Keep Level 1"] = 128,
    ["The Worldstone Keep Level 2"] = 129,
    ["The Worldstone Keep Level 3"] = 130,
    ["Throne Of Destruction"] = 131,
    ["The Worldstone Chamber"] = 132,
    -- Act X
    ["Matrons Den"] = 133,
    ["Fogotten Sands"] = 134,
    ["Furnace of Pain"] = 135,
    ["Tristram2"] = 136
}

AreaIDs = {
    -- Act 1
    ["Rogue Encampment"] = 1,
    ["Blood Moor"] = 2,
    ["Cold Plains"] = 3,
    ["Stony Field"] = 4,
    ["Dark Wood"] = 5,
    ["Black Marsh"] = 6,
    ["Tamoe Highland"] = 7,
    ["Den Of Evil"] = 8,
    ["Cave Level 1"] = 9,
    ["Underground Passage Level 1"] = 10,
    ["Hole Level 1"] = 11,
    ["Pit Level 1"] = 12,
    ["Cave Level 2"] = 13,
    ["Underground Passage Level 2"] = 14,
    ["Hole Level 2"] = 15,
    ["Pit Level 2"] = 16,
    ["Burial Grounds"] = 17,
    ["Crypt"] = 18,
    ["Mausoleum"] = 19,
    ["Forgotten Tower"] = 20,
    ["Tower Cellar Level 1"] = 21,
    ["Tower Cellar Level 2"] = 22,
    ["Tower Cellar Level 3"] = 23,
    ["Tower Cellar Level 4"] = 24,
    ["Tower Cellar Level 5"] = 25,
    ["Monastery Gate"] = 26,
    ["Outer Cloister"] = 27,
    ["Barracks"] = 28,
    ["Jail Level 1"] = 29,
    ["Jail Level 2"] = 30,
    ["Jail Level 3"] = 31,
    ["Inner Cloister"] = 32,
    ["Cathedral"] = 33,
    ["Catacombs Level 1"] = 34,
    ["Catacombs Level 2"] = 35,
    ["Catacombs Level 3"] = 36,
    ["Catacombs Level 4"] = 37,
    ["Tristram"] = 38,
    ["Moo Moo Farm"] = 39,
    -- Act 2
    ["Lut Gholein"] = 40,
    ["Rocky Waste"] = 41,
    ["Dry Hills"] = 42,
    ["Far Oasis"] = 43,
    ["Lost City"] = 44,
    ["Valley Of Snakes"] = 45,
    ["Canyon Of The Magi"] = 46,
    ["A2 Sewers Level 1"] = 47,
    ["A2 Sewers Level 2"] = 48,
    ["A2 Sewers Level 3"] = 49,
    ["Harem Level 1"] = 50,
    ["Harem Level 2"] = 51,
    ["Palace Cellar Level 1"] = 52,
    ["Palace Cellar Level 2"] = 53,
    ["Palace Cellar Level 3"] = 54,
    ["Stony Tomb Level 1"] = 55,
    ["Halls Of The Dead Level 1"] = 56,
    ["Halls Of The Dead Level 2"] = 57,
    ["Claw Viper Temple Level 1"] = 58,
    ["Stony Tomb Level 2"] = 59,
    ["Halls Of The Dead Level 3"] = 60,
    ["Claw Viper Temple Level 2"] = 61,
    ["Maggot Lair Level 1"] = 62,
    ["Maggot Lair Level 2"] = 63,
    ["Maggot Lair Level 3"] = 64,
    ["Ancient Tunnels"] = 65,
    ["Tal Rashas Tomb level 1"] = 66,
    ["Tal Rashas Tomb level 2"] = 67,
    ["Tal Rashas Tomb level 3"] = 68,
    ["Tal Rashas Tomb level 4"] = 69,
    ["Tal Rashas Tomb level 5"] = 70,
    ["Tal Rashas Tomb level 6"] = 71,
    ["Tal Rashas Tomb level 7"] = 72,
    ["Duriels Lair"] = 73,
    -- Act 3
    ["Arcane Sanctuary"] = 74,
    ["Kurast Docktown"] = 75,
    ["Spider Forest"] = 76,
    ["Great Marsh"] = 77,
    ["Flayer Jungle"] = 78,
    ["Lower Kurast"] = 79,
    ["Kurast Bazaar"] = 80,
    ["Upper Kurast"] = 81,
    ["Kurast Causeway"] = 82,
    ["Travincal"] = 83,
    ["Spider Cave"] = 84,
    ["Spider Cavern"] = 85,
    ["Swampy Pit Level 1"] = 86,
    ["Swampy Pit Level 2"] = 87,
    ["Flayer Dungeon Level 1"] = 88,
    ["Flayer Dungeon Level 2"] = 89,
    ["Swampy Pit Level 3"] = 90,
    ["Flayer Dungeon Level 3"] = 91,
    ["A3 Sewers Level 1"] = 92,
    ["A3 Sewers Level 2"] = 93,
    ["Ruined Temple"] = 94,
    ["Disused Fane"] = 95,
    ["Forgotten Reliquary"] = 96,
    ["Forgotten Temple"] = 97,
    ["Ruined Fane"] = 98,
    ["Disused Reliquary"] = 99,
    ["Durance Of Hate Level 1"] = 100,
    ["Durance Of Hate Level 2"] = 101,
    ["Durance Of Hate Level 3"] = 102,
    -- Act 4
    ["The Pandemonium Fortress"] = 103,
    ["Outer Steppes"] = 104,
    ["Plains Of Despair"] = 105,
    ["City Of The Damned"] = 106,
    ["River Of Flame"] = 107,
    ["Chaos Sanctuary"] = 108,
    -- Act 5
    ["Harrogath"] = 109,
    ["Bloody Foothills"] = 110,
    ["Frigid Highlands"] = 111,
    ["Arreat Plateau"] = 112,
    ["Crystalized Passage"] = 113,
    ["Frozen River"] = 114,
    ["Glacial Trail"] = 115,
    ["Drifter Cavern"] = 116,
    ["Frozen Tundra"] = 117,
    ["Ancients Way"] = 118,
    ["Icy Cellar"] = 119,
    ["Arreat Summit"] = 120,
    ["Nihlathaks Temple"] = 121,
    ["Halls Of Anguish"] = 122,
    ["Halls Of Pain"] = 123,
    ["Halls Of Vaught"] = 124,
    ["Abaddon"] = 125,
    ["Pit Of Acheron"] = 126,
    ["Infernal Pit"] = 127,
    ["The Worldstone Keep Level 1"] = 128,
    ["The Worldstone Keep Level 2"] = 129,
    ["The Worldstone Keep Level 3"] = 130,
    ["Throne Of Destruction"] = 131,
    ["The Worldstone Chamber"] = 132,
    -- Act X
    ["Matrons Den"] = 133,
    ["Fogotten Sands"] = 134,
    ["Furnace of Pain"] = 135,
    ["Tristram2"] = 136
}

-- Match Item Codes
local function code_matches(rule, code)
    if rule.codes == "allitems" then -- Special rule: match all items unless explicitly excluded
        return not (HideAllItems and HideAllItems[code])
    end

    if rule.code then
        return rule.code == code
    elseif rule.codes then
        for _, c in ipairs(rule.codes) do
            if c == code then return true end
        end
    end

    return false
end

-- Retrieve output values
local function replace_placeholders(template, Item, Me)
    if not template then return "" end
    local result = template

    result = result:gsub("{code}", Item.Txt.Code or "")
    result = result:gsub("{index}", Item.Name or "")
    result = result:gsub("{rarity}", rarityMap[Item.Rarity] or tostring(Item.Rarity or ""))
    result = result:gsub("{quality}", qualityMap[Item.Data.Quality] or tostring(Item.Data.Quality or ""))
    result = result:gsub("{ethereal}", tostring(Item.Data.IsEthereal or false))
    result = result:gsub("{sockets}", tostring(Item:Stat(194) or ""))

    result = result:gsub("{stat=%((%d+)%)}", function(statIndex)
        local val = Item:Stat(tonumber(statIndex))
        return tostring(val or "")
    end)

    result = result:gsub("{pstat=%((%d+)%)}", function(statIndex)
        local val = Me:Stat(tonumber(statIndex))
        return tostring(val or "")
    end)

    return result
end

-- Apply Color Codes
local function applyColorTags(text)
    return text:gsub("{(.-)}", function(color)
        local code = colorMap[color:lower()]
        if code then
            return code
        else
            return "{" .. color .. "}"
        end
    end)
end

-- Main filter logic
function ApplyFilter(Me, Item, Result)
    local code = Item.Txt.Code
    local quality = Item.Data.Quality
    local rarity = Item.Rarity
    local index = Item.Data.FileIndex
    local location = Item.Mode
    local area = Item.Area
    local difficulty = Me.Difficulty

    print(tostring(difficultyMap[difficulty]))

    -- Dummy Set to hide items
    local HideAllItems = SSet(
    )

    for _, rule in ipairs(config.rules) do
        local expectedLocation = rule.location and locationMap[rule.location] or 3 -- default to 3
        if code_matches(rule, code)
            and location == expectedLocation
            and (not rule.quality or (function()
                local q = rule.quality
                local op, val = "==", 0

                if type(q) == "string" then
                    val = tonumber(q:match("%d+")) or 0
                    if q:find("%+") then
                        op = ">="
                    elseif q:find("%-") then
                        op = "<="
                    else
                        op = "=="
                    end
                elseif type(q) == "number" then
                    val = q
                    op = "=="
                else
                    return false
                end

                if op == "==" then
                    return quality == val
                elseif op == ">=" then
                    return quality >= val
                elseif op == "<=" then
                    return quality <= val
                end
            end)())
            and (not rule.rarity or (function()
                local r = rule.rarity
                local op, val = "==", 0

                if type(r) == "string" then
                    val = tonumber(r:match("%d+")) or 0
                    if r:find("%+") then
                        op = ">="
                    elseif r:find("%-") then
                        op = "<="
                    else
                        op = "=="
                    end
                elseif type(r) == "number" then
                    val = r
                    op = "=="
                else
                    return false
                end

                if op == "==" then
                    return rarity == val
                elseif op == ">=" then
                    return rarity >= val
                elseif op == "<=" then
                    return rarity <= val
                end
            end)())

            and (not rule.index or rule.index == index)
            and (not rule.difficulty or (function()
                local diffVal = difficulty -- assumed to be number (0 = Normal, 1 = Nightmare, etc.)
                local passed = false

                if type(rule.difficulty) == "string" then
                    if rule.difficulty:find(",") then
                        for part in rule.difficulty:gmatch("[^,]+") do
                            local val = tonumber(part)
                            if not val then
                                for id, name in pairs(difficultyMap) do
                                    if name:lower() == part:lower() then
                                        val = id
                                        break
                                    end
                                end
                            end
                            if val ~= nil and val == diffVal then
                                passed = true
                                break
                            end
                        end
                    else
                        local val = tonumber(rule.difficulty:match("%d+")) or 0
                        local op = "=="
                        if rule.difficulty:find("%+") then
                            op = ">="
                        elseif rule.difficulty:find("%-") then
                            op = "<="
                        end
                        if op == "==" then
                            passed = (diffVal == val)
                        elseif op == ">=" then
                            passed = (diffVal >= val)
                        elseif op == "<=" then
                            passed = (diffVal <= val)
                        end
                    end
                elseif type(rule.difficulty) == "number" then
                    passed = (diffVal == rule.difficulty)
                else
                    return false
                end

                return passed
            end)())

            and (not rule.area or (mod == "RMD" and RMDAreaIDs[rule.area] or AreaIDs[rule.area]) == area)
            and (rule.ethereal == nil or rule.ethereal == Item.Data.IsEthereal) then
            local match = true

            -- Stat checks
            if rule.stat then
                local stats = type(rule.stat) == "table" and (rule.stat.index and { rule.stat } or rule.stat) or {}
                for _, stat in ipairs(stats) do
                    local statVal = stat.param and Item:Stat(stat.index, stat.param) or Item:Stat(stat.index)
                    local op = stat.op
                    local val = stat.value

                    -- Multiply value(s) by 256 for special stat indexes 6-9
                    if stat.index >= 6 and stat.index <= 9 then
                        if type(val) == "table" and #val == 2 then
                            val = { val[1] * 256, val[2] * 256 }
                        elseif type(val) == "number" then
                            val = val * 256
                        end
                    end

                    local passed = false
                    if op == "==" then
                        passed = (statVal == val)
                    elseif op == ">" then
                        passed = (statVal > val)
                    elseif op == "<" then
                        passed = (statVal < val)
                    elseif op == ">=" then
                        passed = (statVal >= val)
                    elseif op == "<=" then
                        passed = (statVal <= val)
                    elseif op == "between" and type(val) == "table" and #val == 2 then
                        local lower = val[1]
                        local upper = val[2]
                        passed = (statVal >= lower and statVal <= upper)
                    end

                    if not passed then
                        match = false
                        break
                    end
                end
            end

            -- Stat checks
            if rule.pstat then
                local stats = type(rule.pstat) == "table" and (rule.pstat.index and { rule.pstat } or rule.pstat) or {}
                for _, stat in ipairs(stats) do
                    local statVal = stat.param and Me:Stat(stat.index, stat.param) or Me:Stat(stat.index)
                    local op = stat.op
                    local val = stat.value

                    -- Multiply value(s) by 256 for special stat indexes 6-9
                    if stat.index >= 6 and stat.index <= 9 then
                        if type(val) == "table" and #val == 2 then
                            val = { val[1] * 256, val[2] * 256 }
                        elseif type(val) == "number" then
                            val = val * 256
                        end
                    end

                    local passed = false
                    if op == "==" then
                        passed = (statVal == val)
                    elseif op == ">" then
                        passed = (statVal > val)
                    elseif op == "<" then
                        passed = (statVal < val)
                    elseif op == ">=" then
                        passed = (statVal >= val)
                    elseif op == "<=" then
                        passed = (statVal <= val)
                    elseif op == "between" and type(val) == "table" and #val == 2 then
                        local lower = val[1]
                        local upper = val[2]
                        passed = (statVal >= lower and statVal <= upper)
                    end

                    if not passed then
                        match = false
                        break
                    end
                end
            end

            -- Sockets check (via stat #194)
            if match and rule.sockets ~= nil then
                local socketVal = Item:Stat(194) or 0
                local passed = false

                if type(rule.sockets) == "string" then
                    if rule.sockets:find(",") then
                        for num in rule.sockets:gmatch("%d+") do
                            if tonumber(num) == socketVal then
                                passed = true
                                break
                            end
                        end
                    else
                        local val = tonumber(rule.sockets:match("%d+")) or 0
                        local op = "=="
                        if rule.sockets:find("%+") then
                            op = ">="
                        elseif rule.sockets:find("%-") then
                            op = "<="
                        end

                        if op == "==" then
                            passed = (socketVal == val)
                        elseif op == ">=" then
                            passed = (socketVal >= val)
                        elseif op == "<=" then
                            passed = (socketVal <= val)
                        end
                    end
                else
                    match = false
                end

                if not passed then
                    match = false
                end
            end


            -- Notify Logic
            if match then
                if rule.notify then
                    if rule.notify_message then
                        print(rule.notify_message .. " - " .. Item.Name)
                    else
                        print("Item dropped: " .. Item.Name)
                    end
                end

                -- Hide Logic
                if rule.hide then
                    Result.Hide = true
                    if rule.print == true then
                        print("Hiding item: " .. Item.Name)
                    end
                    return
                end

                -- Display name handling
                local baseName = Result.Name

                if rule.name_override then
                    baseName = applyColorTags(replace_placeholders(rule.name_override, Item, Me))
                end

                if rule.prefix then
                    baseName = applyColorTags(replace_placeholders(rule.prefix, Item, Me)) .. baseName
                end

                if rule.suffix then
                    baseName = baseName .. applyColorTags(replace_placeholders(rule.suffix, Item, Me))
                end

                -- Default_Hide Logic
                Result.Name = baseName
            elseif rule.default_hide then
                Result.Hide = true
                print("Hiding by default: " .. Item.Name)
                return
            end
        end
    end
end
