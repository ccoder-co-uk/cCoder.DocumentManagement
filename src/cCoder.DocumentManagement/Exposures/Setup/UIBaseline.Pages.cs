using cCoder.Data.Models.Packaging;

namespace cCoder.DocumentManagement.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Pages => new()
    {
        Name = "Document Management Pages",
        Category = "DMS",
        Description = "Document Management Pages.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/DocumentManagement",
  "Name": "Document Management",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 7,
  "LastUpdated": "2024-04-04T16:36:36.2803634+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[dms]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Manage the folders and files within the application, here you are able to upload any Masterdata or Transaction files you'd like to  import.",
      "Keywords": "Documents, DMS, sample, page",
      "Title": "Document Management"
    }
  ]
}
"""
            },
        ]
    };
}