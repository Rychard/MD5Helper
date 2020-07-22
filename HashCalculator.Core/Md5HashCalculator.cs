using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashCalculator.Core
{
    public abstract class HashCalculatorBase : IHashCalculator
    {
        private readonly Stream _stream;
        private readonly Byte[] _buffer;
        private readonly HashAlgorithm _hashAlgorithm;
        private Byte[] _hashBytes;

        public Double Progress
        {
            get
            {
                Double onePercent = _stream.Length / 100D;
                Double percentComplete = Math.Round(BytesHashed / onePercent, 2);
                return percentComplete;
            }
        }

        public Boolean IsComplete => _hashBytes != null;

        public Int64 BytesHashed => _stream.Position;

        public Int64 BytesRemaining => _stream.Length - BytesHashed;

        public Int64 TotalBytes => _stream.Length;

        protected HashCalculatorBase(Stream stream, Int64 bufferSize, HashAlgorithm hashAlgorithm)
        {
            _stream = stream;
            _buffer = new Byte[bufferSize];
            _hashAlgorithm = hashAlgorithm;
        }

        private async Task<Boolean> ComputePartialHashAsync()
        {
            Int32 read = await _stream.ReadAsync(_buffer, 0, _buffer.Length);

            if (read == 0)
            {
                _hashAlgorithm.TransformFinalBlock(new Byte[0], 0, 0);
                _hashBytes = _hashAlgorithm.Hash;
                return false;
            }

            _hashAlgorithm.TransformBlock(_buffer, 0, read, _buffer, 0);
            return true;
        }

        public async Task<Byte[]> ComputeHashAsync()
        {
            while (await ComputePartialHashAsync())
            {
            }
            return _hashBytes;
        }
    }

    public class Md5HashCalculator : IHashCalculator
    {
        private readonly Stream _stream;
        private readonly Byte[] _buffer;
        private readonly MD5 _md5;
        private Byte[] _hashBytes;

        public Double Progress
        {
            get
            {
                Double onePercent = _stream.Length / 100D;
                Double percentComplete = Math.Round(BytesHashed / onePercent, 2);
                return percentComplete;
            }
        }

        public Boolean IsComplete => _hashBytes != null;

        public Int64 BytesHashed => _stream.Position;

        public Int64 BytesRemaining => _stream.Length - BytesHashed;

        public Int64 TotalBytes => _stream.Length;

        public Md5HashCalculator(Stream stream, Int64 bufferSize)
        {
            _stream = stream;
            _buffer = new Byte[bufferSize];
            _md5 = MD5.Create();
        }

        private async Task<Boolean> ComputePartialHashAsync()
        {
            Int32 read = await _stream.ReadAsync(_buffer, 0, _buffer.Length);

            if (read == 0)
            {
                _md5.TransformFinalBlock(new Byte[0], 0, 0);
                _hashBytes = _md5.Hash;
                return false;
            }

            _md5.TransformBlock(_buffer, 0, read, _buffer, 0);
            return true;
        }

        public async Task<Byte[]> ComputeHashAsync()
        {
            while (await ComputePartialHashAsync())
            {
            }
            return _hashBytes;
        }
    }
}