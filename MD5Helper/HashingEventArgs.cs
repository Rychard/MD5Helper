using System;

namespace MD5Helper
{
    public class HashingEventArgs : EventArgs
    {
        public String ChecksumValue;

        public HashingEventArgs(String Checksum)
        {
            this.ChecksumValue = Checksum;
        }
    }
}
