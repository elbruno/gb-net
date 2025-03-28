using GB.AI.Ollama;
using GB.Core.Controller;
using GB.Core.Gui;
using GB.Core.Sound;
using GB.WinForms.OsSpecific;
using System.Diagnostics;
using System.Windows.Forms;
using Button = GB.Core.Controller.Button;

namespace GB.WinForms
{
    public partial class MainForm : Form, IController
    {
        private IButtonListener? _listener;

        private readonly Dictionary<Keys, Button> _controls;
        private CancellationTokenSource _cancellation;

        // private readonly ISoundOutput _soundOutput = new NullSoundOutput();
        private readonly ISoundOutput _soundOutput = new SoundOutput();
        private readonly Emulator _emulator;

        // create an instance of the AI Action Generator
        private readonly ActionGenerator actionGenerator = new ActionGenerator();
        private string currentFrameAnalysis = string.Empty;
        private CancellationTokenSource? _aiLoopCancellation;

        public MainForm()
        {
            InitializeComponent();
            autodetectToolStripMenuItem.Tag = GameBoyMode.AutoDetect;
            dMGClassicToolStripMenuItem.Tag = GameBoyMode.DMG;
            gameBoyColorToolStripMenuItem.Tag = GameBoyMode.Color;

            _emulator = new Emulator
            {
                Display = _display,
                SoundOutput = _soundOutput
            };

            _controls = new Dictionary<Keys, Button>
            {
                {Keys.A, Button.Left},
                {Keys.D, Button.Right},
                {Keys.W, Button.Up},
                {Keys.S, Button.Down},
                {Keys.K, Button.A},
                {Keys.O, Button.B},
                {Keys.Enter, Button.Start},
                {Keys.Back, Button.Select}
            };

            _cancellation = new CancellationTokenSource();

            _emulator.Controller = this;

            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Closed += (_, e) =>
            {
                _cancellation.Cancel();
                _aiLoopCancellation?.Cancel();
            };
        }

        public void SetButtonListener(IButtonListener listener) => _listener = listener;

        private void StopEmulator()
        {
            if (!_emulator.Active)
            {
                return;
            }

            _emulator.Stop(_cancellation);
            _soundOutput.Stop();

            _cancellation = new CancellationTokenSource();
            _display.DisplayEnabled = false;
            Thread.Sleep(100);
        }

        private void OpenRom(object sender, EventArgs e)
        {
            StopEmulator();

            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Gameboy ROM (*.gb;*.gbc)|*.gb;*.gbc| All files(*.*) |*.*",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            var (success, romPath) = openFileDialog.ShowDialog() == DialogResult.OK
                ? (true, openFileDialog.FileName)
                : (false, null);

            if (success && !string.IsNullOrEmpty(romPath))
            {
                _emulator.EnableBootRom = enableBootROMToolStripMenuItem.Checked;
                _emulator.RomPath = romPath;
                _emulator.Run(_cancellation.Token);
            }
        }

