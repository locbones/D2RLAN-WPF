using D2RLAN.ViewModels;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using System;
using Windows.Media.SpeechSynthesis;
using System.Windows.Media; // MediaPlayer
using Caliburn.Micro;
using D2RLAN.ViewModels.Drawers;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using Windows.Globalization;
using System.Linq;

namespace D2RLAN
{
    class TTS_Service
    {
        private static SpeechSynthesizer _tts;
        private static bool _running = false;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
        private static string _soundPath;
        private static int _selectedVoiceIndex = 0; // default to first voice
        private static VoiceInformation[] _allVoices;

        public static void Start(ShellViewModel shellViewModel)
        {
            if (_running) return;

            _tts = new SpeechSynthesizer();
            _soundPath = Path.Combine(shellViewModel.SelectedModDataFolder, "D2RLAN", "Filters", "Sounds");

            // Load voices
            _allVoices = SpeechSynthesizer.AllVoices.ToArray();

            if (_allVoices.Length == 0)
            {
                _logger.Warn("[TTSService] No OneCore voices detected. Make sure Windows voices are installed in Settings -> Time & Language -> Speech.");
            }

            // List voices for logging
            ListVoices();

            // Load selected voice index from lootfilter_config.lua
            string configPath = "../D2R/lootfilter_config.lua";
            LoadVoiceFromConfig(configPath);

            // Start pipe listener
            Task.Run(() => ListenPipe());

            _running = true;
            _logger.Info("[TTSService] Started.");
            _logger.Info($"[TTSService] Sounds folder: {_soundPath}");
        }

        // List installed voices
        private static void ListVoices()
        {
            try
            {
                _logger.Info("[TTSService] Available voices:");
                for (int i = 0; i < _allVoices.Length; i++)
                {
                    var v = _allVoices[i];
                    string lang = v.Language ?? "unknown";

                    try
                    {
                        var langObj = new Language(v.Language);
                        lang = langObj.DisplayName;
                    }
                    catch { }

                    _logger.Info($"  [{i}] {v.DisplayName} ({lang})");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("[TTSService] Failed to list voices.", ex);
            }
        }

        // Set voice by index
        private static void SetVoiceByIndex(int index)
        {
            if (_allVoices == null || _allVoices.Length == 0)
            {
                _logger.Warn("[TTSService] No voices available to set.");
                return;
            }

            if (index < 0 || index >= _allVoices.Length)
            {
                _logger.Warn($"[TTSService] Voice index {index} is out of range. Using default voice 0.");
                index = 0;
            }

            _selectedVoiceIndex = index;
            _tts.Voice = _allVoices[_selectedVoiceIndex];
            _logger.Info($"[TTSService] Voice set to index {_selectedVoiceIndex}: {_tts.Voice.DisplayName}");
        }

        // Load audioVoice from lootfilter_config.lua
        private static void LoadVoiceFromConfig(string luaFilePath)
        {
            try
            {
                if (!File.Exists(luaFilePath))
                {
                    _logger.Warn($"[TTSService] Config file not found: {luaFilePath}");
                    return;
                }

                string[] lines = File.ReadAllLines(luaFilePath);

                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("audioVoice"))
                    {
                        var parts = line.Split('=');
                        if (parts.Length >= 2 && int.TryParse(parts[1].Trim().TrimEnd(','), out int index))
                        {
                            SetVoiceByIndex(index);
                            return;
                        }
                    }
                }

                _logger.Warn("[TTSService] audioVoice entry not found in config. Using default voice 0.");
            }
            catch (Exception ex)
            {
                _logger.Error("[TTSService] Failed to load voice from config.", ex);
            }
        }

        // Speak text async using temporary WAV
        private static async Task SpeakAsync(string text)
        {
            try
            {
                using var stream = await _tts.SynthesizeTextToStreamAsync(text);
                string tempFile = Path.Combine(Path.GetTempPath(), $"tts_{Guid.NewGuid()}.wav");

                using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    await stream.AsStreamForRead().CopyToAsync(fileStream);
                }

                var player = new MediaPlayer();
                player.Open(new Uri(tempFile));
                player.Play();

                _logger.Info($"[TTSService] Speaking '{text}' with voice '{_tts.Voice.DisplayName}'");
            }
            catch (Exception ex)
            {
                _logger.Error("[TTSService] Failed to speak text.", ex);
            }
        }

        // Listen for pipe commands
        private static void ListenPipe()
        {
            while (true)
            {
                try
                {
                    using (var pipe = new NamedPipeServerStream("D2RLAN_TTS", PipeDirection.In))
                    {
                        pipe.WaitForConnection();
                        using (var reader = new StreamReader(pipe))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (string.IsNullOrWhiteSpace(line)) continue;

                                if (line.StartsWith("SAY:"))
                                {
                                    string text = line.Substring(4).Trim();
                                    Task.Run(() => SpeakAsync(text));
                                }
                                else if (line.StartsWith("PLAY:"))
                                {
                                    string file = line.Substring(5).Trim();
                                    string path = Path.Combine(_soundPath, file);

                                    if (File.Exists(path))
                                    {
                                        try
                                        {
                                            var player = new MediaPlayer();
                                            player.Open(new Uri(path));
                                            player.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error($"[TTSService] Failed to play file: {path}", ex);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Info($"[TTSService] File not found: {path}");
                                    }
                                }
                                else if (line.Trim().Equals("VOICE_RELOAD", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Reload the voice dynamically
                                    string configPath = Path.Combine("../D2R/lootfilter_config.lua");
                                    LoadVoiceFromConfig(configPath);
                                    _logger.Info("[TTSService] Voice reloaded via pipe command.");
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Task.Delay(50).Wait();
                }
            }
        }

    }
}
