namespace cCoder.DocumentManagement.Models;

public struct Mapping
{
    public string FileExtension { get; set; }

    public string MimeType { get; set; }
}

public static class MimeType
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

    public static Mapping Get(string fileExtension)
    {
        string normalized = (fileExtension ?? string.Empty).ToLowerInvariant();
        return All.FirstOrDefault(mapping => mapping.FileExtension == normalized);
    }
}
