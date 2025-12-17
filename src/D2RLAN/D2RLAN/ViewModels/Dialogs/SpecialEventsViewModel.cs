using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace D2RLAN.ViewModels.Dialogs;

public class SpecialEventsViewModel : Screen
{
    //This system is largely unused due to 2.4's lack of Terror Zones
    #region ---Static Members---
    private ILog _logger = LogManager.GetLogger(typeof(DownloadNewModViewModel));
    private ObservableCollection<KeyValuePair<string, string>> _mods = new ObservableCollection<KeyValuePair<string, string>>();

    //TODO: Both of these should really be acquired in a more safe way such as querying an API endpoint to get mod info.
    private string _serviceAccountEmail = "D2RLAN@D2RLANcore.iam.gserviceaccount.com";
    private string _privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCqwvhB5hfC8fK8\n1SVdybxQAgSXKegys3QHsu+xx/Gv+/f70PkuZFt35UODV0385vlk0nu0aohIWmkF\nt/tmyhITSQqLNfWrVoKHfENRVtcaHn788BOCfDXFNsx0anz9FMACM4ZXBe3zPWGr\nfs/qtG+eSQ4eKP1vR8qn/a5Dj2WR26lNUzEPxMzGjPzwLcLqUUPbX1RGiRD1cKrC\noa7QNbzcPedFaOFRhfj1HmQK3H0H63qFCZOXW+yZoVRRMItCoZug4oJFltViU61m\n0rIbGbmdeU3DIAT93mi3o7WYOcrYC/XhVhBI2Fz1qsMKp4XXPzUZhIFEuidKJvv6\nhn+PrzLpAgMBAAECggEAAnwCs6a+2sG9Z9zsBcDNIhbdbTuZWr98pS4HybzgedB/\nK6U/MtsX75cg09Td2BueLkbXsOjJ4c+a7o/eMwEmoSwzYJIg6GTCUmlO62yJhaJC\n87gkeIYJHDzvXZQ9DEuUfZO1VSfLbfoLJT0blk0YwKNMdsje4xMW0jnhIq9/6U7U\nOkKFQRw7z+Hm/DpwZNJPR6rm3fZlqgL/NspKg0fz4Rark713WeGkgK+/YcyHbaYk\neq6lAJz/eSPwOQ/DXWOpV3F1Ide46V8LzHbg70qj153FWyZUPXeT8t7tlFFdMZ6b\ncuk6qDjWfdmYraVmnnpzah/P8MkJMzyy6Fig4Eai+QKBgQDoGa7h3Xmmze7oLxyd\nqEfY/85RCONVys4m2IlBDet1mHfrFEPnf76FNEMTEBgwL5doGMO4FC9bwAJW3BrD\n8nYwTiZ20vOmxYkIwKt7UXYk+fxEzwBmwa1Wil9JjeSUkhG4I7V+r0i2b71zZlMr\n/oEi120Rq/2WZUyQDfB+UoBMVQKBgQC8WFuL1r5ZWtNGyp5V6v4/gOd0k+Ts3FQO\npNM2bohBWNYMulQSBiKiPz7n2vmq5qxec30dn+be8SMfBJgDaasn58wiMDoAitCB\nEnBnNBKygX6nPx1/83syAYmR9wRlINiHKpWK5p4XXtbANMC/XcH+PGR5PGJb7d/i\n/CX8s1AgRQKBgH0zHXsJFU49V9o3T6Bb3iXYF1rvCHKG651YwPEuqQzOKiHM1LRT\n3FnOT0BBNksH4QxuD2WEvecoNBrWsDly2P5Fqcn/ER+s/raR9+6Vir13e/VCFF1Z\nrD86dRwgRmU+RgCmgojL1NVUgUV2tPbOWqqIunUF6czu59XtLwV1S2/hAoGAIjDE\nBaGpEl17hxlXHu+20d5bpf0HDLx+gd4H/ZSZJYuz58GXa2IzvVJP4BUPR6fyWH8M\nkmkppwUNRB84XT48dNUOaJJqpRiN+zBWuVVpo4AAduntOAICNjSzPY0i/hy1Uew4\nE2wD/OgZgfDRoKurgLSD5MJCdL+86d6uIq6GeCUCgYEAlXLZttb0ocGChfzSLQhd\nnXM3uVuxxqCgXaK9ocUDfC7oF0O0Cq9pL8jyUglHkXjDjTTb/Isfb8MQZi1502ew\nfOvhjRvHivwEED3IzDDg3UL6j0h1kkP1cm2rvfAi7ohzR3TnVOdOkUidn/o111If\nskO0qnMmEU8OVCwo3id1W5E=\n-----END PRIVATE KEY-----\n";
    private KeyValuePair<string, string> _selectedMod;
    private string _modDownloadLink;
    private double _downloadProgress;
    private bool _progressBarIsIndeterminate;
    private string _progressStatus;
    private string _downloadProgressString;
    public string eventNameStr;
    public bool eventJoined = false;
    private string _eventImage;

