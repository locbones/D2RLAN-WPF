using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using D2RLAN.ViewModels;
using D2RLAN.Models;
using D2RLAN.ViewModels.Drawers;
using Syncfusion.Windows.Tools.Controls;
using System.Diagnostics;

namespace D2RLAN.Views.Drawers
{
    /// <summary>
    /// Interaction logic for QoLOptionsDrawerView.xaml
    /// </summary>
    public partial class QoLOptionsDrawerView : System.Windows.Controls.UserControl
    {
        public QoLOptionsDrawerView()
        {
            InitializeComponent();
            LoadLastColors();
        }
        private void ColorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, int matchCount)
        {
            if (e.NewValue is Color chosenColor)
            {
                byte redValue = chosenColor.R;
                byte greenValue = chosenColor.G;
                byte blueValue = chosenColor.B;

                double Normalize(byte value) => Math.Round(value / 2.55 * 0.01, 6);
                string Format(double val) => val.ToString("0.0######");

                var mainWindow = Window.GetWindow(this);

                if (mainWindow != null)
                {
                    var shellViewModel = mainWindow.DataContext as ShellViewModel;

                    if (shellViewModel != null)
                    {
                        string folderPath = System.IO.Path.Combine(shellViewModel.SelectedModDataFolder, "D2RLAN", "Monster Stats");
                        string filePath = "MyColorSettings.txt";

                        Directory.CreateDirectory(folderPath);
                        List<string> lines = new List<string>();

                        if (!File.Exists(filePath))
                            File.WriteAllText(filePath, "Red: 80 Green: 0 Blue: 0\nRed: 80 Green: 0 Blue: 0\nRed: 80 Green: 0 Blue: 0");


                        lines.AddRange(File.ReadAllLines(filePath));

                        if (matchCount >= 0 && matchCount < lines.Count)
                            lines[matchCount] = $"Red: {chosenColor.R} Green: {chosenColor.G} Blue: {chosenColor.B}";
                        else
                        {
                            Console.WriteLine($"Invalid matchCount: {matchCount}");
                            return;
                        }
                        File.WriteAllLines(filePath, lines);

                        //Color Settings Updated - Now Updating Game Files
                        string jsonFilePath = System.IO.Path.Combine(shellViewModel.SelectedModDataFolder, "global", "ui", "layouts", "hudmonsterhealthhd.json");
                        string[] patterns = { @"""r""\s*:\s*-?\d*\.?\d*(?:[Ee][-+]?\d+)?", @"""g""\s*:\s*-?\d*\.?\d*(?:[Ee][-+]?\d+)?", @"""b""\s*:\s*-?\d*\.?\d*(?:[Ee][-+]?\d+)?" };
                        string[] replacements = { $"\"r\": {Format(Normalize(redValue))}", $"\"g\": {Format(Normalize(greenValue))}", $"\"b\": {Format(Normalize(blueValue))}" };

                        if (File.Exists(jsonFilePath))
                        {
                            string fileContents = File.ReadAllText(jsonFilePath);
                            string updatedContents = fileContents;

                            for (int i = 0; i < patterns.Length; i++)
                            {
                                Match match = Regex.Match(updatedContents, patterns[i]);
                                for (int j = 0; j < matchCount; j++)
                                {
                                    match = match.NextMatch();
                                    if (!match.Success)
                                    {
                                        Console.WriteLine($"Not enough matches found for the pattern {patterns[i]}.");
                                        break;
                                    }
                                }
                                if (match.Success)
                                    updatedContents = updatedContents.Remove(match.Index, match.Length).Insert(match.Index, replacements[i]);
                                else
                                    Console.WriteLine($"No match found for the pattern {patterns[i]}.");
                            }
                            File.WriteAllText(jsonFilePath, updatedContents);
                        }
                    }
                }
            }
        }

        private void colorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker_ColorChanged(d, e, 0);
        }

