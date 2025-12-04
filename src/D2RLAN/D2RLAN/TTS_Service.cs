using D2RLAN.ViewModels;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using System;
using Windows.Media.SpeechSynthesis;
using System.Windows.Media;
using Caliburn.Micro;
using D2RLAN.ViewModels.Drawers;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using Windows.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace D2RLAN
{
    class TTS_Service
    {
        private static SpeechSynthesizer _tts;
        private static bool _running = false;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(HomeDrawerViewModel));
        private static string _soundPath;
        private static int _selectedVoiceIndex = 0;
        private static VoiceInformation[] _allVoices;
        private static string _configPath;

        // Keep all MediaPlayers alive until finished
        private static readonly List<MediaPlayer> _activePlayers = new List<MediaPlayer>();

        public static void Start(ShellViewModel shellViewModel)
        {
            if (_running) return;

            _tts = new SpeechSynthesizer();

            // Absolute config path
            _configPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\D2R\lootfilter_config.lua"));
            _soundPath = Path.Combine(shellViewModel.SelectedModDataFolder, "D2RLAN", "Filters", "Sounds");

            // Load voices
            _allVoices = SpeechSynthesizer.AllVoices.ToArray();

            if (_allVoices.Length == 0)
                _logger.Warn("[TTS] No OneCore voices detected. Install Windows voices.");

            ListVoices();
            LoadVoiceFromConfig();

            Task.Run(() => ListenPipe());

            _running = true;
            _logger.Info("[TTS] Service started.");
            _logger.Info($"[TTS] Sound folder: {_soundPath}");
            _logger.Info($"[TTS] Config file: {_configPath}");
            _logger.Info("[TTS] Process ID: " + Environment.ProcessId);
        }

        private static void ListVoices()
        {
            try
            {
                _logger.Info("[TTS] Installed Voices:");
                for (int i = 0; i < _allVoices.Length; i++)
                {
                    string lang = _allVoices[i].Language;
                    try { lang = new Language(lang).DisplayName; } catch { }

                    _logger.Info($"   [{i}] {_allVoices[i].DisplayName} ({lang})");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("[TTS] Failed to list voices", ex);
            }
        }

        private static void SetVoiceByIndex(int index)
        {
            if (_allVoices.Length == 0)
            {
                _logger.Warn("[TTS] Cannot set voice (no voices installed)");
                return;
            }

            if (index < 0 || index >= _allVoices.Length)
            {
                _logger.Warn($"[TTS] Voice index {index} out of range. Forcing index 0.");
                index = 0;
            }

            _selectedVoiceIndex = index;
            _tts.Voice = _allVoices[_selectedVoiceIndex];

            _logger.Info($"[TTS] Voice set → [{_selectedVoiceIndex}] {_tts.Voice.DisplayName}");
        }

        private static void LoadVoiceFromConfig()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _logger.Warn($"[TTS] Config not found: {_configPath}");
                    return;
                }

                foreach (var line in File.ReadAllLines(_configPath))
                {
                    if (line.Trim().StartsWith("audioVoice"))
                    {
                        var parts = line.Split('=');
                        if (parts.Length >= 2 &&
                            int.TryParse(parts[1].Trim().TrimEnd(','), out int index))
                        {
                            _logger.Info($"[TTS] Found audioVoice line: {line.Trim()}");
                            _logger.Info($"[TTS] Config voice index = {index}");
                            SetVoiceByIndex(index);
                            return;
                        }
                    }
                }

                _logger.Warn("[TTS] audioVoice not found; using voice 0");
                SetVoiceByIndex(0);
            }
            catch (Exception ex)
            {
                _logger.Error("[TTS] Failed reading config", ex);
            }
        }

        private static async Task SpeakAsync(string text)
        {
            try
            {
                _logger.Info($"[TTS] Synthesize request: \"{text}\"");

                using var stream = await _tts.SynthesizeTextToStreamAsync(text);
                string tempFile = Path.Combine(Path.GetTempPath(), $"tts_{Guid.NewGuid()}.wav");
                _logger.Info($"[TTS] Writing temp WAV: {tempFile}");

                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    await stream.AsStreamForRead().CopyToAsync(fs);

                var player = new MediaPlayer();
                _activePlayers.Add(player);

                player.Open(new Uri(tempFile));
                player.Volume = 1.0;

                player.MediaEnded += (s, e) =>
                {
                    player.Close();
                    _activePlayers.Remove(player);
                    _logger.Info($"[TTS] Completed playing: {text}");
                };

                player.Play();
                _logger.Info($"[TTS] Started speaking '{text}' using '{_tts.Voice.DisplayName}'");
            }
            catch (Exception ex)
            {
                _logger.Error("[TTS] SpeakAsync error", ex);
            }
        }

        private static void PlaySound(string file)
        {
            try
            {
                string path = Path.Combine(_soundPath, file);
                if (!File.Exists(path))
                {
                    _logger.Warn($"[TTS] Sound not found: {path}");
                    return;
                }

                var player = new MediaPlayer();
                _activePlayers.Add(player);

                player.Open(new Uri(path));
                player.Volume = 1.0;

                player.MediaEnded += (s, e) =>
                {
                    player.Close();
                    _activePlayers.Remove(player);
                    _logger.Info($"[TTS] Completed playing file: {file}");
                };

                player.Play();
                _logger.Info($"[TTS] Playing file: {file}");
            }
            catch (Exception ex)
            {
                _logger.Error("[TTS] Failed playing sound", ex);
            }
        }

        private static void ListenPipe()
        {
            _logger.Info("[TTS] ListenPipe thread STARTED");

            while (true)
            {
                try
                {
                    _logger.Info("[TTS] Waiting for pipe connection...");

                    using var pipe = new NamedPipeServerStream("D2RLAN_TTS", PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    pipe.WaitForConnection();

                    _logger.Info("[TTS] PIPE CONNECTED.");

                    using var reader = new StreamReader(pipe);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        _logger.Info($"[PIPE] Received: {line}");

                        if (line.StartsWith("SAY:"))
                        {
                            string text = line.Substring(4).Trim();
                            Task.Run(() => SpeakAsync(text));
                        }
                        else if (line.StartsWith("PLAY:"))
                        {
                            string file = line.Substring(5).Trim();
                            PlaySound(file);
                        }
                        else if (line.Trim().Equals("VOICE_RELOAD", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.Info("[TTS] VOICE_RELOAD command received");
                            LoadVoiceFromConfig();
                        }
                    }

                    _logger.Warn("[TTS] Pipe disconnected (null read).");
                }
                catch (Exception ex)
                {
                    _logger.Error("[TTS] Pipe error", ex);
                    Task.Delay(100).Wait();
                }
            }
        }
    }
}
