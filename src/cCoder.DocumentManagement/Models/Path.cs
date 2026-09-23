// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models;

public sealed class Path
{
    public static Path Empty =>
        new() { FullPath = string.Empty };

    public string Name => Segments.LastOrDefault();

    private string fullPath;

    public string FullPath
    {
        get => fullPath;
        init => fullPath = (value ?? string.Empty).Trim()
            .TrimEnd(trimChar: '/');
    }

    public string Lowered => FullPath.ToLower();

    public string[] Segments => FullPath.Split(separator: '/');

    public Path ParentPath =>
        Segments.Length > 1
            ? new Path
            {
                FullPath = string.Join(separator: "/", value: Segments)[..(FullPath.Length - (1 + Segments.Last().Length))]
            }
            : Empty;

    public string Extension =>
        Segments.LastOrDefault()?.Contains(value: '.') ?? false
            ? Segments.LastOrDefault()?.Split(separator: '.')
                                                             .Last()
                                                                    .ToLower() ?? string.Empty
            : string.Empty;

    public string MimeType =>
        Extension switch
        {
            "json" => "application/json",
            "pdf" => "application/pdf",
            "svg" => "image/svg+xml",
            "xml" => "application/xml",
            "zip" => "application/zip",
            _ => "text/plain"
        };

    public int Length => FullPath.Length;

    public int Depth => Segments.Length;

    public bool IsToFile => Extension.Length > 0;

}