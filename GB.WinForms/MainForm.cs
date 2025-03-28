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
        private string aiRecentActivity = string.Empty;
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
        private async Task<string> ProcessNextAiActionAsync()
        {
            // save the _display content to a local image
            using var bitmap = new Bitmap(_display.Width, _display.Height);
            _display.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            // if the file "capture.png" exists, delete it
            if (File.Exists("capture.png"))
            {
                File.Delete("capture.png");
            }

            // save the image to a file named "capture.png"
            bitmap.Save("capture.png", System.Drawing.Imaging.ImageFormat.Png);

            // use the AI Action Generator to generate the next action
            string imageLocation = "capture.png";
            var actionResponse = await actionGenerator.GenerateNextActionResponse(imageLocation, aiRecentActivity);

            AddLog(actionResponse);

            // get the PressKey from the actionResponse
            var pressKey = actionResponse.PressKey;
            aiRecentActivity = actionResponse.RecentActivity;
            
            // send a press key action using windows API
            SendKeys.Send(pressKey);

            return pressKey;
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
                string pressedKey = await ProcessNextAiActionAsync();

                // Log the action if needed
                Console.WriteLine($"AI pressed: {pressedKey}");

                // Wait 1 second before the next action
                await Task.Delay(1000, cancellationToken);
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
            string message = $"Key: {actionResponse.PressKey}\n{actionResponse.RecentActivity}";
            AddLog(message);
        }

        void AddLog(string message)
        {
            var logMessage = $"{DateTime.Now}: {message}";
            // add a log in the debug and the textbox named textBoxLog including a datetime stamp and the message, add the log always at the start of the textbox
            textBoxLog.Text = logMessage + Environment.NewLine + textBoxLog.Text;
            // add the log to the debug console
            Debug.WriteLine(logMessage);

        }

    }
}