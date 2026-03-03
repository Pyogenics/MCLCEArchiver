namespace Archiver.Core
{
    public static class ArchiveFile
    {
        public static Archive Open(string archiveFileName, ArchiveMode mode)
        {
            FileStream fileStream = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read);
            return new Archive(fileStream, mode, false);
        }
    }
}
