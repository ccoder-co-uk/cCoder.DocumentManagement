// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Xml.Linq;
using cCoder.Data.Models.Security;

namespace cCoder.Data.Models.DMS;

public static class FolderExtensions
{
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

    public static XElement ToWebDavResponse(
        this Folder folder,
        string urlBase,
        XNamespace ns,
        IEnumerable<string> requestedProperties)
    {
        XElement propStat = BuildPropStatResponse(
            folder: folder,
            ns: ns,
            requestedProperties: requestedProperties);

        XElement response = new(
            name: ns + "response",
            new XElement(
                name: ns + "href",
                content:
                    $"{urlBase}Core/App({folder.AppId})/DAV/{folder.Path}"),
            propStat);

        List<string> unsupportedProperties =
            ["getcontentlength", "executable", "checked-in", "checked-out"];

        foreach (string property in requestedProperties.Where(
            predicate: unsupportedProperties.Contains))
        {
            response.Add(content: new XElement(
                name: ns + "propStat",
                new XElement(
                    name: ns + "prop",
                    content: new XElement(name: ns + property)),
                new XElement(
                    name: ns + "status",
                    content: "HTTP/1.1 404 Not Found"),
                new XElement(
                    name: ns + "responsedescription",
                    content:
                        $"Property {{DAV:}}{property} is not supported.")));
        }

        return response;
    }

    public static bool UserCan(
        this Folder folder,
        User user,
        string privilege)
    {
        Guid[] userRoles = user?.Roles?
            .Select(selector: role => role.RoleId)
            .ToArray() ?? [];

        return user.IsAdminOfApp(appId: folder.AppId)
            || (folder.Roles?
                .Where(predicate: folderRole =>
                    userRoles.Contains(value: folderRole.RoleId))
                .SelectMany(selector: folderRole =>
                    folderRole.Role?.Privileges ?? [])
                .Contains(value: privilege) ?? false);
    }

    private static XElement BuildPropStatResponse(
        Folder folder,
        XNamespace ns,
        IEnumerable<string> requestedProperties) =>
        new(
            name: ns + "propstat",
            new XElement(
                name: ns + "prop",
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "creationdate"))
                        ? new XElement(
                            name: ns + "creationdate",
                            content:
                                DateTimeOffset.Now.ToString(format: "s") + "Z")
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "displayname"))
                        ? new XElement(
                            name: ns + "displayname",
                            content: folder.Name)
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "getlastmodified"))
                        ? new XElement(
                            name: ns + "getlastmodified",
                            content:
                                DateTimeOffset.Now.ToString(format: "s") + "Z")
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "resourcetype"))
                        ? new XElement(
                            name: ns + "resourcetype",
                            content: new XElement(name: ns + "collection"))
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "lockdiscovery"))
                        ? new XElement(name: ns + "lockdiscovery")
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "supportedlock"))
                        ? new XElement(name: ns + "supportedlock")
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "isfolder"))
                        ? new XElement(name: ns + "isfolder", content: 1)
                        : null,
                (!requestedProperties.Any()
                    || requestedProperties.Contains(value: "ishidden"))
                        ? new XElement(name: ns + "ishidden", content: 0)
                        : null),
            new XElement(
                name: ns + "status",
                content: "HTTP/1.1 200 OK"));
}