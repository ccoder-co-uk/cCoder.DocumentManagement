using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services;

public interface IDocumentManagementCurrentAppResolver
{
    App ResolveCurrentApp();
}

