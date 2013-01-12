using System;

namespace MD5Helper
{
    /// <summary>
    /// Used to communicate the progress of the hashing function.
    /// </summary>
    public class HashingStatusUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value that represents the completion of the hashing process.
        /// </summary>
        /// <remarks>The returned value is conveyed as a percentage.</remarks>
        public double Complete { get; private set; }

        /// <summary>
        /// Gets the number of bytes in the stream that is being hashed.
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Gets the index of the byte that the hashing function is on.
        /// </summary>
        /// <remarks>This property, along with the <c>Size</c> property, is used to calculate the <c>Complete</c> property.</remarks>
        public long Position { get; private set; }

        public HashingStatusUpdateEventArgs(double PercentComplete, long TotalSize, long CurrentPosition)
        {
            this.Complete = PercentComplete;
            this.Size = TotalSize;
            this.Position = CurrentPosition;
        }
    }
}
