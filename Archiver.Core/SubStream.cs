namespace Archiver.Core
{
    internal class SubStream : Stream
    {
        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override long Length => _length;

        public override long Position { get => _position; set => _position = value; }

        private readonly Stream _baseStream;
        private readonly long _offset;
        private readonly int _length;
        private long _position = 0;

        public SubStream(Stream baseStream, long offset, int length)
        {
            _baseStream = baseStream;
            _offset = offset;
            _length = length;
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesAvailable = (int)(_length - _position);
            if (count > bytesAvailable)
                count = bytesAvailable;

            _baseStream.Seek(_offset + _position, SeekOrigin.Begin);
            int bytesRead = _baseStream.Read(buffer, offset, count);
            _position += bytesRead;
            
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = _offset + offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = _baseStream.Position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = _offset + _length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }
            if (newPosition < _offset || newPosition > _offset + _length)
                throw new IOException("Attempted to seek outside the bounds of the substream.");

            _position = _baseStream.Seek(newPosition, SeekOrigin.Begin) - _offset;
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Cannot set length of a substream.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
