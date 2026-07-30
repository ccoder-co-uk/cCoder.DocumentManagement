// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO.Compression;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Dependencies;

internal sealed class DocumentArchiveDependency : IDisposable
{
    private readonly DocumentStreamDependency content;
    private readonly ZipArchive archive;
    private bool archiveDisposed;

    internal DocumentArchiveDependency(byte[] bytes = null)
    {
        content = bytes is null
            ? new DocumentStreamDependency()
            : new DocumentStreamDependency(buffer: bytes);
        archive = new(
            stream: content,
            mode: bytes is null
                ? ZipArchiveMode.Create
                : ZipArchiveMode.Read);
    }

    internal void AddEntry(string name, byte[] content = null)
    {
        ZipArchiveEntry entry = archive.CreateEntry(
            entryName: name,
            compressionLevel: CompressionLevel.Optimal);

        if (content is null)
        {
            return;
        }

        using Stream stream = entry.Open();
        stream.Write(buffer: content, offset: 0, count: content.Length);
    }

    internal ArchiveEntryData[] ReadEntries() =>
        archive.Entries.Select(selector: entry =>
        {
            using Stream entryStream = entry.Open();
            using DocumentStreamDependency output = new();
            entryStream.CopyTo(destination: output);

            return new ArchiveEntryData
            {
                FullName = entry.FullName,
                Content = output.ToArray()
            };
        }).ToArray();

    internal byte[] Complete()
    {
        DisposeArchive();
        return content.ToArray();
    }

    public void Dispose()
    {
        DisposeArchive();
        content.Dispose();
    }

    private void DisposeArchive()
    {
        if (archiveDisposed)
        {
            return;
        }

        archive.Dispose();
        archiveDisposed = true;
    }
}