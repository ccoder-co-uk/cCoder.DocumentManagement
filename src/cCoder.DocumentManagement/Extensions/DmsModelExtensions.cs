using System.Xml.Linq;
using cCoder.Data.Models.Security;

namespace cCoder.Data.Models.DMS;

public static class DmsModelExtensions
{
    public static Stream GetContent(this File file, int version = 0) =>
        version > 0
            ? new MemoryStream(file.Contents.FirstOrDefault(content => content.Version == version)?.RawData)
            : new MemoryStream(file.Contents.OrderBy(content => content.Version).Last().RawData);

    public static void RecomputePath(this File file) =>
        file.Path = file.FolderId != Guid.Empty
            ? $"{file.Folder?.Path}/{file.Name}".ToLowerInvariant()
            : file.Name?.ToLowerInvariant();

    public static XElement ToWebDavResponse(this File file, string urlBase, XNamespace ns, IEnumerable<string> requestedProperties)
    {
        XElement propStat = BuildFilePropStatResponse(file, ns, requestedProperties);

        XElement response = new(
            ns + "response",
            new XElement(ns + "href", $"{urlBase}Core/App({file.Folder.AppId})/DAV/{file.Path}"),
            propStat);

        List<string> unsupportedProperties = ["executable", "checked-in", "checked-out"];

        foreach (string property in requestedProperties.Where(unsupportedProperties.Contains))
        {
            response.Add(new XElement(
                ns + "propStat",
                new XElement(ns + "prop", new XElement(ns + property)),
                new XElement(ns + "status", "HTTP/1.1 404 Not Found"),
                new XElement(ns + "responsedescription", $"Property {{DAV:}}{property} is not supported.")));
        }

        return response;
    }

    public static bool UserCan(this File file, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?.Select(role => role.RoleId).ToArray() ?? [];

        return (file.Folder != null && user.IsAdminOfApp(file.Folder.AppId))
            || (file.Folder?.Roles?.Where(folderRole => userRoles.Contains(folderRole.RoleId))
                    .SelectMany(folderRole => folderRole.Role?.Privileges ?? [])
                    .Contains(privilege) ?? false);
    }

    public static void RecomputePaths(this Folder folder)
    {
        string newPath = folder.ParentId != null
            ? $"{folder.Parent?.Path}/{folder.Name?.Replace(" ", string.Empty)}"
            : $"{folder.Name?.Replace(" ", string.Empty)}";

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
        XElement propStat = BuildFolderPropStatResponse(folder, ns, requestedProperties);
        XElement response = new(
            ns + "response",
            new XElement(ns + "href", $"{urlBase}Core/App({folder.AppId})/DAV/{folder.Path}"),
            propStat);

        List<string> unsupportedProperties = ["getcontentlength", "executable", "checked-in", "checked-out"];

        foreach (string property in requestedProperties.Where(unsupportedProperties.Contains))
        {
            response.Add(new XElement(
                ns + "propStat",
                new XElement(ns + "prop", new XElement(ns + property)),
                new XElement(ns + "status", "HTTP/1.1 404 Not Found"),
                new XElement(ns + "responsedescription", $"Property {{DAV:}}{property} is not supported.")));
        }

        return response;
    }

    public static bool UserCan(this Folder folder, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?.Select(role => role.RoleId).ToArray() ?? [];

        return user.IsAdminOfApp(folder.AppId)
            || (folder.Roles?.Where(folderRole => userRoles.Contains(folderRole.RoleId))
                    .SelectMany(folderRole => folderRole.Role?.Privileges ?? [])
                    .Contains(privilege) ?? false);
    }

    private static XElement BuildFilePropStatResponse(File file, XNamespace ns, IEnumerable<string> requestedProperties) =>
        new(
            ns + "propstat",
            new XElement(
                ns + "prop",
                (!requestedProperties.Any() || requestedProperties.Contains("creationdate")) ? new XElement(ns + "creationdate", file.CreatedOn.ToString("s") + "Z") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("displayname")) ? new XElement(ns + "displayname", file.Name) : null,
                (!requestedProperties.Any() || requestedProperties.Contains("getlastmodified")) ? new XElement(ns + "getlastmodified", file.Contents.OrderByDescending(content => content.Version).First().CreatedOn.ToString("r")) : null,
                (!requestedProperties.Any() || requestedProperties.Contains("resourcetype")) ? new XElement(ns + "resourcetype") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("getcontentlength")) ? new XElement(ns + "getcontentlength", file.Contents.OrderByDescending(content => content.Version).First().RawData.Length) : null,
                (!requestedProperties.Any() || requestedProperties.Contains("getcontenttype")) ? new XElement(ns + "getcontenttype", "application/octet-stream") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("lockdiscovery")) ? new XElement(ns + "lockdiscovery") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("supportedlock")) ? new XElement(ns + "supportedlock") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("ishidden")) ? new XElement(ns + "ishidden", 0) : null),
            new XElement(ns + "status", "HTTP/1.1 200 OK"));

    private static XElement BuildFolderPropStatResponse(Folder folder, XNamespace ns, IEnumerable<string> requestedProperties) =>
        new(
            ns + "propstat",
            new XElement(
                ns + "prop",
                (!requestedProperties.Any() || requestedProperties.Contains("creationdate")) ? new XElement(ns + "creationdate", DateTimeOffset.Now.ToString("s") + "Z") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("displayname")) ? new XElement(ns + "displayname", folder.Name) : null,
                (!requestedProperties.Any() || requestedProperties.Contains("getlastmodified")) ? new XElement(ns + "getlastmodified", DateTimeOffset.Now.ToString("s") + "Z") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("resourcetype")) ? new XElement(ns + "resourcetype", new XElement(ns + "collection")) : null,
                (!requestedProperties.Any() || requestedProperties.Contains("lockdiscovery")) ? new XElement(ns + "lockdiscovery") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("supportedlock")) ? new XElement(ns + "supportedlock") : null,
                (!requestedProperties.Any() || requestedProperties.Contains("isfolder")) ? new XElement(ns + "isfolder", 1) : null,
                (!requestedProperties.Any() || requestedProperties.Contains("ishidden")) ? new XElement(ns + "ishidden", 0) : null),
            new XElement(ns + "status", "HTTP/1.1 200 OK"));
}
