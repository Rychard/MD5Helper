using System;
using System.Threading.Tasks;

namespace HashCalculator.Core
{
    public interface IHashCalculator
    {
        /// <summary>
        /// Indicates whether or not the hash operation is complete.
        /// </summary>
        Boolean IsComplete { get; }

        /// <summary>
        /// A percentage value that represents the completion of the hash operation.
        /// </summary>
        Double Progress { get; }

        /// <summary>
        /// The number of bytes that have been hashed.
        /// </summary>
        Int64 BytesHashed { get; }

        /// <summary>
        /// The number of bytes that have not yet been hashed.
        /// </summary>
        Int64 BytesRemaining { get; }

        /// <summary>
        /// The total number of bytes that will be hashed.
        /// </summary>
        Int64 TotalBytes { get; }

        Task<Byte[]> ComputeHashAsync();
    }
}