    #endregion

    #region ---Window/Loaded Handlers---

    public SpecialEventsViewModel()
    {
            GetCurrentEvents();
    }
    public SpecialEventsViewModel(ShellViewModel shellViewModel)
    {
        DisplayName = "Special Event Status";
        ShellViewModel = shellViewModel;
        Execute.OnUIThread(async () =>
                           {
                               await GetCurrentEvents();
                           });
    }

    #endregion

    #region ---Properties---

    public ShellViewModel ShellViewModel { get; }
    public KeyValuePair<string, string> SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (value.Equals(_selectedMod)) return;
            _selectedMod = value;
            NotifyOfPropertyChange();
        }
    }
    private string _eventNameText;
    private string _eventTypeText;
    private string _eventLocText;
    private string _eventDur1Text;
    private string _eventDur2Text;
    private string _eventDesc1Text;
    private string _eventDesc2Text;
    private string _eventDesc3Text;
    private string _eventDesc4Text;
    private string _eventLinkText;
    private string _eventImageText;
    private string _eventMLvlText;
    private string _eventPLvlText;
    private string _eventPDiffText;
    private string _eventPExpText;
    public string EventNameText
    {
        get => _eventNameText;
        set
        {
            if (_eventNameText != value)
            {
                _eventNameText = value;
                OnPropertyChanged(nameof(EventNameText));
            }
        }
    }
    public string EventTypeText
    {
        get => _eventTypeText;
        set
        {
            if (_eventTypeText != value)
            {
                _eventTypeText = value;
                OnPropertyChanged(nameof(EventTypeText));
            }
        }
    }
    public string EventLocText
    {
        get => _eventLocText;
        set
        {
            if (_eventLocText != value)
            {
                _eventLocText = value;
                OnPropertyChanged(nameof(EventLocText));
            }
        }
    }
    public string EventDur1Text
    {
        get => _eventDur1Text;
        set
        {
            if (_eventDur1Text != value)
            {
                _eventDur1Text = value;
                OnPropertyChanged(nameof(EventDur1Text));
            }
        }
    }
    public string EventDur2Text
    {
        get => _eventDur2Text;
        set
        {
            if (_eventDur2Text != value)
            {
                _eventDur2Text = value;
                OnPropertyChanged(nameof(EventDur2Text));
            }
        }
    }
    public string EventDesc1Text
    {
        get => _eventDesc1Text;
        set
        {
            if (_eventDesc1Text != value)
            {
                _eventDesc1Text = value;
                OnPropertyChanged(nameof(EventDesc1Text));
            }
        }
    }
    public string EventDesc2Text
    {
        get => _eventDesc2Text;
        set
        {
            if (_eventDesc2Text != value)
            {
                _eventDesc2Text = value;
                OnPropertyChanged(nameof(EventDesc2Text));
            }
        }
    }
    public string EventDesc3Text
    {
        get => _eventDesc3Text;
        set
        {
            if (_eventDesc3Text != value)
            {
                _eventDesc3Text = value;
                OnPropertyChanged(nameof(EventDesc3Text));
            }
        }
    }
    public string EventDesc4Text
    {
        get => _eventDesc4Text;
        set
        {
            if (_eventDesc4Text != value)
            {
                _eventDesc4Text = value;
                OnPropertyChanged(nameof(EventDesc4Text));
            }
        }
    }
    public string EventLinkText
    {
        get => _eventLinkText;
        set
        {
            if (_eventLinkText != value)
            {
                _eventLinkText = value;
                OnPropertyChanged(nameof(EventLinkText));
            }
        }
    }
    public string EventImageText
    {
        get => _eventImageText;
        set
        {
            if (_eventImageText != value)
            {
                _eventImageText = value;
                OnPropertyChanged(nameof(EventImageText));
            }
        }
    }
    public string EventMLvlText
    {
        get => _eventMLvlText;
        set
        {
            if (_eventMLvlText != value)
            {
                _eventMLvlText = value;
                OnPropertyChanged(nameof(EventMLvlText));
            }
        }
    }
    public string EventPLvlText
    {
        get => _eventPLvlText;
        set
        {
            if (_eventPLvlText != value)
            {
                _eventPLvlText = value;
                OnPropertyChanged(nameof(EventPLvlText));
            }
        }
    }
    public string EventPDiffText
    {
        get => _eventPDiffText;
        set
        {
            if (_eventPDiffText != value)
            {
                _eventPDiffText = value;
                OnPropertyChanged(nameof(EventPDiffText));
            }
        }
    }
    public string EventPExpText
    {
        get => _eventPExpText;
        set
        {
            if (_eventPExpText != value)
            {
                _eventPExpText = value;
                OnPropertyChanged(nameof(EventPExpText));
            }
        }
    }
    public string EventImage
    {
        get => _eventImage;
        set
        {
            if (_eventImage != value)
            {
                _eventImage = value;
                OnPropertyChanged(nameof(EventImage));
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region ---Special Event Functions---

    private async Task GetCurrentEvents()
    {
        string eventScheduleFile = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Events/EventSchedule.txt");
        string eventFolder = System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Events/");

        if (File.Exists(eventScheduleFile))
        {
            string[] scheduleContents = File.ReadAllLines(eventScheduleFile);
            DateTime currentTime = DateTime.Now;
            bool eventFound = false;

            // Iterate through each line and extract data
            foreach (string line in scheduleContents)
            {
                string[] parts = line.Split(',');

                if (parts.Length == 3)
                {
                    string startTimeStr = parts[0].Trim();
                    string endTimeStr = parts[1].Trim();
                    eventNameStr = parts[2].Trim();
                    DateTime startTime;
                    DateTime endTime;

                    if (DateTime.TryParse(startTimeStr, out startTime) && DateTime.TryParse(endTimeStr, out endTime))
                    {
                        startTime = ConvertUtcToLocalTime(startTimeStr);
                        endTime = ConvertUtcToLocalTime(endTimeStr);

                        // Check if the current time falls within the event's time range
                        if (currentTime >= startTime && currentTime <= endTime)
                        {
                            if (Directory.Exists(System.IO.Path.Combine(eventFolder, eventNameStr)))
                            {
                                string TZFile = System.IO.Path.Combine(System.IO.Path.Combine(eventFolder, eventNameStr), "hd/global/excel/desecratedzones.json");

                                try
                                {
                                    if (File.Exists(TZFile))
                                    {
                                        string fileContent = File.ReadAllText(TZFile);
                                        string[] eventInfo = new string[]
                                        { "Event Name: ", "Event Type: ", "Event Location(s): ", "\"start_time_utc\": ", "\"end_time_utc\": ", "Event Description 1: ", "Event Description 2: ", "Event Description 3: ", "Event Description 4: ", "Event Link: ", "Event Image: ", "\"bound_incl_max\": ", "\"boost_level\": ", "\"difficulty_scale\": ", "\"boost_experience_percent\": " };


                                        Dictionary<string, string> extractedValues = new Dictionary<string, string>();

                                        foreach (string pattern in eventInfo)
                                        {
                                            string value = ExtractString(fileContent, pattern);
                                            extractedValues[pattern] = value;
                                        }

                                        List<string> eventInfo2 = new List<string>
                                        { "\"bound_incl_max\": ", "\"boost_level\": ", "\"difficulty_scale\": ", "\"boost_experience_percent\": " };

                                        Dictionary<string, List<string>> extractedValues2 = new Dictionary<string, List<string>>();

                                        foreach (string pattern in eventInfo2)
                                        {
                                            List<string> values = ExtractStrings(fileContent, pattern);
                                            values = values.GetRange(0, Math.Min(3, values.Count));
                                            extractedValues2[pattern] = values;
                                        }

                                        string eventMLvl = FormatValues(extractedValues2["\"bound_incl_max\": "], ", /* Maximum level of a terrorized monster. MAX(bound_incl_max, original_monster_level) */").Replace(",", "");
                                        string eventPLvl = FormatValues(extractedValues2["\"boost_level\": "], ", /* player_level + boost_level = terrorized_monster_level */", "+").Replace(",", "");
                                        string eventPDiff = FormatValues(extractedValues2["\"difficulty_scale\": "], ", /* Fake the amount of players in the game. AKA /players X */").Replace(",", "");
                                        string eventPExp = FormatValuesWithPercentage(extractedValues2["\"boost_experience_percent\": "], " /* Bonus experience percentage applied at to the monster's base experience in monstats.txt */");

                                        string eventName = extractedValues["Event Name: "];
                                        string eventType = extractedValues["Event Type: "];
                                        string eventLoc = extractedValues["Event Location(s): "];
                                        string eventDur1 = extractedValues["\"start_time_utc\": "].Replace("\"", "").Replace(",", "");
                                        string eventDur2 = extractedValues["\"end_time_utc\": "].Replace("\"", "").Replace(",", "").Replace(" /* Use end time to automate multiple configs. Useful for events. */", "");
                                        string eventDesc1 = extractedValues["Event Description 1: "];
                                        string eventDesc2 = extractedValues["Event Description 2: "];
                                        string eventDesc3 = extractedValues["Event Description 3: "];
                                        string eventDesc4 = extractedValues["Event Description 4: "];
                                        string eventLink = extractedValues["Event Link: "];
                                        string eventImage = extractedValues["Event Image: "];

                                        DateTime localTime1 = ConvertUtcToLocalTime(eventDur1);
                                        DateTime localTime2 = ConvertUtcToLocalTime(eventDur2);
                                        string formattedTime1 = localTime1.ToString("MM-dd HH:mm");
                                        string formattedTime2 = localTime2.ToString("MM-dd HH:mm");

                                        string logoPath = System.IO.Path.Combine(System.IO.Path.Combine(eventFolder, eventNameStr), "Event.png");
                                        if (File.Exists(logoPath))
                                        {
                                            string tempPath = System.IO.Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                                            File.Copy(logoPath, tempPath, true);
                                            EventImage = tempPath;
                                        }

                                        ChangeText(eventName, eventType, eventLoc, formattedTime1.ToString(), formattedTime2.ToString(), eventDesc1, eventDesc2, eventDesc3, eventDesc4, eventLink, eventImage, eventMLvl, eventPLvl, eventPDiff, eventPExp);

                                        if (eventJoined == true)
                                        {
                                            if (!Directory.Exists(System.IO.Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent")))
                                                CloneDirectory(ShellViewModel.SelectedModDataFolder, System.IO.Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"));

                                            Directory.Delete(ShellViewModel.SelectedModDataFolder, true);
                                            CloneDirectory(System.IO.Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"), ShellViewModel.SelectedModDataFolder);

                                            await CopyDirectoryAndOverwrite(System.IO.Path.Combine(System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Events/"), eventNameStr), ShellViewModel.SelectedModDataFolder);
                                            MessageBox.Show("Event Joined Successfully!");
                                        }
                                        //MessageBox.Show($"Current Event\n{eventNameStr}\n{startTime}\n{endTime}");
                                        eventFound = true;
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    _logger.Error(ex);
                                    return;
                                }
                            }
                        }
                    }
                    else
                        Console.WriteLine("Invalid date format in line: " + line);
                }
                else
                    Console.WriteLine("Invalid line format: " + line);
            }

            // No Event Found
            if (!eventFound)
            {
                ChangeText("No Active Events", "N/A", "N/A", "N/A", "N/A", "No Data Available", "", "", "", "", "", "", "", "", "");

                if (Directory.Exists(System.IO.Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent")))
                {
                    Directory.Delete(ShellViewModel.SelectedModDataFolder, true);
                    Directory.Move(System.IO.Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"), ShellViewModel.SelectedModDataFolder);
                }
            }
        }
    }
    public void ChangeText(string eventName, string eventType, string eventLoc, string eventDur1, string eventDur2, string eventDesc1, string eventDesc2, string eventDesc3, string eventDesc4, string eventLink, string eventImage, string eventMLvl, string eventPLvl, string eventPDiff, string eventPExp)
    {
        EventNameText = eventName.Replace("\\n", "\n");
        EventTypeText = eventType.Replace("\\n", "\n");
        EventLocText = eventLoc.Replace("\\n", "\n");
        EventDur1Text = eventDur1.Replace("\\n", "\n");
        EventDur2Text = eventDur2.Replace("\\n", "\n");
        EventDesc1Text = eventDesc1.Replace("\\n", "\n") + "\n";
        EventDesc2Text = eventDesc2.Replace("\\n", "\n") + "\n\n";
        EventDesc3Text = eventDesc3.Replace("\\n", "\n") + "\n";
        EventDesc4Text = eventDesc4.Replace("\\n", "\n") + "\n\n";
        EventLinkText = "\n" + eventLink.Replace("\\n", "\n");
        EventImageText = eventImage.Replace("\\n", "\n");
        EventMLvlText = eventMLvl.Replace("\\n", "\n");
        EventPLvlText = eventPLvl.Replace("\\n", "\n");
        EventPDiffText = eventPDiff.Replace("\\n", "\n");
        EventPExpText = eventPExp.Replace("\\n", "\n");
    }
    public async void OnJoinEvent()
    {
        eventJoined = true;
        MessageBox.Show("Please allow the launcher a few moments to make file changes:\n- Backup Non-Event Files\n- Install Event Files\n\nThe launcher will notify you when it is finished");
        CloneDirectory(ShellViewModel.SelectedModDataFolder, System.IO.Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent"));
        await CopyDirectoryAndOverwrite(System.IO.Path.Combine(System.IO.Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Events/"), eventNameStr), ShellViewModel.SelectedModDataFolder);
        await GetCurrentEvents();
    }
    public async void OnLeaveEvent()
    {
        string eventFolder = Path.Combine(ShellViewModel.SelectedModDataFolder, "D2RLAN/Events/");
        string TZFile = Path.Combine(eventFolder, eventNameStr, "hd/global/excel/desecratedzones.json");
        string TZFileContents = "";

        if (File.Exists(TZFile))
            TZFileContents = File.ReadAllText(TZFile);

        if (TZFileContents.Contains("Event Name:"))
        {
            eventJoined = false;

            string backupFolder = Path.Combine(Path.GetDirectoryName(ShellViewModel.SelectedModDataFolder), "data_noevent");
            if (!Directory.Exists(backupFolder))
            {
                MessageBox.Show("Backup folder not found!");
                return;
            }

            // Copy all files from backup to mod folder
            CopyAllFiles(backupFolder, ShellViewModel.SelectedModDataFolder);

            MessageBox.Show("You have left the Event!");
        }
        else
        {
            MessageBox.Show("You have not joined an Event yet!");
        }
    }

    // Recursive file copy method
    private void CopyAllFiles(string sourceDir, string targetDir)
    {
        foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            string subDir = dirPath.Replace(sourceDir, targetDir);
            if (!Directory.Exists(subDir))
                Directory.CreateDirectory(subDir);
        }

        foreach (string filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string destFile = filePath.Replace(sourceDir, targetDir);
            File.Copy(filePath, destFile, true); // overwrite existing files
        }
    }


    #endregion

    #region ---Helper Functions---

    static string FormatValues(List<string> values, string patternToRemove, string additionalPrefix = "")
    {
        List<string> formattedValues = new List<string>();
        foreach (string value in values)
        {
            string formattedValue = additionalPrefix + value.Replace(patternToRemove, "").Trim();
            formattedValues.Add(formattedValue);
        }
        return string.Join(" / ", formattedValues);
    }
    static string FormatValuesWithPercentage(List<string> values, string patternToRemove)
    {
        List<string> formattedValues = new List<string>();
        foreach (string value in values)
        {
            string formattedValue = value.Replace(patternToRemove, "").Trim() + "%";
            formattedValues.Add(formattedValue);
        }
        return string.Join(" / ", formattedValues);
    }
    static string ExtractString(string content, string pattern)
    {
        string regexPattern = Regex.Escape(pattern) + "(.*)";
        Match match = Regex.Match(content, regexPattern);
        return match.Success ? match.Groups[1].Value.Trim() : "Not found";
    }
    static List<string> ExtractStrings(string content, string pattern)
    {
        List<string> values = new List<string>();
        string regexPattern = Regex.Escape(pattern) + "(.*)";
        MatchCollection matches = Regex.Matches(content, regexPattern);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                values.Add(match.Groups[1].Value.Trim());
            }
            else
            {
                values.Add("Not found");
            }
        }

        return values;
    }
    private DateTime ConvertUtcToLocalTime(string utcTimeString)
    {
        DateTime utcDateTime = DateTime.ParseExact(utcTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        DateTime localDateTime = utcDateTime.ToLocalTime();

        return localDateTime;
    }
    public DateTime ConvertUtcToLocalTime2(DateTime utcTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
    }
    static async Task CopyDirectoryAndOverwrite(string sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        foreach (string filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string destFilePath = filePath.Replace(sourceDir, destDir);
            string destFileDir = Path.GetDirectoryName(destFilePath);

            if (!Directory.Exists(destFileDir))
                Directory.CreateDirectory(destFileDir);

            File.Copy(filePath, destFilePath, true);
        }

        foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            string destDirPath = dirPath.Replace(sourceDir, destDir);

            if (!Directory.Exists(destDirPath))
                Directory.CreateDirectory(destDirPath);
        }
    }
    public static void CloneDirectory(string sourceDirName, string destDirName)
    {
        if (Directory.Exists(destDirName))
            Directory.Delete(destDirName, true);

        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

        if (!Directory.Exists(destDirName))
            Directory.CreateDirectory(destDirName);

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = System.IO.Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (DirectoryInfo subdir in dirs)
        {
            string tempPath = System.IO.Path.Combine(destDirName, subdir.Name);
            CloneDirectory(subdir.FullName, tempPath);
        }
    }

    #endregion
}