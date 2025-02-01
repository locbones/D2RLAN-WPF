# Table of Contents

- [About D2RLAN](#about-d2rlan)
    - [Player Features](#-player-feature-list-)
    - [Author Features](#-author-feature-list-)
- [Player Setup Guide](#player-setup-guide)
- [Author Setup Guide](#author-setup-guide)
    - [Step 1: Modinfo.json](#step-1-edit-modinfojson)
    - [Step 2: File Linking](#step-2-setup-mod-file-linking)
    - [Step 3: Optional Features](#step-3-enabling-optional-features)
    - [Step 4: Updating Files](#step-4-updating-mod-news-features-or-appearance)
- [Program Requirements/Specs](#program-requirementsspecifications)
- [Developer Notes](#developer-notes)
- [Credits](#credits)


# About D2RLAN
This app is designed to be a code-less, open, all-in-one solution for D2R mod management.<br>
It is designed to be used with **Multi-Player** mods that utilize **Version 2.4** of D2R's filebase.<br>
It has many features designed to be used by both mod authors and players to enhance their overall experience.<br>
Mod Authors can customize D2RLAN to their mod by editing their modinfo.json file.<br>
Mod Players can download, update and customize mods in a few button clicks.<br>
Some features require additional author support to function correctly, and will be indicated with a **+**<br>

<img src="https://static.wixstatic.com/media/698f72_5d0916a305544662bb4e0a741ba5edd7~mv2.png" alt="D2RLAN Home View" width="820">
Publicly ready download can be found at http://d2rmodding.com/D2RLaunch


### -Player Feature List-
*Mod Manager-Related Features*
- **+Mod Downloader:** One-Click download and install of many popular mods
- **+Mod File Updater:** One-Click updating to the most recent version's mod files
- **Mod Creator:** One-Click creation of a blank, D2R compatible mod
- **Mod Launcher:** Quickly switch and control startup settings for installed mods
- **+News Display:** Read the recent mod news from author with real-time updating
- **Audio/Text Languages:** Change in-game text or audio languages individually
- **TCP/IP Patcher:** Enables previously removed TCP/IP feature to allow Multi-Player functionality
- **Game Browser/Advertiser:** Includes 'Beacon', an app that allows players to view and advertise active LAN games
- **Fast Load Option:** Fast Load option which extracts ALL game files for slightly improved loading times
- **Queue-Skipping:** Disables BNET access while app open to skip the queue-check process and extra protection

*Quality-Of-Life Features*
- **Automatic Backup/Restore:** Automatically backs up your save and stash files, with quick restoral option
- **Advanced Monster Stats:** Ability to display real-time HP and Resistances for monsters and mercenaries
- **Dynamic HP Display:** Control monster HP bar colors for various life% threshholds
- **Stash Tab Naming:** Ability to individually rename stash tabs to your liking
- **+Buff Icon Display:** Design your own Buff Icon layout for tracking timed buffs (no timer, icons only)
- **Hotkey Controls:** Set hotkeys for Cube Transmuting, Item Identification, Stat/Skill Respec, and much more
- **Chat Controls:** Change coloring of chat channel, player name and message content
- **Merc Identifier:** Add a glowing indicator above your mercenary to help identify them in large crowds
- **Rune Identifier:** Add special visual effects to mid and high runes when dropped on the ground
- **Item Display:** Simplify common items, such as potions, scrolls, etc to use icons instead of text (screen clutter)
- **Hide Helmets:** Ability to Hide all Helmets from the world view display
- **Item Level Display:** Toggleable Item Levels Display (appears next to item name)

*Gameplay-Changing Features* (Author may enable/disable as desired)
- **Monster Customizations:** Various controls which allow editing of Monster Density, Drop Rates, Experience, etc
- **Expanded Storage:** Ability to toggle various Expanded Storage functionality for Inventory, Merc, etc
- **More Shared Stash Tabs:** Unlocks 4 additional Shared Stash Tabs (1 personal, 7 shared)
- **Super Telekinesis:** Upgraded Telekinesis skill which can pick up any* item instead of just pots/scrolls
- **Cheat/Debug Commands:** Ability to enable (and use) dozen's of in-game text commands

*Miscellaneous Features*
- **+Vault Access:** Quick-Access to our external app, *The Vault*, which allows infinite item storage and grail-tracking
- **Quick File Access:** Quickly access mod, save and app config files in the side menu
- **+Community Access:** Quick-Access to the mods wiki, discord or patreon sites
- **Map Seeds:** Quickly force map seeds with pre-defined map layouts (or use your own)
- **Font Switching:** Change in-game font to one of 12 currently supported fonts
- **Color Dyes:** Item Color Dye System for the world view display
- **+UI Themes:** Change UI Theme to the one used in the popular mod, ReMoDDeD
- **+Merged HUD Display:** Merged HUD Design which can be toggled on or off
- **Skill Icon Pack:** Choose one of 3 Skill Icon Packs currently available
- **+Runeword Menu Sorting:** Sort in-game runeword menu by name, type or level
- **Character Renaming:** Rename your character (in-game name also)
- **Character Map Seeds:** Edit your characters map seed directly from save file
- **Cinematic Subtitles:** Improved subtitle text and no longer formatted for the deaf/hard-of-hearing

### -Author Feature List-
- **Code-less:** No code needed to add D2RLAN support; control news, features, community links or appearance
- **Easy Installs/Advertisement:** Add your mod to the database; players can easily view and install with one-click
- **Mod Updating:** Allow players to upgrade to your most recent mod version (with backups) in one-click
- **Real-Time News:** Control the news feed displayed to dynamically address your player-base
- **Feature Controls:** Enable or Disable certain mod features for your mod specifically
- **Player Experience:** Add frequently requested features and QoL perks to your mod instantly
- **Make It Your Own:** Control the displayed app logo and community links (discord, wiki, patreon)

# Player Setup Guide
The process to setup the launcher should be simple and straight forward, but here's how to do it:<br>
- **Step 1:** Download and Install the [.NET Desktop Runtime 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.20-windows-x64-installer) which is required for the app to run.<br>
- **Step 2:** Download D2RLAN from [D2RModding](https://d2rmodding.com/D2RLaunch), then drag the *D2RLAN* folder from the downloaded .zip to your Desktop or other convenient location.<br>
- **Step 3a:** Browse to the *Launcher* folder and run *D2RLAN.exe*.<br>
- **Step 3b:** Depending on your version, an update notification may appear in the bottom left of the app.<br>
- **Step 4:** Click the *Download New Mod* button and select 'Base TCP Files' from the dropdown box.<br>
This will acquire the setup package needed to download **Version 2.4** D2R Files.<br>
This specific version of D2R is required to use TCP/IP functionality and D2RLAN.<br>
Depending on your network speed, current traffic, etc...this download may take anywhere from minutes to hours.<br>
- **Step 5:** Click the *Download New Mod* button and select your desired mod from the dropdown box.<br>
- **Step 5:** View the *QoL Options* and other settings to customize things to your liking.<br>
- **Step 6:** Press the *Play Mod* button to start the mod with your chosen configuration and enjoy!

*You may need to run the launcher as Administrator and/or exclude the folder from your Antivirus<br>
**For any issues or questions, please reach out in our [Discord](https://www.discord.gg/pqUWcDcjWF)

# Author Setup Guide
In order to fully support the various features of this launcher, some additional steps will be needed from you.<br>
Some features will require your permission to use, additional files provided or mod download/community links.<br>
As mentioned, no coding is needed for any of these customizations and the instructions are outlined below.<br>

## Step 1: Edit modinfo.json
All of the D2RLAN customizations are determined by this file, since it is included/required by your mod already.<br>
With that said, please edit your modinfo.json file using the following template, being sure to respect the line counts.<br>
The launcher will compare the players file to the web file to determine mod version status and control features/info.<br>
Change your enabled/disabled options as desired (Expect the Option Controls to be updated in future releases)<br>
```
{
    "name": "MyModName",
    "savepath": "MyModName/" 

/*
--My Mod Details--
Mod Download: https://MyModFilesLink.zip
Mod Config Download: https://MyModInfo.json
Mod Version: 0.1.2.3
News 1 Title: "My Mod was Updated! (Version 0.1.2.3)"
News 1 Message: "This is some news message that I like"
News 2 Title: "More News for the Community!"
News 2 Message: "This is some other news message that I like"

--General Options--
Map Layouts: Enabled
UI Themes: Enabled
Customizations: Enabled
Vault Access: Enabled

--Additional Options--
Item Icons: Enabled
Runeword Sorting: Enabled
HUD Display: Enabled
Monster Stats Display: Enabled

--Author Links--
Discord: https://MyDiscordLink.com
Wiki: https://MyWikiLink.com
Patreon: https://MyPatreonLink.com
*/

}
```

## Step 2: Setup Mod File Linking
In order to provide downloading, updating and configuration changes to be made dynamically by the launcher, we need to setup proper links.
For a proper link, it needs to be both **static** and **direct**.<br>
- **Static** - This means when the file has been updated/replaced, the link itself does not change
- **Direct** - This means that when the link is clicked, the file is downloaded directly, not a webpage button download

To setup a link that satisfies both of these requirements, you can use services such as Google Drive, Dropbox, Github, Amazon S3, etc.
Provided below are instructions for some of them I have used previously or currently:
- **Github** - Click the green Code button, then right-click the Download Zip option and select Copy Link Address
- **Google Drive** - Copy your google provided link into this [Online Generator](https://sites.google.com/site/gdocs2direct/) to convert it to a static-direct link.<br>
When updating the file, you must use the **File Properties > File Information > Manage Versions** method<br>
- **Dropbox** - Replace the **&dl=0** at the end of your dropbox provided url with **&dl=1**

The First time you upload modinfo.json, you will need to use a dummy config link, because you havnt uploaded it yet<br>
I recommend adding your mod to the [Mod Database](https://docs.google.com/spreadsheets/d/1RMqexbqTzxOyjk7tWbLhRYJk9RkzPGJ9cKHSLtsuGII/edit#gid=0), which allows players to easily view and install your mod.<br>
*As long as you follow the above linking rules, then the links provided in your modinfo.json file will never need to be updated between mod or config changes!*<br>

## Step 3: Enabling Optional Features
For some features, additional files must be provided to D2RLAN, due to the variety of changes/complexity.<br>
As an example, the Event Manager cannot host Special Events if it has no instructions or files provided for this task.<br>
Any feature that is optional or requires file-safekeeping will use a new D2RLAN folder in your mod directory.<br>
Each feature will be placed in it's own subfolder within it, following these rules:<br>

**Runeword Menu Sorting:** Utilizes a folder named **Runeword Sort** and may contain up to 6 files:
- **runewords-ab.json / helppanelhd-ab.json:** Used to display the runewords sorted **Alphabetically**
- **runewords-it.json / helppanelhd-it.json:** Used to display the runewords sorted **By ItemType**
- **runewords-lv.json / helppanelhd-lv.json:** Used to display the runewords sorted **By Required Level**<br>

*helppanel* files are optional if you've chosen to replace it with your runeword menu (quick-access)

**UI Theme:** Utilizes a folder named **UI Theme** and should contain 2 folders (for now):
- **Retail:** Used to display your own modified UI, based on the Retail Theme.
- **ReMoDDeD:** Used to display the heavily customized UI, based on the ReMoDDeD mod.

**Buff Icons:** Utilizes a folder named **Buff Icons** and should contain X files (explained below, not yet complete):
- **Skill_Names.txt:** Used to provide the launcher with your mods list of *buff skills* (if different from retail).
- **Preview_SkillName.png:** For each custom buff icon you have, include an image the launcher can use for it.

**Custom Mod/App Logo:** Looks for a file named **Logo.png** to be used in the top left of D2RLAN.<br>
The size of the logo can vary to your liking, but I recommend something around 200x200 or so.

**Merged HUD:** Contains the needed files for the **Merged HUD** Option:

As previously mentioned, some folders will be created automatically by the launcher, mostly for file-safekeeping:
- **Customizations:** Created to store unedited copies of the armor, misc, weapons, levels and treasureclass txt files.<br>
This is needed for the Monster Customizations options to work correctly.<br>
- **Monster Stats:** Created to store edited copies of the monster hp bar layout files, for various option displays.<br>
This is needed for the Monster Stats option to work correctly.

## Step 4: Updating Mod, News, Features or Appearance
To update your mod or config files, simply replace the .zip or .json file used in w/e you service chose in **Step 2**.<br>
If you followed those instructions carefully, then the link does not need updating and your edits are immediately live.<br>
When D2RLAN goes to download the mod or access the config file; it will retrieve the updated version(s).<br>
You can use this to dynamically update mod files, allowed features, news messages, app logo, etc.<br>
Keep in mind that you are *pushing* data to the web, and the launcher is automatically *pulling* that data down.<br>

# Program Requirements/Specifications
In order to fully utilize this app, or receive staff support, some requirements must be met.
- **.NET Desktop Runtime 7.0:** This program is included in the D2RLAN download, but can also be found via the [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) website. It is needed to run the program itself.<br>
- **C++ Redistributable 2015-2022:** This program is included in the D2RLAN download, but can also be found via the [Microsoft](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) website. It is needed to perform CASC Extraction of internal game storage.
- **Windows OS:** This is the only supported Operating System for the launcher, although emulation layers may also work for it (WINE, Lutris, etc)<br>
- **Battle.Net Purchased:** This program is only intended for and actively tries to be restricted to, legally purchased D2R copies. If you want to support modding, then purchase the game!<br>
- **Code Base:** This program was designed using C# and WPF. I am a novice coder, so expect inconsistencies, inefficiencies and general issues.<br>
- **D2RHUD.dll:** This file is used to enable hotkey controls and advanced monster stats display options [(Source)](https://github.com/locbones/D2RHUD-2.4)<br>
It is also used for various 'hooks', such as Save/Chat functionality while using TCP/IP

# Developer Notes
This app was made because I am passionate about helping everyone get the most out of their D2R experience.<br>
I put much effort into making it easy, open and powerful, while balancing it with author intentions/efforts.<br>
So thanks for everyones patience and support during this free-time side project of mine; some final words on it:

**To The Players:**<br>
Enjoy the many new QoL features, TCP/IP functionality, mod controls and hassle-free modding that comes with using D2RLAN.<br>
I know sometimes the worst part about modding, can be dealing with all the frustration and confusion around them.<br>
I also know that everyone likes to play just a little bit differently, and noone can say no to some added QoL.<br>
So I hope this app helps fill in a large gap of what may have been missing from your mod experience.

**To The Authors**<br>
Enjoy what I hope is a very simple, code-less method to control the launchers various systems to your liking.<br>
(Please keep in mind you will need to use the **Version 2.4** file package when making your mod compatible with TCP/IP).<br>
I know sometimes it can be frustrating dealing with so many files, troubleshooting with your community, etc.<br>
I also know that many of the same features get requested by players or wish were included by default.<br>
So I hope this app helps provide some of that to your player-base; letting them enjoy your mod even longer.

**To The Developers**<Br>
I respectfully request that you help improve this project rather than fork it, if you wish to modify it.<br>
Much effort was made to allow authors control of the launcher in a dynamic fashion, without coding needed.<br>
I wish for it to be open and hope you might be convinced to help improve it further.<br>
With that said; a standard **GPL License** has been attached to this project.

In order for build deployment, you will need to provide your own Licenses/API Keys for the following services:<br>
**SyncFusion:** Used for various form controls and library functions<br>
**Google Sheets:** Used to retrieve the Mod Listing from the Mod Database and update the Download Dropdown<br>
File should be named *appSettings.json* and placed in the *Resources* folder. An example file has already been included.<br>

# Credits
Special thanks to the following people or groups for their help along this project's journey so far:<br>
- [Ethan-Braddy](https://github.com/Ethan-Braddy) for helping convert this project from WinForms to WPF, improving stability/performance<br>
- [Dschu012](https://github.com/dschu012) for being there to answer my dumb questions and D2RHUD's base implementation<br>
- [D2RModding Community](https://www.discord.gg/pqUWcDcjWF) for being patient, supportive and assisting with bug-reports or improvements<br>
- Killshot for helping with D2RHUD function hooks and debug assistance <br>
