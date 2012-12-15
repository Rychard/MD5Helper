using System;

namespace MD5Helper
{
    public class FileSize
    {
        private readonly long _totalBytes;
        private readonly double _bytes;
        private readonly double _kilobytes;
        private readonly double _megabytes;
        private readonly double _gigabytes;
        private readonly double _terabytes;

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
    }
}
