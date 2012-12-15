using System;

namespace MD5Helper
{
    public class HashingStatusUpdateEventArgs : EventArgs
    {
        public double Complete;
        public long Size;
        public long Position;

        public HashingStatusUpdateEventArgs(double PercentComplete, long TotalSize, long CurrentPosition)
        {
            this.Complete = PercentComplete;
            this.Size = TotalSize;
            this.Position = CurrentPosition;
        }
    }
}
