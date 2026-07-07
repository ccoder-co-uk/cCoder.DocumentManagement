using cCoder.Data.Models.Packaging;

namespace cCoder.DocumentManagement.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Resources => new()
    {
        Name = "Document Management Resources",
        Category = "DMS",
        Description = "Document Management Resources.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "addnewfoldertitle",
  "DisplayName": "Add Folder",
  "ShortDisplayName": "Add Folder",
  "Description": "Add Folder",
  "LastUpdated": "2022-03-18T10:41:54.1905294+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "FailedSavedChanges",
  "DisplayName": "Failed saving changes",
  "ShortDisplayName": "Failed saving changes",
  "Description": "Failing saving changes",
  "LastUpdated": "2022-03-18T10:41:54.1905345+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "size",
  "DisplayName": "Size",
  "ShortDisplayName": "Size",
  "Description": "Size",
  "LastUpdated": "2022-03-18T10:41:54.1905395+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "areyousure",
  "DisplayName": "Are you sure?",
  "ShortDisplayName": "Are you sure?",
  "Description": "Are you sure?",
  "LastUpdated": "2022-03-18T10:41:54.1905446+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "size",
  "DisplayName": "Taille",
  "ShortDisplayName": "Taille",
  "Description": "Taille",
  "LastUpdated": "2022-03-18T10:41:54.1905497+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "newname",
  "DisplayName": "Nouveau nom",
  "ShortDisplayName": "Nouveau nom",
  "Description": "Nouveau nom",
  "LastUpdated": "2022-03-18T10:41:54.1905548+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "folderdeletedsuccesfully",
  "DisplayName": "Folder Deleted Successfully",
  "ShortDisplayName": "Folder Deleted Successfully",
  "Description": "Folder Deleted Succesfully",
  "LastUpdated": "2022-03-18T10:41:54.1905599+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "deletefolder",
  "DisplayName": "Delete Folder",
  "ShortDisplayName": "Delete Folder",
  "Description": "Delete Folder",
  "LastUpdated": "2022-03-18T10:41:54.1905665+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "rename",
  "DisplayName": "Rename",
  "ShortDisplayName": "Rename",
  "Description": "Rename",
  "LastUpdated": "2022-03-18T10:41:54.1905717+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1905769+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "FolderAddedSuccesfully",
  "DisplayName": "Folder added succesfully",
  "ShortDisplayName": "Folder added succesfully",
  "Description": "Folder added succesfully",
  "LastUpdated": "2022-03-18T10:41:54.1905819+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "successfullysavedchanges",
  "DisplayName": "Successfully saved changes",
  "ShortDisplayName": "Successfully saved changes",
  "Description": "Successfully saved changes",
  "LastUpdated": "2022-03-18T10:41:54.190587+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "properties",
  "DisplayName": "Properties",
  "ShortDisplayName": "Properties",
  "Description": "Properties",
  "LastUpdated": "2022-03-18T10:41:54.1905921+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "newname",
  "DisplayName": "New Name",
  "ShortDisplayName": "New Name",
  "Description": "newname",
  "LastUpdated": "2022-03-18T10:41:54.1905972+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "newfolder",
  "DisplayName": "New Folder",
  "ShortDisplayName": "New Folder",
  "Description": "New Folder",
  "LastUpdated": "2022-03-18T10:41:54.1906023+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "folderpropertiestitle",
  "DisplayName": "Folder Properties",
  "ShortDisplayName": "Folder Properties",
  "Description": "Folder Properties",
  "LastUpdated": "2022-03-18T10:41:54.1906091+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "folderdragerror",
  "DisplayName": "A folder cannot be placed inside itself!",
  "ShortDisplayName": "A folder cannot be placed inside itself!",
  "Description": "A folder cannot be placed inside itself!",
  "LastUpdated": "2022-03-18T10:41:54.190666+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "renamefoldertitle",
  "DisplayName": "Renommer le dossier",
  "ShortDisplayName": "Renommer le dossier",
  "Description": "Renommer le dossier",
  "LastUpdated": "2022-03-18T10:41:54.1906711+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "folderpropertiestitle",
  "DisplayName": "Propriétés du dossier",
  "ShortDisplayName": "Propriétés du dossier",
  "Description": "Propriétés du dossier",
  "LastUpdated": "2022-03-18T10:41:54.1906761+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "renamefoldertitle",
  "DisplayName": "Rename Folder",
  "ShortDisplayName": "Rename Folder",
  "Description": "Rename Folder",
  "LastUpdated": "2022-03-18T10:41:54.1906825+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "addnewfoldertitle",
  "DisplayName": "Ajouter le dossier",
  "ShortDisplayName": "Ajouter le dossier",
  "Description": "Ajouter le dossier",
  "LastUpdated": "2022-03-18T10:41:54.1906877+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "pleasenamethenewfolder",
  "DisplayName": "Please name the folder.",
  "ShortDisplayName": "Please name the folder.",
  "Description": "Please name the folder.",
  "LastUpdated": "2022-03-18T10:41:54.1906928+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "pleasenamethenewfolder",
  "DisplayName": "Veuillez nommer le dossier",
  "ShortDisplayName": "Veuillez nommer le dossier",
  "Description": "Veuillez nommer le dossier",
  "LastUpdated": "2022-03-18T10:41:54.1906978+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "folderroles",
  "DisplayName": "Rôles de dossier",
  "ShortDisplayName": "Rôles de dossier",
  "Description": "Rôles de dossier",
  "LastUpdated": "2022-03-18T10:41:54.1907028+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "doyouwanttodeletethisfolder",
  "DisplayName": "Do you want to delete this folder?",
  "ShortDisplayName": "Do you want to delete this folder?",
  "Description": "Do you want to delete this folder?",
  "LastUpdated": "2022-03-18T10:41:54.1907079+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "FolderDeletedSuccesfully",
  "DisplayName": "Folder deleted successfully",
  "ShortDisplayName": "Folder deleted successfully",
  "Description": "Folder deleted successfully",
  "LastUpdated": "2022-03-18T10:41:54.1907128+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "folderproperties",
  "DisplayName": "Folder Properties",
  "ShortDisplayName": "Folder Properties",
  "Description": "Folder Properties",
  "LastUpdated": "2022-03-18T10:41:54.1907178+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "confirm",
  "DisplayName": "Confirm",
  "ShortDisplayName": "Confirm",
  "Description": "Confirm",
  "LastUpdated": "2022-03-18T10:41:54.1907245+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "folderroles",
  "DisplayName": "Folder Roles",
  "ShortDisplayName": "Folder Roles",
  "Description": "The Folder role",
  "LastUpdated": "2022-03-18T10:41:54.1907295+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "areyousureyouwanttodeletethisfolder",
  "DisplayName": "Voulez-vous vraiment supprimer ce dossier?",
  "ShortDisplayName": "Voulez-vous vraiment supprimer ce dossier?",
  "Description": "Voulez-vous vraiment supprimer ce dossier?",
  "LastUpdated": "2022-03-18T10:41:54.1907346+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "DoYouWantToDeleteThisFolderTitle",
  "DisplayName": "Voulez-vous supprimer ce dossier?",
  "ShortDisplayName": "Voulez-vous supprimer ce dossier?",
  "Description": "Voulez-vous supprimer ce dossier?",
  "LastUpdated": "2022-03-18T10:41:54.1907396+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "GonnaDeleteThis",
  "DisplayName": "gggg",
  "ShortDisplayName": "gggg",
  "Description": "gggg",
  "LastUpdated": "2022-03-18T10:41:54.1907446+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "deleteconfirmation",
  "DisplayName": "Voulez-vous vraiment supprimer ce dossier? Tous les fichiers à l'intérieur seront également supprimés.",
  "ShortDisplayName": "Voulez-vous vraiment supprimer ce dossier? Tous les fichiers à l'intérieur seront également supprimés.",
  "Description": "Voulez-vous vraiment supprimer ce dossier? Tous les fichiers à l'intérieur seront également supprimés.",
  "LastUpdated": "2022-03-18T10:41:54.1907497+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "confirm",
  "DisplayName": "Confirmer",
  "ShortDisplayName": "Confirmer",
  "Description": "Confirmer",
  "LastUpdated": "2022-03-18T10:41:54.1907547+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "DoYouWantToDeleteThisFolderTitle",
  "DisplayName": "Do you want to delete this folder?",
  "ShortDisplayName": "Do you want to delete this folder?",
  "Description": "Do you want to delete this folder?",
  "LastUpdated": "2022-03-18T10:41:54.1907617+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "DeleteFolderTitle",
  "DisplayName": "Supprimer le dossier",
  "ShortDisplayName": "Supprimer le dossier",
  "Description": "Supprimer le dossier",
  "LastUpdated": "2022-03-18T10:41:54.1907821+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "areyousureyouwanttodeletethisfolder",
  "DisplayName": "Are you sure you want to delete this folder?",
  "ShortDisplayName": "Are you sure you want to delete this folder?",
  "Description": "Are you sure you want to delete this folder?",
  "LastUpdated": "2022-03-18T10:41:54.1907922+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "folderwillbepermanentlylost",
  "DisplayName": "Le dossier sera définitivement perdu,",
  "ShortDisplayName": "Le dossier sera définitivement perdu,",
  "Description": "Le dossier sera définitivement perdu,",
  "LastUpdated": "2022-03-18T10:41:54.1907974+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "folderwillbepermanentlylost",
  "DisplayName": "The folder will be permanently lost,",
  "ShortDisplayName": "The folder will be permanently lost,",
  "Description": "The folder will be permanently lost,",
  "LastUpdated": "2022-03-18T10:41:54.1908039+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "nogoback",
  "DisplayName": "Non, reviens",
  "ShortDisplayName": "Non, reviens",
  "Description": "Non, reviens",
  "LastUpdated": "2022-03-18T10:41:54.190809+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "nogoback",
  "DisplayName": "No, go back.",
  "ShortDisplayName": "No, go back.",
  "Description": "No, go back.",
  "LastUpdated": "2022-03-18T10:41:54.1908141+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "yesdelete",
  "DisplayName": "Oui, supprimez.",
  "ShortDisplayName": "Oui, supprimez.",
  "Description": "Oui, supprimez.",
  "LastUpdated": "2022-03-18T10:41:54.1908191+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "deleteconfirmation",
  "DisplayName": "Are you sure you want to delete this folder?",
  "ShortDisplayName": "Are you sure you want to delete this folder? ",
  "Description": "Are you sure you want to delete this folder?",
  "LastUpdated": "2022-03-18T10:41:54.1908242+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "yesdelete",
  "DisplayName": "Yes, delete.",
  "ShortDisplayName": "Yes, delete.",
  "Description": "Yes, delete.",
  "LastUpdated": "2022-03-18T10:41:54.1908292+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "andallfoldersinsideittoo",
  "DisplayName": "and all folders inside it too.",
  "ShortDisplayName": "and all folders inside it too.",
  "Description": "and all folders inside it too.",
  "LastUpdated": "2022-03-18T10:41:54.1908341+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "doyouwanttodeletethisfolder",
  "DisplayName": "Voulez-vous supprimer ce dossier?",
  "ShortDisplayName": "Voulez-vous supprimer ce dossier?",
  "Description": "Voulez-vous supprimer ce dossier?",
  "LastUpdated": "2022-03-18T10:41:54.1908407+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "andallfoldersinsideittoo",
  "DisplayName": "et tous les dossiers à l'intérieur.",
  "ShortDisplayName": "et tous les dossiers à l'intérieur.",
  "Description": "et tous les dossiers à l'intérieur.",
  "LastUpdated": "2022-03-18T10:41:54.1908561+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "folderdeletedsuccesfully",
  "DisplayName": "Dossier supprimé avec succès",
  "ShortDisplayName": "Dossier supprimé avec succès",
  "Description": "Dossier supprimé avec succès",
  "LastUpdated": "2022-03-18T10:41:54.190861+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "DMS",
  "Name": "DeleteFolderTitle",
  "DisplayName": "Delete Folder",
  "ShortDisplayName": "Delete Folder",
  "Description": "Delete Folder",
  "LastUpdated": "2022-03-18T10:41:54.190866+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "DMS",
  "Name": "newfolder",
  "DisplayName": "Nouveau Dossier",
  "ShortDisplayName": "Nouveau Dossier",
  "Description": "Nouveau Dossier",
  "LastUpdated": "2023-01-04T12:32:37.1401249+00:00"
}
"""
            },
        ]
    };
}