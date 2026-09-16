// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Brokers;

internal interface IDocumentArchiveBroker
{
    byte[] Create(IEnumerable<ArchiveEntryData> entries);

    ArchiveEntryData[] Read(byte[] content);
}