using cCoder.Data.Models.Packaging;

namespace cCoder.DocumentManagement.Exposures.Setup;

public static partial class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        FolderRoles,
        PageRoles
    ];
}
