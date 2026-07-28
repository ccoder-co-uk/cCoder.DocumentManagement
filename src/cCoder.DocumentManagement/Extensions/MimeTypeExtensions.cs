// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Extensions;

public static class MimeTypeExtensions
{
    private static readonly Mapping[] All =
    [
        new() { FileExtension = "json", MimeType = "application/json" },
        new() { FileExtension = "pdf", MimeType = "application/pdf" },
        new() { FileExtension = "svg", MimeType = "image/svg+xml" },
        new() { FileExtension = "txt", MimeType = "text/plain" },
        new() { FileExtension = "xml", MimeType = "application/xml" },
        new() { FileExtension = "zip", MimeType = "application/zip" },
    ];

    public static Mapping GetMimeType(string fileExtension)
    {
        string normalized =
            (fileExtension ?? string.Empty).ToLowerInvariant();

        return All.FirstOrDefault(
            predicate: mapping =>
                mapping.FileExtension == normalized);
    }
}