namespace cCoder.DocumentManagement.Models;

public class DocumentManagementPackage
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public string SourceApi { get; set; }

    public virtual ICollection<DocumentManagementPackageItem> Items { get; set; }

    public DocumentManagementPackage() { }

    public DocumentManagementPackage(string name)
    {
        Name = name;
    }
}