        private void TogglePause(object sender, EventArgs e)
        {
            _emulator.TogglePause();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            var button = _controls.ContainsKey(e.KeyCode) ? _controls[e.KeyCode] : null;
            if (button != null)
            {
                _listener?.OnButtonPress(button);
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            var button = _controls.ContainsKey(e.KeyCode) ? _controls[e.KeyCode] : null;
            if (button != null)
            {
                _listener?.OnButtonRelease(button);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopEmulator();

            base.OnFormClosing(e);
        }

        private void ToggleSoundChannel(object sender, EventArgs e)
        {
            var channel = 0;
            if (sender == _toggleChannel1)
            {
                channel = 1;
            }
            else if (sender == _toggleChannel2)
            {
                channel = 2;
            }
            else if (sender == _toggleChannel3)
            {
                channel = 3;
            }
            else if (sender == _toggleChannel4)
            {
                channel = 4;
            }
            if (channel is < 1 or > 4)
            {
                return;
            }

            _emulator.ToggleSoundChannel(channel);
        }

        private void GameBoyModeClicked(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedToolStripMenuItem)
            {
                return;
            }

            if (clickedToolStripMenuItem.CheckState == CheckState.Checked)
            {
                // Don't act if the user clicks on an already checked menu item
                return;
            }

            // Reset all three
            autodetectToolStripMenuItem.CheckState = CheckState.Unchecked;
            dMGClassicToolStripMenuItem.CheckState = CheckState.Unchecked;
            gameBoyColorToolStripMenuItem.CheckState = CheckState.Unchecked;

            // Check the sender
            clickedToolStripMenuItem.CheckState = CheckState.Checked;

            _emulator.GameBoyMode = (GameBoyMode)clickedToolStripMenuItem.Tag;
        }

        // AI Stuff
        private async Task ProcessNextAiActionAsync()
        {
            try
            {
                _emulator.TogglePause();

                // save the _display content to a local image
                using var bitmap = new Bitmap(_display.Width, _display.Height);
                _display.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                string imageLocation = CapturesManager.SaveScreenCapture(bitmap);

                // use the AI Action Generator to generate the next action
                var actionResponse = await actionGenerator.GenerateNextActionResponse(imageLocation, currentFrameAnalysis);
                AddLog(actionResponse);

                // delete the created file
                if (File.Exists(imageLocation))
                {
                    File.Delete(imageLocation);
                }

                // get the PressKey from the actionResponse
                var pressKey = actionResponse.PressKey;
                currentFrameAnalysis = actionResponse.FrameAnalysis;
                Keys key = ParseKeyString(pressKey);

                var button = _controls.ContainsKey(key) ? _controls[key] : null;
                if (button != null)
                {
                    // fake fire
                    _emulator.TogglePause();
                    _listener?.OnButtonPress(_controls[Keys.K]);
                    await Task.Delay(100);
                    _listener?.OnButtonRelease(_controls[Keys.K]);
                    _emulator.TogglePause();

                    // perform move
                    _listener?.OnButtonPress(button);
                    _emulator.TogglePause();
                    await Task.Delay(1000);
                    _emulator.TogglePause();
                    _listener?.OnButtonRelease(button);

                    _emulator.TogglePause();
                    _listener?.OnButtonPress(_controls[Keys.K]);
                    await Task.Delay(100);
                    _listener?.OnButtonRelease(_controls[Keys.K]);
                    _emulator.TogglePause();
                }
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
            finally
            {
                _emulator.TogglePause();
            }
        }

        private void startToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            // Cancel any existing AI loop
            _aiLoopCancellation?.Cancel();

            // Create a new cancellation token source for the loop
            _aiLoopCancellation = new CancellationTokenSource();

            try
            {
                // Start the AI loop
                RunAiLoopAsync(_aiLoopCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                // Loop was cancelled, this is expected
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in AI loop: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RunAiLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessNextAiActionAsync();
                // Wait 0.05 second before the next action
                await Task.Delay(50, cancellationToken);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cancel the AI loop if it's running
            _aiLoopCancellation?.Cancel();
            _aiLoopCancellation = null;
        }

        void AddLog(ActionResponse actionResponse)
        {
            string message = @$"Key: {actionResponse.PressKey}|{actionResponse.SuggestedMove}{Environment.NewLine}{actionResponse.FrameAnalysis}";
            AddLog(message);
        }

        void AddLog(string message)
        {
            var logMessage = $"{DateTime.Now:HH:mm:ss}: {message}";
            textBoxLog.Text = logMessage + Environment.NewLine + textBoxLog.Text;
            Debug.WriteLine(logMessage);
        }

        /// <summary>
        /// Converts a string representation of a key (e.g., "Keys.A") to the corresponding Keys enum value.
        /// </summary>
        /// <param name="keyString">String representation of the key (e.g., "Keys.A", "Keys.Enter")</param>
        /// <returns>The corresponding Keys enum value if successful, null if parsing fails</returns>
        private Keys ParseKeyString(string keyString)
        {
            var defaultValue = Keys.Back; // Default to Back key if empty

            if (string.IsNullOrEmpty(keyString))
            {
                AddLog("Empty key string provided");
                return defaultValue;
            }

            try
            {
                // If the string starts with "Keys.", remove that prefix
                if (keyString.StartsWith("Keys.", StringComparison.OrdinalIgnoreCase))
                {
                    keyString = keyString.Substring(5); // "Keys.".Length == 5
                }

                // Try to parse the key name to get the enum value
                if (Enum.TryParse<Keys>(keyString, true, out var key))
                {
                    return key;
                }

                AddLog($"Failed to parse key: {keyString}");
                return defaultValue;
            }
            catch (Exception ex)
            {
                AddLog($"Error parsing key string: {ex.Message}");
                return defaultValue;

            }
        }


    }
}