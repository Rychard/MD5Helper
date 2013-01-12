using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO; // For Streams

namespace MD5Helper
{
    public delegate void ShowDelegate();
    public delegate void HideDelegate();
    public delegate void SetupControlsDelegate(HashingEventArgs e);
    public partial class MainForm : Form
    {
        // TODO: This needs to be located to some type of configuration file, to aid in localization.
        // Static Text to be displayed on the form.
        private const String bClipboard = "Copy To Clipboard";
        private const String bClipboard2 = "Copied to Clipboard";
        private const String bCompare = "Quick Compare";
        private const String tCompare = "Paste MD5 Here To Compare";
        private const String bExit = "Exit";
        readonly String filepath;

        // The volatile keyword indicates that a field can be modified in the program by something such as the operating system, the hardware, or a concurrently executing thread.
        // Source: http://msdn.microsoft.com/en-us/library/x13ttww7(v=vs.71).aspx
        // The important part of this is where it says "concurrently executing thread", because that's exactly what is happening.
        volatile SplashForm splash;

        public MainForm()
        {
            String[] args = Environment.GetCommandLineArgs(); // 0th element is the path of the executable.  1st element is the first argument.
            if (args.Length > 1)
            {
                filepath = args[1];
            } else {
                const String msg = "Right-Click on a file and select 'Calculate md5'.";
                MessageBox.Show(msg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                return;
            }
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Boolean fileExists = false;

            try
            {
                fileExists = File.Exists(filepath);
            }
            catch (Exception)
            {
                // If an exception is thrown, it's likely that the file is in use.
                MessageBox.Show("File is currently in use by another application.");
                Application.Exit();
            }

            if (!fileExists)
            {
                string msg = "File does not exist." + Environment.NewLine + "Path: " + filepath;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            splash = new SplashForm();
            splash.HashingStatusUpdate += new HashingStatusUpdate(splash_OnHashingStatusUpdate);
            splash.HashingFinished += new HashingFinished(splash_OnHashingFinished);
            splash.Text = "MD5 Checksum";
            this.Visible = false;
            splash.Calculate(filepath);
        }

        void SetupControls(HashingEventArgs e)
        {
            // I'm not entirely sure why I'm creating controls by hand.
            // Though it likely has something to do some initial error with cross-thread access.
            // At this point though, it's probably safe to do all this in the designer, but I'm leaving it this way because it works.

            this.Visible = true;
            String md5_hash = e.ChecksumValue;

            // Measuring the size of strings and such.
            Graphics g = CreateGraphics();
            Font font_checksum = new Font("Segoe UI Mono", 24, FontStyle.Regular);
            Font font_button = new Font("Segoe UI", 12, FontStyle.Regular);
            SizeF size_hash = g.MeasureString(md5_hash, font_checksum);
            SizeF size_clipboard = g.MeasureString(bClipboard, font_button);
            SizeF size_exit = g.MeasureString(bExit, font_button);

            // Textbox Properties
            TextBox txtChecksum = new TextBox
                {
                    Name = "txtChecksum",
                    Margin = new Padding(0),
                    Location = new Point(0, 0),
                    TextAlign = HorizontalAlignment.Center,
                    Font = font_checksum,
                    AutoSize = false,
                    Height = (int)Math.Ceiling(size_hash.Height),
                    Width = (int)Math.Ceiling(size_hash.Width),
                    Text = md5_hash
                };
            txtChecksum.Select(txtChecksum.Text.Length, 0); // Deselect the text in the box.

            // Button (Clipboard) Properties
            Button btnClipboard = new Button
                {
                    Margin = new Padding(0),
                    Location = new Point(0, txtChecksum.Location.Y + txtChecksum.Height),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = font_button,
                    AutoSize = true,
                    Height = (int)Math.Ceiling(size_clipboard.Height),
                    Width = txtChecksum.Width / 3,
                    Text = bClipboard
                };
            btnClipboard.Click += new EventHandler(btnClipboard_Click);

            // Button (Compare) Properties
            Button btnCompare = new Button
                {
                    Margin = new Padding(0),
                    Location = new Point(btnClipboard.Right, txtChecksum.Location.Y + txtChecksum.Height),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = font_button,
                    AutoSize = true,
                    Height = (int)Math.Ceiling(size_exit.Height),
                    Width = txtChecksum.Width / 3,
                    Text = bCompare
                };
            btnCompare.Click += new EventHandler(btnCompare_Click);

            // Textbox (Compare) Properties
            TextBox txtCompare = new TextBox
                {
                    Name = "txtCompare",
                    Margin = new Padding(0),
                    Location = btnCompare.Location,
                    AutoSize = false,
                    TextAlign = HorizontalAlignment.Center,
                    Font = btnCompare.Font,
                    Height = btnCompare.Height,
                    Width = btnCompare.Width,
                    Text = tCompare,
                    Visible = false
                };

            txtCompare.GotFocus += new EventHandler(txtCompare_GotFocus);
            txtCompare.LostFocus += new EventHandler(txtCompare_LostFocus);
            txtCompare.TextChanged += new EventHandler(txtCompare_TextChanged);
            txtCompare.Select(txtCompare.Text.Length, 0); // Deselect the text in the box.  

            // Button (Exit) Properties
            Button btnExit = new Button
                {
                    Margin = new Padding(0),
                    Location = new Point(btnCompare.Right, txtChecksum.Location.Y + txtChecksum.Height),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = font_button,
                    AutoSize = true,
                    Height = (int)Math.Ceiling(size_exit.Height),
                    Width = txtChecksum.Width / 3,
                    Text = bExit
                };
            btnExit.Click += new EventHandler(btnExit_Click);

            // Form Properties
            this.Text = "MD5 Checksum";
            this.Width = 1;
            this.Height = 1;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            this.AutoSize = true;
            this.Controls.Add(txtChecksum);
            this.Controls.Add(btnClipboard);
            this.Controls.Add(btnCompare);
            btnCompare.Parent.Controls.Add(txtCompare);
            this.Controls.Add(btnExit);

            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;

        }

        void txtCompare_LostFocus(object sender, EventArgs e)
        {
            TextBox txtCompare = (TextBox)sender;
            if (txtCompare.Text == "")
            {
                txtCompare.Text = tCompare;
            }
        }

        void txtCompare_GotFocus(object sender, EventArgs e)
        {
            TextBox txtCompare = (TextBox)sender;
            if (txtCompare.Text == tCompare)
            {
                txtCompare.Text = "";
            }
        }

        void btnClipboard_Click(object sender, EventArgs e)
        {
            Button btnClipboard = ((Button)sender);
            Control[] Matches = btnClipboard.Parent.Controls.Find("txtChecksum", true);
            if (Matches.Length > 0)
            {
                Control txtChecksum = Matches[0];
                Clipboard.Clear();
                Clipboard.SetText(txtChecksum.Text);
                btnClipboard.Text = bClipboard2;
            }
            else
            {
                btnClipboard.Text = "Copy to Clipboard Failed!";
            }
        }

        void btnCompare_Click(object sender, EventArgs e)
        {
            Button btnCompare = ((Button)sender);
            btnCompare.Visible = false;

            // For the Find() method to "find" anything, you have to set the name property of the control.
            Control[] Matches = btnCompare.Parent.Controls.Find("txtCompare", true);
            if (Matches.Length > 0)
            {
                Control txtCompare = Matches[0];
                txtCompare.Visible = true;
            }
        }

        void txtCompare_TextChanged(object sender, EventArgs e)
        {
            TextBox txtCompare = ((TextBox)sender);
            Control[] Matches = txtCompare.Parent.Controls.Find("txtChecksum", false);
            if (Matches.Length > 0)
            {
                txtCompare.BackColor = Matches[0].Text == txtCompare.Text ? Color.LightGreen : Color.PaleVioletRed;
            }
        }

        void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void splash_OnHashingFinished(object sender, HashingEventArgs e)
        {
            splash.Invoke(new CloseDelegate(splash.Close));
            this.Invoke(new ShowDelegate(this.Show));
            this.Invoke(new SetupControlsDelegate(this.SetupControls), e);
        }

        private void splash_OnHashingStatusUpdate(object sender, HashingStatusUpdateEventArgs e)
        {
            if (e.Complete < 100)
            {
                this.Invoke(new HideDelegate(this.Hide));
            }
        }
    }
}

