using Kermalis.EndianBinaryIO;
using System.IO.Compression;

namespace Archiver.Core
{
    public class ArchiveEntry
    {
        private readonly Archive _archive;
        
        private readonly int _offset;
        public string Name { get; }
        public int Size { get; }
        public bool Compressed { get; }

        internal ArchiveEntry(Archive archive, string name, int offset, int size, bool compressed)
        {
            _archive = archive;
            _offset = offset;
            Name = name;
            Size = size;
            Compressed = compressed;
        }

        public Stream Open()
        {
            Stream stream = _archive.GetSubStream(_offset, Size);
            if (Compressed)
            {
                // TODO: Compressed data is untested, couldn't find any archives with compressed entries to test with
                EndianBinaryReader binaryReader = new EndianBinaryReader(stream, Endianness.BigEndian);
                int uncompressedSize = binaryReader.ReadInt32(); // Uncompressed size is redundant for zlib

                // TODO: implement platform specfic decompress options
                return new ZLibStream(stream, CompressionMode.Decompress);
            }

            return stream;
        }
    }
}
