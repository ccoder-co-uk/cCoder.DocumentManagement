using cCoder.Data.Models.Packaging;

namespace cCoder.DocumentManagement.Exposures.Setup;

public static class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        FolderRoles,
        PageRoles
    ];

    static Package Components => new()
    {
        Name = "Document Management Components",
        Category = "DMS",
        Description = "Document Management Components.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "DMS",
  "Key": "Document Management",
  "ResourceKey": "DMS",
  "Script": "DMS = {\n    init: async function (app, container) {\n        app = app || session.app;\n        app = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\n        container = container || $('.component[name=DMS]');\n        \n        api.addToMetaCache([\n            {\n                \"Name\": \"Core\",\n                \"Types\": [\n                    [meta[DocumentManagement/Folder]]\n                ]\n            }]);\n\n        var tree = new ODataTree(new ODataTreeOptions()\n            .setElement($(\"[name=treeRoot]\", container))\n            .setEndpoint(\"DocumentManagement/Folder\")\n            .setODataAppend(\"?$expand=Files,SubFolders($top=1)&$filter=AppId eq \" + app.Id + \"&$orderby=Name\")\n        );\n        let typeInfo = await api.getType(\"DocumentManagement/Folder\");\n\n        tree.prepareData = (data) => {\n            model.prepareItem(data, typeInfo);\n            return { text: data.Name, spriteCssClass: \"folder\", type: \"Folder\", data: data, expanded: false, hasChildren: data.SubFolders.length > 0 };\n        };\n\n        tree.select = (e) => DMS.selectNode(e, tree, app, $(\".component[name=FolderManagement]\", container));\n        tree.init();\n\n        $(\"[name=treeRoot]\", container).on(\"contextmenu\", (e) => DMS.rightClick(e, app, tree, container));\n    },\n\n    resize: function (container) {\n        var headerHeight = $(\"body > header\").height();\n        var footerHeight = $(\"body > footer\").height();\n        var bodyHeight = $(\"body\").height();\n        container.height(bodyHeight - (headerHeight + footerHeight) - 3);\n        container.width(\"100%\");\n    },\n\n    selectNode: function (e, tree, app, listingContainer) {\n        if(!e || !e.node) { return; }\n        var folder = tree.dataItem(e.node).data;\n        tree.selectNode(e.node);\n        FolderManagement.init(app, listingContainer, folder, false);\n    },\n\n    rightClick: function (e, app, tree, container) {\n        e.preventDefault();\n\n        tree.selectNode(e.target);\n        var node = $(e.target).parents().filter(\"li\").first();\n        var nodeData = tree.dataItem(e.target);\n       \n        var contextMenu = new ContextMenuWidget(container);\n        contextMenu.commands.push({name: \"newChild\", icon:\"k-i-folderAddIcon\", text: \"[resource_displayname[newfolder]]\"});\n        if (nodeData) {\n            contextMenu.commands.push({name: \"delete\", icon: \"k-i-trashIcon\", text: \"[resource_displayname[delete]]\"});\n        }\n        contextMenu.commands.push({template:\"<li name='unpack'><span class='k-icon k-i-fileZipIcon'></span>[resource_displayname[unpackzipto]]</li>\"});\n        contextMenu.commands.push({name: \"rename\", icon: \"k-i-textboxIcon\", text: \"[resource_displayname[rename]]\"});\n        contextMenu.commands.push({name: \"downloadFolder\", icon: \"k-i-download\", text: \"[resource_displayname[download]]\"});\n        contextMenu.commands.push({name: \"folderProperties\", icon: \"k-i-folder-more\", text: \"[resource_displayname[properties]]\"});\n\n        contextMenu.init(e.pageX, e.pageY);\n\n        $(\"[name=newChild]\", contextMenu.contextMenuElement).on(\"click\", (e) => DMS.addNewFolder(e, app, tree, node));\n        $(\"[name=delete]\", contextMenu.contextMenuElement).on(\"click\", (e) => DMS.removeFolder(e, tree, node));\n        $(\"[name=rename]\", contextMenu.contextMenuElement).on(\"click\", (e) => DMS.renameFolder(e, tree, node));\n        $(\"[name=folderProperties]\", contextMenu.contextMenuElement).on(\"click\", async (e) => await DMS.showFolderProperties(e, app, nodeData.data));\n        $(\"[name=downloadFolder]\", contextMenu.contextMenuElement).on(\"click\", async (e) => await DMS.downloadFolder(e, nodeData.data));\n        \n        $(\"[name=unpack]\", contextMenu.contextMenuElement).on(\"click\",(e) => DMS.unpackFolder(e, tree, node, container));\n    },\n\n    unpackFolder: async function(e, tree, node, container) {\n      e.stopPropagation();\n      var folder = tree.dataItem(node).data;\n      var inputElement = $(\"<input id = 'file' type='file' style='visibility: hidden; position: absolute'/>\").appendTo(container);\n      $(inputElement).change(async (e) => {\n        api.file.upload(folder.Path + \"?unpack\",$(inputElement)[0].files[0], () => {\n            notification.success(\"[resource_displayname[unpacked]]\");\n        });\n\n      });\n      document.getElementById(\"file\").click();\n    },\n\n    renameFolder: async function (e, tree, node) {\n        e.preventDefault();\n        var dataItem = tree.dataItem(node);\n        if (!dataItem) {\n            console.error(\"Invalid node:\", node);\n            return;\n        }\n        var editorDialog = new EditorDialog({\n            title: \"[resource_displayname[renamefolder]]\",\n            fields: [{ field: \"Name\", title: \"[resource_displayname[name]]\" }],\n            confirm: \"[resource_displayname[update]]\",\n            data: dataItem.data\n        });\n        editorDialog.events.confirm = (e) => {\n            editorDialog.data.save(e);\n            notification.success('[resource_description[folderrenamedsuccessfully]]');\n            dataItem.set(\"text\", dataItem.data.Name);\n            editorDialog.events.close();\n        };\n\n        await editorDialog.init();\n    },\n\n\n    addNewFolder: async function (e, app, tree, node) {\n        e.preventDefault();\n\n        var folder = await model.item.createInstance(\"DocumentManagement/Folder\");\n        folder.AppId = app.Id;\n\n        var hasParent = node.length > 0 && !$(node).hasClass(\"k-treeview\");\n\n        if(hasParent) {\n            var item = tree.dataItem(node).data;\n            folder.ParentId = item.Id;\n        }\n\n        var editorDialog = new EditorDialog({\n            title: \"[resource_displayname[newfolder]]\",\n            confirm: \"<span class='k-icon k-i-plus'></span> [resource_displayname[create]]\",\n            fields: [\n                { field: \"Name\", title: \"[resource_displayname[name]]\" }\n            ],\n            data: folder\n        });\n\n        editorDialog.events.confirm = async (e) => {\n            await editorDialog.data.save(e);\n            if(hasParent) {\n                var item = tree.dataItem(node);\n                if(item.expanded) {\n                    tree.collapseNode(node);\n                    tree.expandNode(node);\n                 }\n            } else {\n                await tree.refresh();\n            }\n            editorDialog.events.close();\n        };\n\n        editorDialog.init();\n    },\n\n    removeFolder: async function (e, tree, node) {\n        var data = tree.dataItem(node).data;\n\n        var c = new ConfirmDialog({\n            title: \"[resource_displayname[DeleteFolderTitle]]: \" + data.Path,\n            question: \"[resource_displayname[deleteconfirmation]]\",\n            confirm: \"[resource_displayname[confirm]]\",\n            close: \"[resource_displayname[close]]\"\n        });\n\n        c.events.confirm = async function () {\n            await data.destroy(e);\n            notification.success('[resource_description[FolderDeletedSuccesfully]]');\n            tree.removeNode(node);\n            c.events.close();\n        };\n        c.init();\n    },\n\n    showFolderProperties: async function (e, app, folder) {\n        e.preventDefault();\n        var d = new Dialog({\n            title: \"[resource_displayname[folderproperties]]: \" + folder.Path,\n            height: 500,\n            width: 1000,\n            component: \"FolderProperties\"\n        });\n        d.init(async function (c) {\n            await FolderProperties.init(app, $(\".component[name=FolderProperties]\", d.element), folder);\n        });\n    },\n\n    downloadFolder: async function(e, folder) {\n        e.preventDefault();\n        var link = window.location.protocol + \"//\" + window.location.hostname + \"/Api/DMS/\" + folder.Path;\n        window.location.href = link;\n    }\n}",
  "Content": "<div name=\"splitter\" >\n\t<div class=\"panel left\">\n\t\t<div name=\"treeRoot\"></div>\n\t</div>\n\t<div  class=\"panel right\" name=\"workspace\" >\n\t\t[component[FolderManagement]]\n\t</div>\n</div>\n\n<style scoped>\n    .component[name=DMS]>[name=splitter] { margin: 0; border: none; box-shadow: none; display: flex; flex: 1; flex-direction: row; max-height: 100%; }\n\t.panel.left { min-width: 200px; max-width: 350px; border: [theme[border.style]]; overflow: auto;  }\n\t.panel.right { display: flex; flex:1; flex-direction: column; padding-bottom: 2px; }\n\t.panel.right > .component { margin: 0;  }\n    .component[name=DMS]  [name=treeRoot] { padding: 10px; }\n\tdiv[name=DMSContextMenu] { \n\t\tvertical-align: top; box-sizing: border-box; overflow: hidden;\n\t\tmargin: [theme[margins]]; padding: 0; background: [theme[colours.background]]; \n\t\tborder-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]];  box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]];  \n\t }\n</style>",
  "LastUpdated": "2025-05-30T18:44:57.6789397Z"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "FolderManagement",
  "Key": "DMS",
  "ResourceKey": "DMS",
  "Script": "var FolderManagement = {\n    init: async function (app, container, folder, readOnly, disablePaging=false) {\n        app = app || session.app;\n        container = container || $(\".component[name=FolderManagement]\");\n        \n        if(!folder)\n            return;\n        \n        readOnly = readOnly || false;\n        api.addToMetaCache([\n        {\n            \"Name\": \"Core\",\n            \"Types\": [\n                [meta[DocumentManagement/Folder]],\n                [meta[DocumentManagement/File]],\n                [meta[DocumentManagement/FileContent]]\n            ]\n        }]);\n        $(\"h3\", container).text(folder.Name);\n        $(\"div\", container).remove();\n\n        await FolderManagement.buildWidget($(container), folder, readOnly, app, disablePaging);\n    },\n\n    dragContainsFiles: (event) => (event.dataTransfer.types) ? event.dataTransfer.types.filter(r => r === \"Files\").length > 0 : false,\n\n    buildWidget: async function (element, folder, readOnly, app, disablePaging=false) {\n        var shiftPressed = false;\n        var ctrlPressed = false;\n        $(window).keydown((evt) => {\n            if(evt.which === 16) { shiftPressed = true; }\n            if(evt.which === 17) { ctrlPressed = true; }\n        }).keyup((evt) => {\n            if (evt.which === 16) { shiftPressed = false; }\n            if(evt.which === 17) { ctrlPressed = false; }\n        });\n\n        var config = {\n            endpoint: \"DocumentManagement/File\",\n            odataAppend: \"?$filter=FolderId eq \" + folder.Id + \"&$expand=Contents($select=CreatedOn;$orderBy=CreatedOn desc;$top=1)\"\n        };\n        var ds = await model.getDatasource(config);\n        \n\t\tvar folderGrid = new GridWidget(element, ds);\n        folderGrid.filterable = true;\n\t\tfolderGrid.groupable = false;\n        folderGrid.editable = false;\n        folderGrid.scrollable = true;\n        if(disablePaging) {\n            folderGrid.pageable = false;\n            folderGrid.filterable = false;\n        }\n\t\tfolderGrid.columns= [\n            { \n                title: '', \n                width: 30, \n                template: '<img src=\"/icons/#:Name.split(\".\")[Name.split(\".\").length-1]#.svg\" style=\"height: 25px;\"></img>' \n            },\n            { \n                field: \"Name\", \n                title: \"[resource_shortdisplayname[name]]\" \n            },\n            { \n                field: 'Size', \n                width: 80, \n                title: \"[resource_shortdisplayname[size]]\" \n            },\n            { \n                field: 'CreatedBy', \n                title: \"[resource_shortdisplayname[createdby]]\", \n                editable: false, \n\t\t\t\twidth: \"[theme[columns.small]]\" \n            },\n            { \n                field: 'CreatedOn', \n                title: \"[resource_shortdisplayname[createdon]]\", \n                editable: false, \n                format: \"{0:\" + type.dateFormat + \" HH:mm:ss}\", \n\t\t\t\twidth: \"[theme[columns.small]]\"  \n            },\n            {\n                field: 'Contents[0].CreatedOn',\n                title: \"[resource_shortdisplayname[lastupdated]]\",\n                editable: false,\n                template: `#: kendo.toString(new Date(Contents[0].CreatedOn), '${type.dateFormat} HH:mm:ss') #`,\n\t\t\t\twidth: \"[theme[columns.small]]\"\n            }\n        ];\n        folderGrid.columns.splice(0, 0, { selectable: true, width: 30 });\n        folderGrid.dataBound = function () {\n            $(\"[name=save]\", folderGrid.gridElement).on(\"click\", (e) => FolderManagement.saveFile(e, folderGrid));\n            folderGrid.gridElement.on(\"click\", \"tr.k-master-row\", (e) => {\n                if(shiftPressed) { FolderManagement.shiftClick(e, folderGrid); }\n                if(ctrlPressed) { FolderManagement.ctrlClick(e, folderGrid); }\n            });\n        };\n\n        var fileDropContainerWidget = new FileDropContainerWidget(folderGrid.gridElement);\n        fileDropContainerWidget.events.drop = (e) => FolderManagement.fileDrop(e, folderGrid, folder);\n        fileDropContainerWidget.init();\n\n        folderGrid.detailTemplate = kendo.template(\"<div name='fileVersionGrid'></div>\");\n        folderGrid.detailExpand = async (e) => await FolderManagement.expand(e, app, folderGrid, readOnly);\n        await folderGrid.init();\n\n        folderGrid.gridElement.on(\"contextmenu\", \"tr.k-master-row\", (e) => FolderManagement.rightClick(e, folderGrid, readOnly));\n        folderGrid.gridElement.on(\"click\", (e) => {\n            if(e.target.tagName.toLowerCase() === \"div\") {\n                FolderManagement.clearSelection(e, folderGrid);\n            }\n        });\n    },\n\n    clearSelection: function(e, grid) {\n        e.preventDefault();\n        grid.clearSelection();\n    },\n\n    ctrlClick: function(e, grid) {\n        var rowSelected = $(e.currentTarget).closest(\"tr\");\n        grid.kendoObject.select(rowSelected);\n    },\n\n    shiftClick: async function(e, grid) {\n        var rowSelected = $(\"tr.k-state-selected\", grid.gridElement).length  > 0;\n        if(rowSelected) {\n            var rowSelectedIndex = $(\"tr.k-state-selected\", grid.gridElement).index();\n            var currentRowIndex = $(e.currentTarget).closest(\"tr\").index();\n            var children = $(\"tr.k-state-selected\", grid.gridElement).parent().children();\n            grid.kendoObject.clearSelection();\n            if(currentRowIndex > rowSelectedIndex) {\n                for(let i = rowSelectedIndex; i <= currentRowIndex; i++) {\n                    grid.kendoObject.select(children[i]);\n                }\n            } else {\n                for(let i = currentRowIndex; i <= rowSelectedIndex; i++) {\n                    grid.kendoObject.select(children[i]);\n                }\n            }\n        }\n    },\n\n    rightClick: async function(e, grid, readOnly) {\n        e.preventDefault();\n        var item = $(e.currentTarget).closest(\"tr\");\n        grid.kendoObject.select(item);\n        var contextMenu = new ContextMenuWidget(grid.gridElement);\n\n        contextMenu.commands.push({name: \"download\", icon: \"k-i-download\", text: \"[resource_displayname[download]]\"});\n        if(!readOnly) {\n            contextMenu.commands.push({name: \"rename\", icon: \"k-i-edit-tools\", text: \"[resource_displayname[rename]]\"});\n            contextMenu.commands.push({name: \"delete\", icon: \"k-i-trashIcon\", text: \"[resource_displayname[delete]]\"});\n        }\n\n        contextMenu.init(e.clientX, e.clientY);\n        $(\"[name=download]\", contextMenu.contextMenuElement).on(\"click\", (e) => FolderManagement.downloadFiles(e, grid));\n        $(\"[name=rename]\", contextMenu.contextMenuElement).on(\"click\", (e) => FolderManagement.renameFiles(e, grid));\n        $(\"[name=delete]\", contextMenu.contextMenuElement).on(\"click\", (e) => FolderManagement.deleteFiles(e, grid));\n    },\n\n    downloadFiles: async function(e, grid) {\n        e.preventDefault();\n        var files = grid.select();\n        if(files.length === 1) {\n            window.location.href = session.apiRoot + \"DMS/\" + files[0].Path + \"?download\";\n        } else {\n            window.location.href = session.apiRoot + \"DMS/?downloadPaths=\" + files.map(f => f.Path).join();\n        }\n    },\n\n    renameFiles: async function(e, grid) {\n        e.preventDefault();\n        var files = grid.select();\n        var renameDialog = new Dialog({title: \"[resource_displayname[rename]]\", height: \"auto\"});\n        renameDialog.template = $(\"[name=renameDialog]\").first().html();\n        renameDialog.events.rename = async function() {\n            var newName = $(\"[name=newName]\", renameDialog.element).val();\n            if(files.length === 1) {\n                var folderPath = files[0].Path.substring(0, files[0].Path.lastIndexOf(\"/\")+1);\n                if(newName.indexOf(\".\") !== -1) {\n                    await api.post(\"DMS/\" + files[0].Path + \"?moveTo=\" + folderPath + newName).then(() => {\n                        notification.success(\"[resource_displayname[saved]]\");\n                        grid.refresh();\n                        renameDialog.events.close();\n                    }).catch((err) => error(err));\n                } else {\n                    var extension = files[0].Name.substring(files[0].Name.lastIndexOf(\".\"));\n                    await api.post(\"DMS/\" + files[0].Path + \"?moveTo=\" + folderPath + newName + extension)\n                    .then(() => {\n                        notification.success(\"[resource_displayname[saved]]\");\n                        grid.refresh();\n                        renameDialog.events.close();\n                    })\n                    .catch((err) => error(err));\n                }\n                grid.refresh();\n            } else {\n                if(newName.indexOf(\".\") !== -1) {\n                    error(\"[resource_displayname[batchextensionrenameunsupported]]\");\n                } else {\n                    for(let i = 0; i < files.length; i++) {\n                        var folderPath = files[i].Path.substring(0, files[i].Path.lastIndexOf(\"/\")+1);\n                        var extension = files[i].Name.substring(files[i].Name.lastIndexOf(\".\"));\n                        await api.post(\"DMS/\" + files[i].Path + \"?moveTo=\" + folderPath + newName + \"-\" + (i+1).toString() + extension).catch((err) => error(err));\n                    }\n                    notification.success(\"[resource_displayname[saved]]\");\n                    grid.refresh();\n                    renameDialog.events.close();\n                }\n            }\n        };\n        renameDialog.init(() => $(\"[name=newName]\").val(files[0].Name));\n    },\n\n    deleteFiles: async function(e, grid) {\n        e.preventDefault();\n        var files = grid.select();\n        for(let i = 0; i < files.length; i++) {\n            await api.file.destroy(files[i].Path).catch((err) => error(err));\n        }\n        grid.refresh();\n    },\n\t\n\texpand: async function (e, app, grid, readOnly) {\n\t\tif ($(e.detailRow).find(\".fileVersionGrid > *\").length == 0) {\n\t\t\tvar file = grid.dataItem(e.masterRow);\n\n            await loadComponent($('[name=fileVersionGrid]', e.detailRow), 'FileVersionGrid', async (c) => {\n                await c.init(app, $('[name=fileVersionGrid]', e.detailRow), file, readOnly);\n            });\n\t\t};\n\t},\n\n    saveFileVersion: async function(e, grid) {\n        e.preventDefault();\n\t\tvar fileVersion =  grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        await fileVersion.save().then(() => { notification.success(\"[resource_displayname[saved]]\"); })\n        .catch((err) => { error(err); });\n    },\n\n    saveFile: async function(e, grid) {\n        var file = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        var oldPath = file.Path;\n        var newPath = oldPath.substring(0, oldPath.lastIndexOf(\"/\")+1)+file.Name;\n        await api.post(\"DMS/\" + oldPath + \"?moveTo=\" + newPath).then(()=>{\n            notification.success(\"[resource_displayname[FileSaved]]\"); \n            grid.refresh();\n        }).catch((err)=>error(err));\n    },\n\n    deleteFileVersion: async function (e, grid, file) {\n        e.preventDefault();\n        var fileVersion = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        var d = new ConfirmDialog({\n            title: \"[resource_displayname[areyousure]]\",\n            question: \"[resource_displayname[areyousure]]\",\n            close: \"[resource_displayname[close]]\",\n            confirm: \"[resource_displayname[confirm]]\"\n        });\n        d.events.confirm = async function () {\n            await api.file.destroy(file.Path + \"?version=\" + fileVersion.Version).then(async () => {\n                //TODO: Figure out why Kendo is being a fussy shit with removing items from grid...\n                d.events.close();\n            }).catch((err) => { error(err); });\n        };\n        d.init();\n    },\n\n    deleteFile: async function (e, grid, version, file) {\n        e.preventDefault();\n        var d = new ConfirmDialog({\n            title: \"[resource_displayname[areyousure]]\",\n            question: \"[resource_displayname[areyousure]]\",\n            close: \"[resource_displayname[close]]\",\n            confirm: \"[resource_displayname[confirm]]\"\n        });\n        d.events.confirm = async function () {\n            await api.file.destroy(file.Path + \"?version=\" + version).then(() => {\n                grid.refresh();\n                d.events.close();\n            }).catch((err) => { error(err); });\n        };\n        d.init();\n    },\n\n    fileDrop: function (e, grid, folder) {\n        e.preventDefault();\n        e.stopPropagation();\n        var files = e.target.files;\n        if(files.length === 0) {\n            files = e.originalEvent.dataTransfer.files;\n        }\n        if (folder !== null) {\n            for (var i = 0; i < files.length; i++) {\n                var file = files[i];\n                api.file.upload(folder.Path + \"/\" + file.name, file, () => grid.refresh());\n            }\n        }\n        $(\"input[type=file]\", grid.gridElement).remove();\n    }\n};\t",
  "Content": "<script type=\"text/template\" name=\"renameDialog\">\n<ul class=\"fieldList\">\n    <li>\n        <label>[resource_displayname[newname]]</label>\n        <div class=\"value\">\n            <input type=\"text\" name=\"newName\" />\n        </div>\n    </li>\n</ul>\n<hr>\n<div class=\"value\">\n    <button name=\"rename\">[resource_displayname[rename]]</button>\n</div>\n</script>\n<style scoped>\n    .drop { position: relative; left: 20px; padding: 5px; background: #EEE; border: dashed 2px #CCC;  }\n    input[name=dropTarget]\t\t  { text-align: center; border: none; display: block; }\n   .download { padding: 10px; line-height: 30px; }\n   .icon { position:relative; height: 16px; width: 16px; }\n   \tdiv[name=FolderManagementContextMenu] { background: white; }\n</style>\n",
  "LastUpdated": "2026-04-20T10:20:09.0563737+01:00"
}
"""
            },
        ]
    };

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

    static Package FolderRoles => new()
    {
        Name = "Document Management Folder Roles",
        Category = "DMS",
        Description = "Document Management Folder Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "icons",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "icons",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "icons",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/flags",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/flags",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/flags",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/standarduserguide",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/standarduserguide",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/standarduserguide",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/csvtransactionprocessing",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/csvtransactionprocessing",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/csvtransactionprocessing",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/statemanagement",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/statemanagement",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/statemanagement",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data/transactions",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data/transactions",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data/transactions",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data/transactions/received",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data/transactions/received",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "data/transactions/received",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/dms",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/dms",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/dms",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/cultures",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/cultures",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/cultures",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/components",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/components",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/components",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/configuration",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/configuration",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/configuration",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/layouts",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/layouts",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/layouts",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/templates",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/templates",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/templates",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/theming",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/theming",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/theming",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/mail management",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/mail management",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/mail management",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/scheduling",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/scheduling",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/scheduling",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/log stream",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/log stream",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/log stream",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/security",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/security",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/security",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/resources",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/resources",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/app management/resources",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/cms",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/cms",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/cms",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/calendar management",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/calendar management",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/core documentation/calendar management",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/forgot your password",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/forgot your password",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/forgot your password",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/managing your profile",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/managing your profile",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/managing your profile",
  "Name": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/logging in",
  "Name": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/logging in",
  "Name": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/FolderRole",
                Data = """
{
  "Path": "content/documentation/sso documentation/logging in",
  "Name": "Guests"
}
"""
            },
        ]
    };

    static Package PageRoles => new()
    {
        Name = "Document Management Page Roles",
        Category = "DMS",
        Description = "Document Management Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/DocumentManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/DocumentManagement",
  "Role": "Users"
}
"""
            },
        ]
    };
}
