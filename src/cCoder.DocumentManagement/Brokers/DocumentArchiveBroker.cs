// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Dependencies;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Brokers;

internal sealed class DocumentArchiveBroker : IDocumentArchiveBroker, IUtilityBroker
{
    public byte[] Create(IEnumerable<ArchiveEntryData> entries)
    {
        using DocumentArchiveDependency archive = new(create: true);

        archive.Add(entries:
            entries.Select(selector: entry =>
                (entry.FullName, entry.Content)));

        return archive.Complete();
    }

    public ArchiveEntryData[] Read(byte[] content)
    {
        using DocumentArchiveDependency archive = new(bytes: content);

        return archive.Read()
            .Select(selector: entry => new ArchiveEntryData
            {
                FullName = entry.FullName,
                Content = entry.Content
            })
            .ToArray();
    }
}