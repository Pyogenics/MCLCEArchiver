using Archiver.Core;
using Spectre.Console;
using System.CommandLine;


//
// Main
//
RootCommand rootCommand = new RootCommand("CLI tool for Minecraft Legacy Console Edition archive files");

// Options
Option<bool> verboseOption = new("--verbose")
{
    Aliases = { "-v" },
    Description = "Print verbose output"
};
rootCommand.Add(verboseOption);

Option<DirectoryInfo> outputOption = new("--output")
{
    Aliases = { "-o" },
    Description = "Output directory for extracted files (defaults to current directory)"
};

// Commands
Argument<FileInfo> archiveFileArgument = new("archive-file")
{
    Description = "The archive file to operate on"
};
Command listCommand = new Command("list")
{
    Description = "Lists the contents of an archive file",
    Aliases = { "l" }
};
listCommand.Add(archiveFileArgument);
listCommand.SetAction(ListCommandHandler);
rootCommand.Add(listCommand);

Command extractCommand = new Command("extract")
{
    Description = "Extracts the contents of an archive file",
    Aliases = { "x" }
};
extractCommand.Add(archiveFileArgument);
extractCommand.Add(outputOption);
extractCommand.Add(verboseOption);
extractCommand.SetAction(ExtractCommandHandler);
rootCommand.Add(extractCommand);


//
// Functions
//
void ListCommandHandler(ParseResult parseResult)
{
    FileInfo? fileInfo = parseResult.GetValue<FileInfo>("archive-file");
    if (fileInfo == null)
    {
        Console.WriteLine("No archive file specified.");
        return;
    }
    if (!fileInfo.Exists)
    {
        Console.WriteLine("The specified archive file does not exist.");
        return;
    }

    using Archive archive = ArchiveFile.Open(fileInfo.FullName, ArchiveMode.Read);
    Console.WriteLine($"Reading {archive.Entries.Count} entries from {fileInfo.Name}");

    Table table = new();
    table.AddColumn("Name");
    table.AddColumn("Size", col => col.RightAligned());
    table.AddColumn("Compressed", col => col.RightAligned());
    foreach (ArchiveEntry entry in archive.Entries)
    {
        table.AddRow(entry.Name, entry.Size.ToString() + " Bytes", entry.Compressed.ToString());
    }
    AnsiConsole.Write(table);
}

void ExtractCommandHandler(ParseResult parseResult)
{
    bool verbose = parseResult.GetValue<bool>(verboseOption);
    FileInfo? fileInfo = parseResult.GetValue<FileInfo>("archive-file");
    if (fileInfo == null)
    {
        Console.WriteLine("No archive file specified.");
        return;
    }
    if (!fileInfo.Exists)
    {
        Console.WriteLine("The specified archive file does not exist.");
        return;
    }
    DirectoryInfo outputDirectory = parseResult.GetValue<DirectoryInfo>(outputOption) ?? new DirectoryInfo(Directory.GetCurrentDirectory());

    using Archive archive = ArchiveFile.Open(fileInfo.FullName, ArchiveMode.Read);
    Console.WriteLine($"Extracting {archive.Entries.Count} entries from {fileInfo.Name} to {outputDirectory.FullName}");

    foreach (ArchiveEntry entry in archive.Entries)
    {
        string outputPath = Path.Combine(outputDirectory.FullName, entry.Name);
        if (verbose)
            Console.WriteLine($"{outputPath}");

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        using Stream entryStream = entry.Open();
        using FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        entryStream.CopyTo(outputStream);
    }
}


//
// Return
//
return rootCommand.Parse(args).Invoke();
