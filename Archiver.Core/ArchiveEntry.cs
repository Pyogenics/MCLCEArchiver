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
            return _archive.GetSubStream(_offset, Size);
        }
    }
}
