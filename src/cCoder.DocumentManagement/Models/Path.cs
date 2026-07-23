// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models;

public class Path
{
    public static Path Empty { get; } = new(path: string.Empty);

    public string Name => Segments.LastOrDefault();

    public string FullPath { get; }

    public string Lowered => FullPath.ToLower();

    public string[] Segments => FullPath.Split(separator: '/');

    public Path ParentPath =>
        Segments.Length > 1
            ? new Path(
                path: string.Join(separator: "/", value: Segments)[..(FullPath.Length - (1 + Segments.Last().Length))]
            )
            : Empty;

    public string Extension =>
        Segments.LastOrDefault()?.Contains(value: '.') ?? false
            ? Segments.LastOrDefault()?.Split(separator: '.')
                                                             .Last()
                                                                    .ToLower() ?? string.Empty
            : string.Empty;

    public string MimeType
    {
        get
        {
            Mapping mapping = Models.MimeType.Get(fileExtension: Extension);
            return string.IsNullOrWhiteSpace(value: mapping.MimeType) ? "text/plain" : mapping.MimeType;
        }
    }

    public int Length => FullPath.Length;

    public int Depth => Segments.Length;

    public bool IsToFile => Extension.Length > 0;

    public Path(string path)
    {
        FullPath = (path ?? string.Empty).Trim()
            .TrimEnd(trimChar: '/');
    }

    public override string ToString() =>
        FullPath;
}