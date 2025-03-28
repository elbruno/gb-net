namespace GB.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _soundOutput.Stop();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openROMToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            emulatorToolStripMenuItem = new ToolStripMenuItem();
            pauseToolStripMenuItem = new ToolStripMenuItem();
            soundToolStripMenuItem = new ToolStripMenuItem();
            _toggleChannel1 = new ToolStripMenuItem();
            _toggleChannel2 = new ToolStripMenuItem();
            _toggleChannel3 = new ToolStripMenuItem();
            _toggleChannel4 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            enableBootROMToolStripMenuItem = new ToolStripMenuItem();
            gameBoyModeToolStripMenuItem = new ToolStripMenuItem();
            autodetectToolStripMenuItem = new ToolStripMenuItem();
            dMGClassicToolStripMenuItem = new ToolStripMenuItem();
            gameBoyColorToolStripMenuItem = new ToolStripMenuItem();
            aIInteractionToolStripMenuItem = new ToolStripMenuItem();
            startToolStripMenuItem = new ToolStripMenuItem();
            stopToolStripMenuItem = new ToolStripMenuItem();
            _display = new GB.WinForms.OsSpecific.BitmapDisplay();
            splitContainerGB = new SplitContainer();
            textBoxLog = new TextBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerGB).BeginInit();
            splitContainerGB.Panel1.SuspendLayout();
            splitContainerGB.Panel2.SuspendLayout();
            splitContainerGB.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(32, 32);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, emulatorToolStripMenuItem, aIInteractionToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(8, 2, 0, 2);
            menuStrip1.Size = new Size(1000, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openROMToolStripMenuItem, toolStripMenuItem1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "&File";
            // 
            // openROMToolStripMenuItem
            // 
            openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            openROMToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openROMToolStripMenuItem.Size = new Size(227, 26);
            openROMToolStripMenuItem.Text = "&Open ROM...";
            openROMToolStripMenuItem.Click += OpenRom;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(224, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(227, 26);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += OnExit;
            // 
            // emulatorToolStripMenuItem
            // 
            emulatorToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { pauseToolStripMenuItem, soundToolStripMenuItem, toolStripMenuItem2, enableBootROMToolStripMenuItem, gameBoyModeToolStripMenuItem });
            emulatorToolStripMenuItem.Name = "emulatorToolStripMenuItem";
            emulatorToolStripMenuItem.Size = new Size(83, 24);
            emulatorToolStripMenuItem.Text = "&Emulator";
            // 
            // pauseToolStripMenuItem
            // 
            pauseToolStripMenuItem.CheckOnClick = true;
            pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            pauseToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            pauseToolStripMenuItem.Size = new Size(210, 26);
            pauseToolStripMenuItem.Text = "&Pause";
            pauseToolStripMenuItem.CheckedChanged += TogglePause;
            // 
            // soundToolStripMenuItem
            // 
            soundToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _toggleChannel1, _toggleChannel2, _toggleChannel3, _toggleChannel4 });
            soundToolStripMenuItem.Name = "soundToolStripMenuItem";
            soundToolStripMenuItem.Size = new Size(210, 26);
            soundToolStripMenuItem.Text = "&Sound";
            // 
            // _toggleChannel1
            // 
            _toggleChannel1.Checked = true;
            _toggleChannel1.CheckOnClick = true;
            _toggleChannel1.CheckState = CheckState.Checked;
            _toggleChannel1.Name = "_toggleChannel1";
            _toggleChannel1.Size = new Size(157, 26);
            _toggleChannel1.Text = "Channel &1";
            _toggleChannel1.CheckedChanged += ToggleSoundChannel;
            // 
            // _toggleChannel2
            // 
            _toggleChannel2.Checked = true;
            _toggleChannel2.CheckOnClick = true;
            _toggleChannel2.CheckState = CheckState.Checked;
            _toggleChannel2.Name = "_toggleChannel2";
            _toggleChannel2.Size = new Size(157, 26);
            _toggleChannel2.Text = "Channel &2";
            _toggleChannel2.CheckedChanged += ToggleSoundChannel;
            // 
            // _toggleChannel3
            // 
            _toggleChannel3.Checked = true;
            _toggleChannel3.CheckOnClick = true;
            _toggleChannel3.CheckState = CheckState.Checked;
            _toggleChannel3.Name = "_toggleChannel3";
            _toggleChannel3.Size = new Size(157, 26);
            _toggleChannel3.Text = "Channel &3";
            _toggleChannel3.CheckedChanged += ToggleSoundChannel;
            // 
            // _toggleChannel4
            // 
            _toggleChannel4.Checked = true;
            _toggleChannel4.CheckOnClick = true;
            _toggleChannel4.CheckState = CheckState.Checked;
            _toggleChannel4.Name = "_toggleChannel4";
            _toggleChannel4.Size = new Size(157, 26);
            _toggleChannel4.Text = "Channel &4";
            _toggleChannel4.CheckedChanged += ToggleSoundChannel;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(207, 6);
            // 
            // enableBootROMToolStripMenuItem
            // 
            enableBootROMToolStripMenuItem.Checked = true;
            enableBootROMToolStripMenuItem.CheckOnClick = true;
            enableBootROMToolStripMenuItem.CheckState = CheckState.Checked;
            enableBootROMToolStripMenuItem.Name = "enableBootROMToolStripMenuItem";
            enableBootROMToolStripMenuItem.Size = new Size(210, 26);
            enableBootROMToolStripMenuItem.Text = "Enable &Boot ROM";
            // 
            // gameBoyModeToolStripMenuItem
            // 
            gameBoyModeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { autodetectToolStripMenuItem, dMGClassicToolStripMenuItem, gameBoyColorToolStripMenuItem });
            gameBoyModeToolStripMenuItem.Name = "gameBoyModeToolStripMenuItem";
            gameBoyModeToolStripMenuItem.Size = new Size(210, 26);
            gameBoyModeToolStripMenuItem.Text = "Game Boy &Mode";
            // 
            // autodetectToolStripMenuItem
            // 
            autodetectToolStripMenuItem.Checked = true;
            autodetectToolStripMenuItem.CheckState = CheckState.Checked;
            autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            autodetectToolStripMenuItem.Size = new Size(200, 26);
            autodetectToolStripMenuItem.Text = "&Auto-detect";
            autodetectToolStripMenuItem.Click += GameBoyModeClicked;
            // 
            // dMGClassicToolStripMenuItem
            // 
            dMGClassicToolStripMenuItem.Name = "dMGClassicToolStripMenuItem";
            dMGClassicToolStripMenuItem.Size = new Size(200, 26);
            dMGClassicToolStripMenuItem.Text = "&DMG (Classic)";
            dMGClassicToolStripMenuItem.Click += GameBoyModeClicked;
            // 
            // gameBoyColorToolStripMenuItem
            // 
            gameBoyColorToolStripMenuItem.Name = "gameBoyColorToolStripMenuItem";
            gameBoyColorToolStripMenuItem.Size = new Size(200, 26);
            gameBoyColorToolStripMenuItem.Text = "Game Boy &Color";
            gameBoyColorToolStripMenuItem.Click += GameBoyModeClicked;
            // 
            // aIInteractionToolStripMenuItem
            // 
            aIInteractionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { startToolStripMenuItem, stopToolStripMenuItem });
            aIInteractionToolStripMenuItem.Name = "aIInteractionToolStripMenuItem";
            aIInteractionToolStripMenuItem.Size = new Size(112, 24);
            aIInteractionToolStripMenuItem.Text = "AI Interaction";
            // 
            // startToolStripMenuItem
            // 
            startToolStripMenuItem.Name = "startToolStripMenuItem";
            startToolStripMenuItem.Size = new Size(123, 26);
            startToolStripMenuItem.Text = "Start";
            startToolStripMenuItem.Click += startToolStripMenuItem_ClickAsync;
            // 
            // stopToolStripMenuItem
            // 
            stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            stopToolStripMenuItem.Size = new Size(123, 26);
            stopToolStripMenuItem.Text = "Stop";
            stopToolStripMenuItem.Click += stopToolStripMenuItem_Click;
            // 
            // _display
            // 
            _display.BackColor = Color.FromArgb(0, 230, 248, 218);
            _display.DisplayEnabled = false;
            _display.Dock = DockStyle.Fill;
            _display.Location = new Point(0, 0);
            _display.Margin = new Padding(2);
            _display.Name = "_display";
            _display.Size = new Size(1000, 565);
            _display.TabIndex = 1;
            _display.TabStop = false;
            _display.Text = "Game Boy Display";
            // 
            // splitContainerGB
            // 
            splitContainerGB.Dock = DockStyle.Fill;
            splitContainerGB.Location = new Point(0, 28);
            splitContainerGB.Name = "splitContainerGB";
            splitContainerGB.Orientation = Orientation.Horizontal;
            // 
            // splitContainerGB.Panel1
            // 
            splitContainerGB.Panel1.Controls.Add(_display);
            // 
            // splitContainerGB.Panel2
            // 
            splitContainerGB.Panel2.Controls.Add(textBoxLog);
            splitContainerGB.Size = new Size(1000, 810);
            splitContainerGB.SplitterDistance = 565;
            splitContainerGB.TabIndex = 2;
            splitContainerGB.TabStop = false;
            // 
            // textBoxLog
            // 
            textBoxLog.Dock = DockStyle.Fill;
            textBoxLog.Enabled = false;
            textBoxLog.Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxLog.Location = new Point(0, 0);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(1000, 241);
            textBoxLog.TabIndex = 0;
            textBoxLog.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1000, 838);
            Controls.Add(splitContainerGB);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "GB.NET";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainerGB.Panel1.ResumeLayout(false);
            splitContainerGB.Panel2.ResumeLayout(false);
            splitContainerGB.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerGB).EndInit();
            splitContainerGB.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openROMToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem emulatorToolStripMenuItem;
        private ToolStripMenuItem pauseToolStripMenuItem;
        private OsSpecific.BitmapDisplay _display;
        private ToolStripMenuItem soundToolStripMenuItem;
        private ToolStripMenuItem _toggleChannel1;
        private ToolStripMenuItem _toggleChannel2;
        private ToolStripMenuItem _toggleChannel3;
        private ToolStripMenuItem _toggleChannel4;
        private ToolStripMenuItem enableBootROMToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem gameBoyModeToolStripMenuItem;
        private ToolStripMenuItem autodetectToolStripMenuItem;
        private ToolStripMenuItem dMGClassicToolStripMenuItem;
        private ToolStripMenuItem gameBoyColorToolStripMenuItem;
        private ToolStripMenuItem aIInteractionToolStripMenuItem;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private SplitContainer splitContainerGB;
        private TextBox textBoxLog;
    }
}