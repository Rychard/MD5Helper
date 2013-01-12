using System;

namespace MD5Helper
{
    /// <summary>
    /// Used to represent the value of a computed hash.
    /// </summary>
    public class HashingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the value of the computed hash.
        /// </summary>
        public String ChecksumValue { get; private set; }

        public HashingEventArgs(String Checksum)
        {
            this.ChecksumValue = Checksum;
        }
    }
}
