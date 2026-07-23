// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Xml.Linq;
using cCoder.Data.Models.Security;

namespace cCoder.Data.Models.DMS;

public static class DmsModelExtensions
{
    public static Stream GetContent(this File file, int version = 0) =>
        version > 0
            ? new MemoryStream(buffer: file.Contents.FirstOrDefault(predicate: content => content.Version == version)?.RawData)
            : new MemoryStream(buffer: file.Contents.OrderBy(keySelector: content => content.Version)
                                                                                        .Last().RawData);

    public static void RecomputePath(this File file) =>
        file.Path = file.FolderId != Guid.Empty
            ? $"{file.Folder?.Path}/{file.Name}".ToLowerInvariant()
            : file.Name?.ToLowerInvariant();

    public static XElement ToWebDavResponse(this File file, string urlBase, XNamespace ns, IEnumerable<string> requestedProperties)
    {
        XElement propStat = BuildFilePropStatResponse(file: file, ns: ns, requestedProperties: requestedProperties);

        XElement response = new(
            name: ns + "response",
            new XElement(name: ns + "href", content: $"{urlBase}Core/App({file.Folder.AppId})/DAV/{file.Path}"),
            propStat);

        List<string> unsupportedProperties = ["executable", "checked-in", "checked-out"];

        foreach (string property in requestedProperties.Where(predicate: unsupportedProperties.Contains))
        {
            response.Add(content: new XElement(
                name: ns + "propStat",
                new XElement(name: ns + "prop", content: new XElement(name: ns + property)),
                new XElement(name: ns + "status", content: "HTTP/1.1 404 Not Found"),
                new XElement(name: ns + "responsedescription", content: $"Property {{DAV:}}{property} is not supported.")));
        }

        return response;
    }

    public static bool UserCan(this File file, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?.Select(selector: role => role.RoleId)
            .ToArray() ?? [];

        return (file.Folder != null && user.IsAdminOfApp(appId: file.Folder.AppId))
            || (file.Folder?.Roles?.Where(predicate: folderRole => userRoles.Contains(value: folderRole.RoleId))
                    .SelectMany(selector: folderRole => folderRole.Role?.Privileges ?? [])
                    .Contains(value: privilege) ?? false);
    }

    public static void RecomputePaths(this Folder folder)
    {
        string newPath = folder.ParentId != null
            ? $"{folder.Parent?.Path}/{folder.Name?.Replace(oldValue: " ", newValue: string.Empty)}"
            : $"{folder.Name?.Replace(oldValue: " ", newValue: string.Empty)}";

        if (newPath != folder.Path)
        {
            folder.Path = newPath;

            if (folder.SubFolders != null)
            {
                foreach (Folder subFolder in folder.SubFolders)
                {
                    subFolder.RecomputePaths();
                }
            }
        }
    }

    public static XElement ToWebDavResponse(this Folder folder, string urlBase, XNamespace ns, IEnumerable<string> requestedProperties)
    {
        XElement propStat = BuildFolderPropStatResponse(folder: folder, ns: ns, requestedProperties: requestedProperties);

        XElement response = new(
            name: ns + "response",
            new XElement(name: ns + "href", content: $"{urlBase}Core/App({folder.AppId})/DAV/{folder.Path}"),
            propStat);

        List<string> unsupportedProperties = ["getcontentlength", "executable", "checked-in", "checked-out"];

        foreach (string property in requestedProperties.Where(predicate: unsupportedProperties.Contains))
        {
            response.Add(content: new XElement(
                name: ns + "propStat",
                new XElement(name: ns + "prop", content: new XElement(name: ns + property)),
                new XElement(name: ns + "status", content: "HTTP/1.1 404 Not Found"),
                new XElement(name: ns + "responsedescription", content: $"Property {{DAV:}}{property} is not supported.")));
        }

        return response;
    }

    public static bool UserCan(this Folder folder, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?.Select(selector: role => role.RoleId)
            .ToArray() ?? [];

        return user.IsAdminOfApp(appId: folder.AppId)
            || (folder.Roles?.Where(predicate: folderRole => userRoles.Contains(value: folderRole.RoleId))
                    .SelectMany(selector: folderRole => folderRole.Role?.Privileges ?? [])
                    .Contains(value: privilege) ?? false);
    }

    private static XElement BuildFilePropStatResponse(File file, XNamespace ns, IEnumerable<string> requestedProperties) =>
        new(
            name: ns + "propstat",
            new XElement(
                name: ns + "prop",
                (!requestedProperties.Any() || requestedProperties.Contains(value: "creationdate")) ? new XElement(name: ns + "creationdate", content: file.CreatedOn.ToString(format: "s") + "Z") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "displayname")) ? new XElement(name: ns + "displayname", content: file.Name) : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "getlastmodified")) ? new XElement(name: ns + "getlastmodified", content: file.Contents.OrderByDescending(keySelector: content => content.Version)
                                                                                                                                                                                                                        .First().CreatedOn.ToString(format: "r")) : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "resourcetype")) ? new XElement(name: ns + "resourcetype") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "getcontentlength")) ? new XElement(name: ns + "getcontentlength", content: file.Contents.OrderByDescending(keySelector: content => content.Version)
                                                                                                                                                                                                                          .First().RawData.Length) : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "getcontenttype")) ? new XElement(name: ns + "getcontenttype", content: "application/octet-stream") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "lockdiscovery")) ? new XElement(name: ns + "lockdiscovery") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "supportedlock")) ? new XElement(name: ns + "supportedlock") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "ishidden")) ? new XElement(name: ns + "ishidden", content: 0) : null),
            new XElement(name: ns + "status", content: "HTTP/1.1 200 OK"));

    private static XElement BuildFolderPropStatResponse(Folder folder, XNamespace ns, IEnumerable<string> requestedProperties) =>
        new(
            name: ns + "propstat",
            new XElement(
                name: ns + "prop",
                (!requestedProperties.Any() || requestedProperties.Contains(value: "creationdate")) ? new XElement(name: ns + "creationdate", content: DateTimeOffset.Now.ToString(format: "s") + "Z") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "displayname")) ? new XElement(name: ns + "displayname", content: folder.Name) : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "getlastmodified")) ? new XElement(name: ns + "getlastmodified", content: DateTimeOffset.Now.ToString(format: "s") + "Z") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "resourcetype")) ? new XElement(name: ns + "resourcetype", content: new XElement(name: ns + "collection")) : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "lockdiscovery")) ? new XElement(name: ns + "lockdiscovery") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "supportedlock")) ? new XElement(name: ns + "supportedlock") : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "isfolder")) ? new XElement(name: ns + "isfolder", content: 1) : null,
                (!requestedProperties.Any() || requestedProperties.Contains(value: "ishidden")) ? new XElement(name: ns + "ishidden", content: 0) : null),
            new XElement(name: ns + "status", content: "HTTP/1.1 200 OK"));
}