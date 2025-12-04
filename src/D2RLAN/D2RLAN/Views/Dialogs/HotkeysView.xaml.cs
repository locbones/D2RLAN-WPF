using D2RLAN.Models;
using D2RLAN.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering logic for StashTabSettingsView.xaml
    /// </summary>
    ///




    public partial class HotkeysView : Window
    {

        #region Imports
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region Constants
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int MEM_COMMIT = 0x1000;
        const int PAGE_EXECUTE_READWRITE = 0x40;

        public static bool debugLogging = false;
        #endregion

        #region Config Classes
        public class MemoryConfig
        {
            public string Description { get; set; }
            public string Address { get; set; }
            public List<string> Addresses { get; set; }
            public int Length { get; set; }
            public string Values { get; set; }
        }

        public class Config
        {
            public bool MonsterStatsDisplay { get; set; }
            public List<MemoryConfig> MemoryConfigs { get; set; }
        }
        #endregion


        public HotkeysView()
        {
            InitializeComponent();
            LoadKeybinds();
            Loaded += HotkeysView_Loaded;
        }

        private void HotkeysView_Loaded(object sender, RoutedEventArgs e)
        {
            // Verify owner window and then center this window relative to it
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - Width) / 2;
                Top = Owner.Top + (Owner.Height - Height) / 2;
            }    
        }

        private async void LoadKeybinds()
        {
            var mainWindow = Window.GetWindow(this);
            var shellViewModel = mainWindow.DataContext as ShellViewModel;
            string filePath = "D2RLAN_Config.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                await File.WriteAllBytesAsync(filePath, await Helper.GetResourceByteArray("CASC.D2RLAN_Config.txt"));
            }
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
        }

        private void kbBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not System.Windows.Controls.TextBox tb) return;

            string filePath = "D2RLAN_Config.txt";
            var lines = File.ReadAllLines(filePath).ToList();

            // Extract numeric index from the TextBox name
            if (!tb.Name.StartsWith("kbBox") ||
                !int.TryParse(tb.Name.Substring(5), out int textBoxIndex))
                return;

            textBoxIndex--; // convert 1-based → 0-based
            if (textBoxIndex < 0 || textBoxIndex >= lines.Count) return;

            tb.Clear(); // Always clear so user sees fresh captured keys

            // ===============================================================
            //  CLEAR HOTKEY (DELETE KEY)
            // ===============================================================
            if (e.Key == Key.Delete)
            {
                ApplyDeleteReplacement(lines, textBoxIndex);
                tb.Text = "NaN";
                File.WriteAllLines(filePath, lines);
                e.Handled = true;
                return;
            }

            // ===============================================================
            //  BUILD KEY + MODIFIERS → "VK_CTRL + VK_SHIFT + VK_X"
            // ===============================================================
            var mods = new List<string>();
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) mods.Add("CTRL");
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) mods.Add("ALT");
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) mods.Add("SHIFT");
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows)) mods.Add("WIN");

            string mainKey = e.Key switch
            {
                Key.Insert => "INSERT",
                Key.Delete => "DELETE",
                Key.Home => "HOME",
                Key.PageUp => "PRIOR",
                Key.PageDown => "NEXT",
                Key.End => "END",
                _ => e.Key.ToString().ToUpper(),
            };

            string vkCode = "VK_" + string.Join(" + VK_", mods.Append(mainKey));

            // ===============================================================
            //  APPLY REPLACEMENT BACK INTO LINES[]
            // ===============================================================
            ApplyKeyReplacement(lines, textBoxIndex, vkCode);

            tb.Text = vkCode;
            tb.SelectionStart = tb.Text.Length;

            File.WriteAllLines(filePath, lines);
            e.Handled = true;
        }


        private void kbBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not System.Windows.Controls.TextBox tb)
                return;

            string filePath = "D2RLAN_Config.txt";
            var lines = File.ReadAllLines(filePath).ToList();

            // ------------------------------------------------------
            //  Detect which TextBox this is
            // ------------------------------------------------------
            int? customIndex = tb.Name switch
            {
                "kbBoxC7" => 6,
                "kbBoxC8" => 7,
                "kbBoxC9" => 8,
                "kbBoxC10" => 9,
                "kbBoxC11" => 10,
                "kbBoxC12" => 11,
                _ => null
            };

            string newText = tb.Text.Trim();

            // ------------------------------------------------------
            //  CASE A: Custom Command TextBoxes (C7 – C12)
            // ------------------------------------------------------
            if (customIndex is int idx)
            {
                if (idx < 0 || idx >= lines.Count)
                    return;

                var line = lines[idx];
                var parts = line.Split(':', 2);
                if (parts.Length != 2)
                    return;

                string left = parts[0].Trim(); // "Custom Command X"
                string right = parts[1].Trim();

                // Right-hand side: "VK_..., commandName"
                var commaParts = right.Split(',', 2);
                if (commaParts.Length != 2)
                    return;

                string vkPart = commaParts[0].Trim();     // VK_XXXX
                string currentCmd = commaParts[1].Trim(); // "old command"

                // Only update if different
                if (!currentCmd.Equals($"\"{newText}\"", StringComparison.Ordinal))
                {
                    lines[idx] = $"{left}: {vkPart}, \"{newText}\"";
                }
            }
            else
            {
                // ------------------------------------------------------
                //  CASE B: Startup Commands TextBox (any other TextBox)
                // ------------------------------------------------------

                // remove previous "Startup Commands:" line (if any)
                lines.RemoveAll(l => l.StartsWith("Startup Commands:", StringComparison.OrdinalIgnoreCase));

                // append updated one (no quotes)
                lines.Add($"Startup Commands: {newText}");
            }

            File.WriteAllLines(filePath, lines);
        }


        private static void ApplyDeleteReplacement(List<string> lines, int index)
        {
            string line = lines[index];
            var split = line.Split(':', 2);
            if (split.Length < 2) return;

            string left = split[0].Trim();
            string right = split[1].Trim();

            // BASIC ENTRIES
            if (index <= 5 || index == 18 || (index >= 12 && index <= 16))
            {
                lines[index] = $"{left}: NaN";
                return;
            }

            // CUSTOM COMMANDS (key, commandText)
            if (index >= 6 && index <= 11)
            {
                var cmdSplit = right.Split(',', 2);
                string cmd = cmdSplit.Length > 1 ? cmdSplit[1].Trim() : "";
                lines[index] = $"{left}: NaN, {cmd}";
                return;
            }

            // TOGGLE STAT PANEL (bool, key)
            if (index == 17)
            {
                int comma = right.IndexOf(',');
                if (comma > 0)
                {
                    string boolPart = right[..comma].Trim();
                    lines[index] = $"{left}: {boolPart}, NaN";
                }
            }
        }

        private static void ApplyKeyReplacement(List<string> lines, int index, string vkCode)
        {
            string line = lines[index];
            var split = line.Split(':', 2);
            if (split.Length < 2) return;

            string left = split[0].Trim();
            string right = split[1].Trim();

            // BASIC VK ENTRIES
            if (index <= 5 || index == 18 || (index >= 12 && index <= 16))
            {
                lines[index] = $"{left}: {vkCode}";
                return;
            }

            // CUSTOM COMMANDS
            if (index >= 6 && index <= 11)
            {
                var cmdSplit = right.Split(',', 2);
                string cmd = cmdSplit.Length > 1 ? cmdSplit[1].Trim() : "";
                lines[index] = $"{left}: {vkCode}, {cmd}";
                return;
            }

            // TOGGLE STAT ADJUSTMENT PANEL
            if (index == 17)
            {
                int comma = right.IndexOf(',');
                if (comma > 0)
                {
                    string boolPart = right[..comma].Trim();
                    lines[index] = $"{left}: {boolPart}, {vkCode}";
                }
            }
        }


    }
}
