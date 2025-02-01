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
            var currentTextBox = sender as System.Windows.Controls.TextBox;
            if (currentTextBox == null) return;

            string filePath = "D2RLAN_Config.txt";
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
            int textBoxIndex = -1;

            if (currentTextBox.Name == "kbBox1") textBoxIndex = 0;
            else if (currentTextBox.Name == "kbBox2") textBoxIndex = 1;
            else if (currentTextBox.Name == "kbBox3") textBoxIndex = 2;
            else if (currentTextBox.Name == "kbBox4") textBoxIndex = 3;
            else if (currentTextBox.Name == "kbBox5") textBoxIndex = 4;
            else if (currentTextBox.Name == "kbBox6") textBoxIndex = 5;
            else if (currentTextBox.Name == "kbBox7") textBoxIndex = 6;
            else if (currentTextBox.Name == "kbBox8") textBoxIndex = 7;
            else if (currentTextBox.Name == "kbBox9") textBoxIndex = 8;
            else if (currentTextBox.Name == "kbBox10") textBoxIndex = 9;
            else if (currentTextBox.Name == "kbBox11") textBoxIndex = 10;
            else if (currentTextBox.Name == "kbBox12") textBoxIndex = 11;

            if (textBoxIndex == -1) return;

            string virtualKeyCode = "VK_" + e.Key.ToString().ToUpper();
            int virtualKey = KeyInterop.VirtualKeyFromKey(e.Key);

            if (virtualKey == 0x2D)
            {
                virtualKeyCode = "VK_INSERT";
            }
            else if (virtualKey == 0x2E)
            {
                virtualKeyCode = "VK_DELETE";
            }

            switch (e.Key)
            {
                case Key.Home:
                    virtualKeyCode = "VK_HOME";
                    break;
                case Key.PageUp:
                    virtualKeyCode = "VK_PRIOR";
                    break;
                case Key.PageDown:
                    virtualKeyCode = "VK_NEXT";
                    break;
                case Key.End:
                    virtualKeyCode = "VK_END";
                    break;
            }

            if (textBoxIndex <= 5)
            {
                lines[textBoxIndex] = $"{lines[textBoxIndex].Split(':')[0]}: {virtualKeyCode}";
                currentTextBox.Text = virtualKeyCode;
                currentTextBox.SelectionStart = currentTextBox.Text.Length;
            }
            else // For custom commands (index 6 and above)
            {
                var existingLine = lines[textBoxIndex];
                var parts = existingLine.Split(new[] { ':' }, 2);

                if (parts.Length > 1)
                {
                    var commandParts = parts[1].Split(new[] { ',' }, 2);
                    string commandId = parts[0].Trim();
                    string commandName = commandParts.Length > 1 ? commandParts[1].Trim() : "";

                    lines[textBoxIndex] = $"{commandId}: {virtualKeyCode}, {commandName}";
                    currentTextBox.Text = virtualKeyCode;
                    currentTextBox.SelectionStart = currentTextBox.Text.Length;
                }
            }

            e.Handled = true;
            File.WriteAllLines(filePath, lines);
        }

        private void kbBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentTextBox = sender as System.Windows.Controls.TextBox;
            if (currentTextBox == null) return;

            string filePath = "D2RLAN_Config.txt";
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
            int textBoxIndex = -1;

            // Determine the index in the file based on the TextBox name
            if (currentTextBox.Name == "kbBoxC7") textBoxIndex = 6;
            else if (currentTextBox.Name == "kbBoxC8") textBoxIndex = 7;
            else if (currentTextBox.Name == "kbBoxC9") textBoxIndex = 8;
            else if (currentTextBox.Name == "kbBoxC10") textBoxIndex = 9;
            else if (currentTextBox.Name == "kbBoxC11") textBoxIndex = 10;
            else if (currentTextBox.Name == "kbBoxC12") textBoxIndex = 11;

            if (textBoxIndex != -1)
            {
                // Update the specific command entry with quotes around newCommandName
                var existingLine = lines[textBoxIndex];
                var parts = existingLine.Split(new[] { ':' }, 2);

                if (parts.Length > 1)
                {
                    string commandId = parts[0].Trim();
                    string newCommandName = currentTextBox.Text.Trim();

                    // Only update if the new command name differs
                    if (!existingLine.Contains(newCommandName))
                        lines[textBoxIndex] = $"{commandId}: {parts[1].Split(',')[0].Trim()}, \"{newCommandName}\"";
                }
            }
            else
            {
                // Update the Startup Commands line without adding quotes around newCommands
                string newCommands = currentTextBox.Text.Trim();

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("Startup Commands:"))
                    {
                        lines.RemoveAt(i);
                        break;
                    }
                }
                lines.Add($"Startup Commands: {newCommands}");
            }

            // Write updated lines back to the file
            File.WriteAllLines(filePath, lines);
        }


    }
}
