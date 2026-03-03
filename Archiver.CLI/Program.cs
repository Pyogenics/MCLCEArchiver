using Archiver.Core;

namespace Archiver.CLI
{
    internal class Program
    {
        // TODO: this was quick because I want to go to sleep rn
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect usage! Please supply a path to an arc file you want to deflate followed by the output folder path.");
            }

            using FileStream fileStream = new(args[0], FileMode.Open);
            Archive archive = new(fileStream, ArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                // TODO: Handle compressed files, probably not here but inside ArchiverFileEntry?
                string entryPath = args[1] + "/" + entry.Name;
                Console.WriteLine(entryPath);
                Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                using FileStream outputFileStream = new(entryPath, FileMode.Create);
                using Stream entryStream = entry.Open();
                entryStream.CopyTo(outputFileStream);
            }
        }
    }
}
