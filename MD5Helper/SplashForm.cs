using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography; // MD5

namespace MD5Helper
{
    public delegate void HashingFinished(object sender, HashingEventArgs e);
    public delegate void HashingStatusUpdate(object sender, HashingStatusUpdateEventArgs e);
    public delegate void UpdateStatusDelegate(HashingStatusUpdateEventArgs e);
    public delegate void CloseDelegate();
    
    public partial class SplashForm : Form
    {
        public event HashingFinished HashingFinished;
        public event HashingStatusUpdate HashingStatusUpdate;

        public HashingStatusUpdateEventArgs DisplayedStatus;
        public DateTime DateOfStatus;

        const long bufferSize = 131072; // 128 KB
        //const long bufferSize = 8388608; // 8192 KB

        public SplashForm()
        {
            InitializeComponent();
            HashingStatusUpdate += new HashingStatusUpdate(SplashForm_OnHashingStatusUpdate);
        }

        public void UpdateProgress(HashingStatusUpdateEventArgs e)
        {
            if (DisplayedStatus == null)
            {
                DisplayedStatus = e;
                DateOfStatus = DateTime.Now;
                pbCalculation.Value = (int)Math.Floor(e.Complete);
                lblPosition.Text = e.Complete + "% - " + e.Position.ToString() + "/" + e.Size.ToString();
                lblSpeed.Text = String.Empty;
            }
            else
            {
                if (DisplayedStatus.Complete + 2 < e.Complete)
                {
                    DateTime CurrentTime = DateTime.Now;
                    pbCalculation.Value = (int)Math.Floor(e.Complete);
                    lblPosition.Text = e.Complete + "% - " + e.Position.ToString() + "/" + e.Size.ToString();
                    long BytesSinceLast = e.Position - DisplayedStatus.Position;
                    FileSize fs = new FileSize(BytesSinceLast);
                    String DataSinceLast = fs.ToString();
                    lblSpeed.Text = DataSinceLast + "/second";
                    DisplayedStatus = e;
                    DateOfStatus = CurrentTime;   
                }
            }
        }

        void SplashForm_OnHashingStatusUpdate(object sender, HashingStatusUpdateEventArgs e)
        {
            this.Invoke(new UpdateStatusDelegate(this.UpdateProgress), e);
        }

        public void Calculate(String filepath)
        {
            this.Show();

            try
            {
                if (File.Exists(filepath))
                {
                    FileInfo fi = new FileInfo(filepath);
                    FileSize fs = new FileSize(fi.Length);

                    lblStatus.Text = "Calculating MD5... (For large files this may take a while)";
                    lblFilename.Text = filepath;
                    lblFileSize.Text = fs.ToString();

                    ParameterizedThreadStart pt = new ParameterizedThreadStart(GetMD5);
                    Thread t = new Thread(pt);
                    t.Start(filepath);
                }
                else
                {
                    this.Close();
                    MessageBox.Show("File does not exist.");
                    Application.Exit();
                }
            }
            catch (Exception)
            {
                // If an exception is thrown, it's likely that the file is in use.
                MessageBox.Show("File is currently in use by another application.");
                Application.Exit();
            }
        }

        public void GetMD5(Object strFilePath)
        {
            String FilePath = strFilePath.ToString();
            MD5 MD5Hasher = MD5.Create();
            long offset = 0;
            FileStream fs = null;

            try
            {
                fs = new FileStream(FilePath, FileMode.Open);
            }
            catch (Exception)
            {
                // If an exception is thrown, it's likely that the file is in use.
                MessageBox.Show("File is currently in use by another application.");
                Application.Exit();
            }

            if (fs == null)
            {
                // I can't think of a single reason why it would still be null...
                // But just in case, let's handle it.
                Application.Exit();
            }

            long filesize = fs.Length;
            double onePercent = filesize / 100D;
            HashingStatusUpdateEventArgs update;

            byte[] buffer = new byte[bufferSize];
            while (fs.Read(buffer, 0, buffer.Length) > 0)
            {
                long length = filesize - offset;
                int currentLength = buffer.Length;
                if (buffer.Length > length)
                {
                    currentLength = (int)length;
                }
                offset += MD5Hasher.TransformBlock(buffer, 0, currentLength, buffer, 0);
                update = new HashingStatusUpdateEventArgs(Math.Round((fs.Position / onePercent), 2), fs.Length, fs.Position);
                HashingStatusUpdate(this, update);
            }
            fs.Close();
            MD5Hasher.TransformFinalBlock(new Byte[0], 0, 0);
            byte[] bHash = MD5Hasher.Hash;
            update = new HashingStatusUpdateEventArgs(100, filesize, filesize); // Finished.
            HashingStatusUpdate(this, update);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bHash.Length; i++)
            {
                sb.Append(bHash[i].ToString("x2")); // Return as Hex, duh.
            }
            String sHash = sb.ToString();

            HashingEventArgs e = new HashingEventArgs(sHash);
            if (HashingFinished != null)
            {
                HashingFinished(this, e);
            }
        }
    }
}
