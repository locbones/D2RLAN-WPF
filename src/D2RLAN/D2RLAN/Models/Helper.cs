using D2RLAN.Models.Enums;
using D2RLAN.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using D2RLAN.Views;
using Newtonsoft.Json;
using Syncfusion.Licensing;
using D2RLAN.Extensions;
using CASCLibNET;

namespace D2RLAN.Models
{
    public static class Helper
    {
        public static async Task<UserSettings> GetDefaultUserSettings()
        {
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(await GetResource("DefaultUserSettings.json"));
            return userSettings;
        }

        public static async Task<string> GetResource(string name)
        {
            return await Assembly.GetExecutingAssembly().ReadResourceAsync(name);
        }

        public static async Task<byte[]> GetResourceByteArray(string name)
        {
            return await Assembly.GetExecutingAssembly().ReadResourceByteArrayAsync(name);
        }

        public static byte[] GetResourceByteArray2(string name)
        {
            return Assembly.GetExecutingAssembly().ReadResourceByteArrayAsync2(name);
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static async Task<UserSettings> ConvertUserSettings(string[] oldUserSettings)
        {
            try
            {
                UserSettings userSettings = new UserSettings();

                foreach (string line in oldUserSettings)
                {
                    if (line.Contains("Infinite Respec:"))
                    {
                        userSettings.InfiniteRespec = bool.Parse(line.Split("Infinite Respec:")[1].Trim());
                    }
                    if (line.Contains("Reset Maps:"))
                    {
                        userSettings.ResetMaps = bool.Parse(line.Split("Reset Maps:")[1].Trim());
                    }
                    if (line.Contains("Audio Language:"))
                    {
                        userSettings.AudioLanguage = int.Parse(line.Split("Audio Language:")[1].Trim());
                    }
                    if (line.Contains("Text Language:"))
                    {
                        userSettings.TextLanguage = int.Parse(line.Split("Text Language:")[1].Trim());
                    }
                    if (line.Contains("UI Theme:"))
                    {
                        userSettings.UiTheme = int.Parse(line.Split("UI Theme:")[1].Trim());
                    }
                    if (line.Contains("Item Icons:"))
                    {
                        userSettings.ItemIcons = int.Parse(line.Split("Item Icons:")[1].Trim());
                    }
                    if (line.Contains("Merc Icons:"))
                    {
                        userSettings.MercIcons = int.Parse(line.Split("Merc Icons:")[1].Trim());
                    }
                    if (line.Contains("Runeword Sorting:"))
                    {
                        userSettings.RunewordSorting = int.Parse(line.Split("Runeword Sorting:")[1].Trim());
                    }
                    if (line.Contains("Auto Backups:"))
                    {
                        userSettings.AutoBackups = int.Parse(line.Split("Auto Backups:")[1].Trim());
                    }
                    if (line.Contains("HUD Design:"))
                    {
                        userSettings.HudDesign = int.Parse(line.Split("HUD Design:")[1].Trim());
                    }
                    if (line.Contains("Item Ilvl's:"))
                    {
                        userSettings.ItemIlvls = int.Parse(line.Split("Item Ilvl's:")[1].Trim());
                    }
                    if (line.Contains("Buff Icons:"))
                    {
                        userSettings.BuffIcons = int.Parse(line.Split("Buff Icons:")[1].Trim());
                    }
                    if (line.Contains("Monster Stats Display:"))
                    {
                        userSettings.MonsterStatsDisplay = int.Parse(line.Split("Monster Stats Display:")[1].Trim());
                    }
                    if (line.Contains("Hide Helmets:"))
                    {
                        userSettings.HideHelmets = int.Parse(line.Split("Hide Helmets:")[1].Trim());
                    }
                    if (line.Contains("Personalized Tabs::"))
                    {
                        userSettings.PersonalizedTabs = int.Parse(line.Split("Personalized Tabs:")[1].Trim());
                    }
                    if (line.Contains("HDR Fix:"))
                    {
                        userSettings.HdrFix = bool.Parse(line.Split("HDR Fix:")[1].Trim());
                    }
                    if (line.Contains("Skill Icon Pack:"))
                    {
                        userSettings.SkillIcons = int.Parse(line.Split("Skill Icon Pack:")[1].Trim());
                    }
                }

                return userSettings;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static string GetCultureString(string name)
        {
            eLanguage appLanguage = ((eLanguage)Settings.Default.AppLanguage);

            CultureInfo currentCulture = new CultureInfo(appLanguage.GetAttributeOfType<DisplayAttribute>().Name.Split(' ')[1].Trim(new[] { '(', ')' }) /*.Insert(2, "-")*/);

            return Resources.ResourceManager.GetString(name, currentCulture);
        }

        public static async Task<ModInfo> ParseModInfo(string modInfoPath)
        {
            try
            {
                ModInfo modInfo = new();

                if (!File.Exists(modInfoPath))
                {
                    MessageBox.Show("Could not locate ModInfo.json!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                using StreamReader read = new(modInfoPath);

                while (await read.ReadLineAsync() is { } line)
                {
                    if (line.Contains("name"))
                    {
                        modInfo.Name = line.Split(":")[1].Trim().Replace("\"", "").Replace(",", "");
                    }
                    else if (line.Contains("savepath"))
                    {
                        modInfo.SavePath = line.Split(":")[1].Trim().Replace("\"", "");
                    }
                    else if (line.Contains("Mod Download"))
                    {
                        modInfo.ModDownloadLink = line.Split("Mod Download:")[1].Trim();
                    }
                    else if (line.Contains("Mod Config"))
                    {
                        modInfo.ModConfigDownloadLink = line.Split("Mod Config Download:")[1].Trim();
                    }
                    else if (line.Contains("Mod Version"))
                    {
                        modInfo.ModVersion = line.Split(":")[1].Trim();
                    }
                    else if (line.Contains("News 1 Title"))
                    {
                        modInfo.ModTitle = line.Split(":")[1].Trim().Replace("\"", "");
                    }
                    else if (line.Contains("News 1 Message"))
                    {
                        modInfo.ModDescription = line.Split(":")[1].Trim().Replace("\"", "");
                    }
                    else if (line.Contains("News 2 Title"))
                    {
                        modInfo.NewsTitle = line.Split(":")[1].Trim().Replace("\"", "");
                    }
                    else if (line.Contains("News 2 Message"))
                    {
                        modInfo.NewsDescription = line.Split(":")[1].Trim().Replace("\"", "");
                    }
                    else if (line.Contains("Map Layouts"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.MapLayouts = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("UI Themes"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.UIThemes = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("Customizations"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.Customizations = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("Vault Access"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.VaultAccess = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("Item Icons"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.ItemIcons = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("Runeword Sorting"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.RunewordSorting = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("HUD Display"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.HudDisplay = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("Monster Stats Display"))
                    {
                        string value = line.Split(":")[1].Trim();

                        modInfo.MonsterStatsDisplay = value.ToUpperInvariant() == "ENABLED";
                    }
                    else if (line.Contains("Discord"))
                    {
                        string value = line.Replace("Discord: ", "");

                        modInfo.Discord = value;
                    }
                    else if (line.Contains("Wiki"))
                    {
                        string value = line.Replace("Wiki: ", "");

                        modInfo.Wiki = value;
                    }
                    else if (line.Contains("Patreon"))
                    {
                        string value = line.Replace("Patreon: ","");

                        modInfo.Patreon = value;
                    }
                }

                return modInfo;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task CloneDirectory(string source, string destination)
        {
            if (!Directory.Exists(source))
                return;

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source))
            {
                string destFile = System.IO.Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, destFile);
            }

            foreach (string directory in Directory.GetDirectories(source))
            {
                string destDir = System.IO.Path.Combine(destination, Path.GetFileName(directory));
                await CloneDirectory(directory, destDir);
            }
        }

        public static async Task<string> FindFolderWithMpq(string directory)
        {
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                if (subDirectory.Contains(".mpq"))
                {
                    return subDirectory;
                }
                string result = await FindFolderWithMpq(subDirectory);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static void ExtractFileFromCasc(string cascStoragePath, string specifiedFileName, string dest, string oldDataName)
        {
            using CASCStorage storage = new CASCStorage(cascStoragePath);
            foreach (CASCFileInfo file in storage.Files)
            {
                // Check if the file name matches the specified file name or pattern
                if (!file.FileName.Equals(specifiedFileName, StringComparison.OrdinalIgnoreCase))
                {
                    //System.Diagnostics.Debug.WriteLine(file.FileName);
                    continue; // Skip files that don't match
                }

                System.Diagnostics.Debug.WriteLine(file.FileName);

                // Replace the data:data separator with data/data
                string modifiedFileName = file.FileName.Replace($"{oldDataName}\\", "");

                // Create the full path for the extracted file
                string extractedFilePath = System.IO.Path.Combine(dest, modifiedFileName);

                // Create the directory if it doesn't exist
                string extractedFileDirectory = Path.GetDirectoryName(extractedFilePath);
                Directory.CreateDirectory(extractedFileDirectory);

                // Open the file for reading from CASC
                using CASCFileStream reader = storage.OpenFile(file.FileName);
                // Create a FileStream to write the extracted file
                using FileStream writer = new FileStream(extractedFilePath, FileMode.Create);
                // Read and write the file contents in chunks
                byte[] buffer = new byte[4096]; // You can adjust the buffer size as needed
                int bytesRead;
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, bytesRead);
                }

                // If you only want to extract one specified file, you can break out of the loop here.
                break;
            }
        }

        public static void CreateFileIfNotExists(string filePath, byte[] fileData)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                File.WriteAllBytes(filePath, fileData);
            }
            else
                File.WriteAllBytes(filePath, fileData);
        }
    }
}
