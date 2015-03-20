using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using HashHelper.Model;

namespace HashHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // TODO: This should optimally be pulled from a configuration file.
        const long bufferSize = 131072; // 128 KB
        //const long bufferSize = 8388608; // 8192 KB

        private Double _completion;
        public Double Completion
        {
            get { return _completion; }
            set
            {
                // Since 0.01% is the minimum change that can be represented on the UI, that's the smallest value that we will accept as a change.
                const double tolerance = 0.0001;
                if (Math.Abs(_completion - value) < tolerance) { return; }
                _completion = value;
                RaisePropertyChanged();
            }
        }

        private long _speedAmount;
        public long SpeedAmount
        {
            get { return _speedAmount; }
            set
            {
                _speedAmount = value;
                RaisePropertyChanged();
            }
        }

        private String _fileName;
        public  String FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName == value) { return; }
                _fileName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _completion = 0;
            _speedAmount = 0;
            _fileName = @"C:\Users\Joshua\Desktop\Backup\Desktop\MSDN\Windows Server 2012\SW_DVD5_NTRL_Windows_Svrs_2012_R2_English_FPP_OEM_Std_DC_X19-03239.ISO";

            var result = Hash();
            result.ContinueWith(task =>
            {
                MessageBox.Show(task.Result, "Hash");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            });
        }

        private async Task<String> Hash()
        {
            SHA1 hasher = SHA1.Create();
            //MD5 hasher = MD5.Create();
            long offset = 0;
            FileStream fs = null;

            try
            {
                fs = new FileStream(FileName, FileMode.Open);
            }
            catch {}

            long filesize = fs.Length;

            await Task.Run(() =>
            {
                long lastSecondOffset = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                byte[] buffer = new byte[bufferSize];
                while (fs.Read(buffer, 0, buffer.Length) > 0)
                {    
                    long length = filesize - offset;
                    int currentLength = buffer.Length;
                    if (buffer.Length > length)
                    {
                        currentLength = (int)length;
                    }
                    offset += hasher.TransformBlock(buffer, 0, currentLength, buffer, 0);
                    
                    Double percentComplete = Math.Round((fs.Position / (Double)fs.Length), 4);

                    const Double updateIntervalSeconds = 0.25;
                    if (stopwatch.Elapsed.TotalSeconds >= 0.25)
                    {
                        SpeedAmount = (long)Math.Round((offset - lastSecondOffset) * (1 / updateIntervalSeconds), 0);
                        lastSecondOffset = offset;
                        stopwatch.Restart();
                    }
                    
                    Completion = percentComplete;
                }
                fs.Close();
                hasher.TransformFinalBlock(new Byte[0], 0, 0);
            });
            
            byte[] bHash = hasher.Hash;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bHash.Length; i++)
            {
                sb.Append(bHash[i].ToString("x2")); // Return as Hex, duh.
            }
            String sHash = sb.ToString();
            return sHash;
        }
    }
}