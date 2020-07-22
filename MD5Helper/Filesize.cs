using System;

namespace MD5Helper
{
    public class FileSize
    {
        private static readonly String[] Sizes = { "B", "KB", "MB", "GB", "TB" };

        private readonly Int64 _totalBytes;
        private readonly Double _bytes;
        private readonly Double _kilobytes;
        private readonly Double _megabytes;
        private readonly Double _gigabytes;
        private readonly Double _terabytes;

        public FileSize(long bytes)
        {
            _totalBytes = bytes;

            _bytes = bytes;
            while (_bytes >= 1024)
            {
                _bytes -= 1024;
                _kilobytes++;
            }
            while (_kilobytes >= 1024)
            {
                _kilobytes -= 1024;
                _megabytes++;
            }
            while (_megabytes >= 1024)
            {
                _megabytes -= 1024;
                _gigabytes++;
            }
            while (_gigabytes >= 1024)
            {
                _gigabytes -= 1024;
                _terabytes++;
            }
        }

        public double TotalBytes
        {
            get { return _totalBytes; }
        }
        public double TotalKilobytes
        {
            get { return TotalBytes / 1024; }
        }
        public double TotalMegabytes
        {
            get { return TotalKilobytes / 1024; }
        }
        public double TotalGigabytes
        {
            get { return TotalMegabytes / 1024; }
        }
        public double TotalTerabytes
        {
            get { return TotalGigabytes / 1024; }
        }

        public double Bytes
        {
            get { return _bytes; }
        }
        public double Kilobytes
        {
            get { return _kilobytes; }
        }
        public double Megabytes
        {
            get { return _megabytes; }
        }
        public double Gigabytes
        {
            get { return _gigabytes; }
        }
        public double Terabytes
        {
            get { return _terabytes; }
        }

        public override String ToString()
        {
            if (TotalTerabytes > 1) { return Math.Round(TotalTerabytes, 1) + "TB"; }
            if (TotalGigabytes > 1) { return Math.Round(TotalGigabytes, 1) + "GB"; }
            if (TotalMegabytes > 1) { return Math.Round(TotalMegabytes, 1) + "MB"; }
            if (TotalKilobytes > 1) { return Math.Round(TotalKilobytes, 1) + "KB"; }
            return TotalBytes + "B";
        }


        
        private static String GetHumanReadableFileSize(Int64 bytes)
        {
            // Source: https://stackoverflow.com/a/281679/1187982
            Double len = bytes;
            Int64 magnitude = 0;
            while (len >= 1024 && magnitude + 1 < Sizes.Length)
            {
                magnitude++;
                len = len / 1024;
            }

            // Adjust the format String to your preferences. For example "{0:0.#}{1}" would show a single decimal place, and no space.
            return String.Format("{0:0.00} {1}", len, Sizes[magnitude]);
        }
    }
}
