using Kermalis.EndianBinaryIO;
using System.Text;

namespace Archiver.Core
{
    public class Archive : IDisposable
    {
        private readonly Stream _stream;
        private readonly ArchiveMode _mode;
        private readonly bool _leaveOpen;

        public List<ArchiveEntry> Entries { get; } = new List<ArchiveEntry>(); // TODO: make this readonly lol

        public Archive(Stream stream, ArchiveMode mode)
        { 
            _stream = stream;
            _mode = mode;
            _leaveOpen = true;
            
            if (_mode == ArchiveMode.Read)
                ReadHeader();
        }

        internal Archive(Stream stream, ArchiveMode mode, bool leaveOpen)
        {
            _stream = stream;
            _mode = mode;
            _leaveOpen = leaveOpen;

            if (_mode == ArchiveMode.Read)
                ReadHeader();
        }

        private void ReadHeader()
        {
            EndianBinaryReader binaryReader = new EndianBinaryReader(_stream, Endianness.BigEndian);

            // Read file metadata
            int fileCount = binaryReader.ReadInt32();
            for (int fileI = 0; fileI < fileCount; fileI++)
            {
                int fileNameLength = binaryReader.ReadInt16();
                byte[] fileName = new byte[fileNameLength];
                binaryReader.ReadBytes(fileName);

                int fileOffset = binaryReader.ReadInt32();
                int fileSize = binaryReader.ReadInt32();

                // Files marked with an asterisk have been compressed
                bool compressed = false;
                if (fileName[0] == '*')
                {
                    compressed = true;
                    fileName = fileName[1..];
                }

                ArchiveEntry fileEntry = new(
                    this,
                    Encoding.UTF8.GetString(fileName),
                    fileOffset,
                    fileSize,
                    compressed
                );
                Entries.Add(fileEntry);
            }
        }

        internal Stream GetSubStream(int offset, int length)
        {
            SubStream stream = new(_stream, offset, length);
            return stream;
        }

        public void Dispose()
        {
            if (!_leaveOpen) _stream.Dispose();
        }
    }
}