        private void colorPicker2_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker_ColorChanged(d, e, 1);
        }

        private void colorPicker3_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker_ColorChanged(d, e, 2);
        }

        private void LoadLastColors()
        {
            if (File.Exists("MyColorSettings.txt"))
            {
                string[] colorLines = File.ReadAllLines("MyColorSettings.txt");
                string pattern = @"\d+";

                MatchCollection matches1 = Regex.Matches(colorLines[0], pattern);
                MatchCollection matches2 = Regex.Matches(colorLines[1], pattern);
                MatchCollection matches3 = Regex.Matches(colorLines[2], pattern);

                SetColorNormal(Convert.ToByte(matches1[0].Value), Convert.ToByte(matches1[1].Value), Convert.ToByte(matches1[2].Value));
                SetColorWarning(Convert.ToByte(matches2[0].Value), Convert.ToByte(matches2[1].Value), Convert.ToByte(matches2[2].Value));
                SetColorCritical(Convert.ToByte(matches3[0].Value), Convert.ToByte(matches3[1].Value), Convert.ToByte(matches3[2].Value));
            }
        }

        private void SetColorNormal(byte red, byte green, byte blue)
        {
            Color newColor = Color.FromRgb(red, green, blue);
            colorPicker.Color = newColor;
        }
        private void SetColorWarning(byte red, byte green, byte blue)
        {
            Color newColor = Color.FromRgb(red, green, blue);
            colorPicker2.Color = newColor;
        }
        private void SetColorCritical(byte red, byte green, byte blue)
        {
            Color newColor = Color.FromRgb(red, green, blue);
            colorPicker3.Color = newColor;
        }

        private async void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            var shellViewModel = mainWindow.DataContext as ShellViewModel;

            string filePath = System.IO.Path.Combine(shellViewModel.GamePath, "D2RHUD_Config.txt");
            string dllPath = System.IO.Path.Combine(shellViewModel.GamePath, "D2RHUD.dll");

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                await File.WriteAllBytesAsync(filePath, await Helper.GetResourceByteArray("Options.MonsterStats.D2RHUD_Config.txt"));
            }

            if (!File.Exists(dllPath))
            {
                File.Create(dllPath).Close();
                await File.WriteAllBytesAsync(dllPath, await Helper.GetResourceByteArray("Options.MonsterStats.D2RHUD.dll"));
            }

            List<string> lines = new List<string>(File.ReadAllLines(filePath));

            // Determine the index of the current TextBox
            int textBoxIndex = -1;
            System.Windows.Controls.TextBox currentTextBox = sender as System.Windows.Controls.TextBox;

            if (currentTextBox != null)
            {
                if (currentTextBox.Name == "kbBox1")
                {
                    textBoxIndex = 1;
                }
                else if (currentTextBox.Name == "kbBox2")
                {
                    textBoxIndex = 2;
                }
                else if (currentTextBox.Name == "kbBox3")
                {
                    textBoxIndex = 3;
                }
                else if (currentTextBox.Name == "kbBox4")
                {
                    textBoxIndex =4;
                }
            }

            if (textBoxIndex != -1)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    int colonIndex = line.IndexOf(':');

                    if (colonIndex != -1 && colonIndex < line.Length - 1)
                    {
                        string key = line.Substring(0, colonIndex).Trim();

                        if (i == textBoxIndex)
                        {
                            string currentValue = line.Substring(colonIndex + 1).Trim();
                            string virtualKeyCode = "";

                            // Handle common keys
                            if (e.Key != Key.None)
                            {
                                virtualKeyCode = "VK_" + e.Key.ToString();
                            }
                            // Handle system keys
                            else if (e.SystemKey != Key.None)
                            {
                                virtualKeyCode = "VK_" + e.SystemKey.ToString();
                            }

                            // Add specific handling for keys like HOME, INSERT, PAGE UP, etc.
                            switch (e.Key)
                            {
                                case Key.Home:
                                    virtualKeyCode = "VK_HOME";
                                    break;
                                case Key.Insert:
                                    virtualKeyCode = "VK_INSERT";
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
                                case Key.Delete:
                                    virtualKeyCode = "VK_DELETE";
                                    break;
                                default:
                                    break;
                            }

                            currentValue = virtualKeyCode.ToString().ToUpper();
                            lines[i] = $"{key}: {currentValue}";
                            break;
                        }
                    }
                }

                File.WriteAllLines(filePath, lines);

                // Cancel the key press event
                e.Handled = true;

                // Update the TextBox value
                if (currentTextBox != null && lines.Count > textBoxIndex)
                {
                    string vkText = lines[textBoxIndex].Split(':')[1].Trim();
                    currentTextBox.Text = vkText;
                    currentTextBox.SelectionStart = currentTextBox.Text.Length;
                }
            }
        }


        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var shellViewModel = this.DataContext as ShellViewModel;
            //LoadKeybinds();
        }
    }
}
