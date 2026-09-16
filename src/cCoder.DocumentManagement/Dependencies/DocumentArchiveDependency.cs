// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO.Compression;

namespace cCoder.DocumentManagement.Dependencies;

internal sealed class DocumentArchiveDependency : ZipArchive
{
    private readonly MemoryStream content;

    internal DocumentArchiveDependency(bool create)
        : this(
            content: new MemoryStream(),
            mode: create ? ZipArchiveMode.Create : ZipArchiveMode.Read)
    {
    }

    internal DocumentArchiveDependency(byte[] bytes)
        : this(
            content: new MemoryStream(buffer: bytes),
            mode: ZipArchiveMode.Read)
    {
    }

    private DocumentArchiveDependency(MemoryStream content, ZipArchiveMode mode)
        : base(stream: content, mode: mode, leaveOpen: true)
    {
        this.content = content;
    }

    internal void Add(IEnumerable<(string FullName, byte[] Content)> entries)
    {
        foreach ((string fullName, byte[] entryContent) in entries)
        {
            ZipArchiveEntry entry = CreateEntry(
                entryName: fullName,
                compressionLevel: CompressionLevel.Optimal);

            if (entryContent is not null)
            {
                using Stream entryStream = entry.Open();
                entryStream.Write(
                    buffer: entryContent,
                    offset: 0,
                    count: entryContent.Length);
            }
        }
    }

    internal (string FullName, byte[] Content)[] Read() =>
        Entries.Select(selector: entry =>
        {
            using Stream entryStream = entry.Open();
            using MemoryStream output = new();
            entryStream.CopyTo(destination: output);

            return (entry.FullName, output.ToArray());
        }).ToArray();

    internal byte[] Complete()
    {
        Dispose();
        return content.ToArray();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing: disposing);

        if (disposing)
        {
            content.Dispose();
        }
    }
}