using Kermalis.EndianBinaryIO;
using System.Text;

namespace Archiver.Core
{
    public class Archive
    {
        private readonly Stream _stream;
        private readonly ArchiveMode _mode;
        public List<ArchiveEntry> Entries { get; } = new List<ArchiveEntry>(); // TODO: make this readonly lol

        public Archive(Stream stream, ArchiveMode mode)
        { 
            _stream = stream;
            _mode = mode;

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
            // TODO: need to create some SubStream helper or something
            _stream.Seek(offset, SeekOrigin.Begin);
            MemoryStream stream = new MemoryStream();
            _stream.CopyTo(stream, offset);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
