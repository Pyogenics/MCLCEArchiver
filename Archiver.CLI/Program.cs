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

            using Archive archive = ArchiveFile.Open(args[0], ArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                string entryPath = Path.Combine(args[1], entry.Name);
                Console.WriteLine(entryPath);
                Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                using FileStream outputFileStream = new(entryPath, FileMode.Create);
                using Stream entryStream = entry.Open();
                entryStream.CopyTo(outputFileStream);
            }
        }
    }
}
