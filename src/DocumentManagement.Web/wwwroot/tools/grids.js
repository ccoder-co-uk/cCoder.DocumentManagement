window.DocumentManagementGrids = {
    apiRoot: "/Api/Core",

    configs: {
        Folder: {
            name: "Folder",
            title: "Folder",
            key: "Id",
            fields: {
                Id: { label: "Id", readonly: true, create: false },
                AppId: { label: "App Id", type: "number" },
                ParentId: { label: "Parent Id", nullable: true },
                Name: { label: "Name" },
                Path: { label: "Path" }
            },
            columns: ["Name", "Path", "AppId", "ParentId", "Id"]
        },
        File: {
            name: "File",
            title: "File",
            key: "Id",
            context: { field: "FolderId", type: "folder" },
            fields: {
                Id: { label: "Id", readonly: true, create: false },
                FolderId: { label: "Folder Id", readonly: true },
                Name: { label: "Name" },
                Description: { label: "Description" },
                Path: { label: "Path" },
                MimeType: { label: "Mime Type" },
                Size: { label: "Size" },
                CreatedBy: { label: "Created By" }
            },
            columns: ["Name", "Path", "MimeType", "Size", "Description", "Id"]
        },
        FileContent: {
            name: "FileContent",
            title: "File Content",
            key: "Id",
            context: { field: "FileId", type: "file" },
            fields: {
                Id: { label: "Id", readonly: true, create: false },
                FileId: { label: "File Id", readonly: true },
                Description: { label: "Description" },
                Size: { label: "Size" },
                CreatedBy: { label: "Created By" },
                Version: { label: "Version", type: "number" },
                RawDataText: { label: "Raw Data Text", type: "textarea", payload: false }
            },
            columns: ["Version", "Description", "Size", "CreatedBy", "Id"]
        },
        FolderRole: {
            name: "FolderRole",
            title: "Folder Role",
            composite: true,
            context: { field: "FolderId", type: "folder" },
            fields: {
                FolderId: { label: "Folder Id", readonly: true },
                RoleId: { label: "Role Id" }
            },
            columns: ["FolderId", "RoleId"]
        }
    },

    init: function () {
        document.getElementById("create-folder")
            ?.addEventListener("click", () => this.openEditor(this.configs.Folder, null, null));

        this.loadFolders();
    },

    loadFolders: async function () {
        try {
            const rows = await this.read(this.configs.Folder);
            this.renderFolderGrid(rows);
            DocumentManagementApi.notify("Ready");
        } catch (error) {
            DocumentManagementApi.notify(error.message, true);
        }
    },

    read: async function (config, context) {
        let url = `${this.apiRoot}/${config.name}?$top=500`;

        if (config.context) {
            const value = this.contextValue(config.context.type, context);
            url += `&$filter=${encodeURIComponent(`${config.context.field} eq ${value}`)}`;
        }

        const body = await DocumentManagementApi.get(url);
        return DocumentManagementApi.unwrapCollection(body);
    },

    renderFolderGrid: function (rows) {
        const grid = document.getElementById("folder-grid");
        grid.innerHTML = this.tableHtml(this.configs.Folder, rows, "Folder");

        grid.querySelectorAll("[data-action]").forEach(button =>
            button.addEventListener("click", event => this.onAction(event)));
    },

    tableHtml: function (config, rows, scope, context = null) {
        const headers = [
            `<th class="dm-expand-column"></th>`,
            ...config.columns.map(column => `<th>${this.escape(this.label(config, column))}</th>`),
            `<th>Actions</th>`
        ].join("");
        const body = rows.length === 0
            ? `<tr><td colspan="${config.columns.length + 2}" class="dm-empty">No ${this.escape(config.title)} rows found.</td></tr>`
            : rows.map(row => this.rowHtml(config, row, scope, context)).join("");

        return `<table class="dm-table" data-scope="${scope}">` +
            `<thead><tr>${headers}</tr></thead>` +
            `<tbody>${body}</tbody>` +
            `</table>`;
    },

    rowHtml: function (config, row, scope, context) {
        const rowKey = this.rowKey(config, row);
        const values = config.columns
            .map(column => `<td>${this.escape(this.displayValue(row[column]))}</td>`)
            .join("");
        const expandButton = this.canExpand(config)
            ? `<button data-action="toggle" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">+</button>`
            : "";

        this.storeRow(scope, rowKey, row, context);

        return `<tr data-row-key="${this.escape(rowKey)}">` +
            `<td class="dm-expand-column">${expandButton}</td>` +
            values +
            `<td class="dm-actions">` +
            `<button data-action="edit" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">Edit</button>` +
            `<button data-action="delete" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">Delete</button>` +
            `</td>` +
            `</tr>`;
    },

    store: {},

    storeRow: function (scope, key, row, context) {
        this.store[scope] = this.store[scope] || {};
        this.store[scope][key] = { row, context };
    },

    stored: function (scope, key) {
        return this.store[scope]?.[key] ?? null;
    },

    onAction: async function (event) {
        const button = event.currentTarget;
        const action = button.dataset.action;
        const stored = this.stored(button.dataset.scope, button.dataset.key);

        if (!stored) {
            return;
        }

        if (action === "toggle") {
            await this.toggleDetails(button, stored);
            return;
        }

        if (action === "edit") {
            this.openEditor(this.configForScope(button.dataset.scope), stored.row, stored.context);
            return;
        }

        if (action === "delete") {
            await this.deleteRow(this.configForScope(button.dataset.scope), stored.row, stored.context);
        }
    },

    toggleDetails: async function (button, stored) {
        const row = button.closest("tr");
        const existing = row.nextElementSibling;

        if (existing?.classList.contains("dm-detail-row")) {
            existing.remove();
            button.textContent = "+";
            return;
        }

        button.textContent = "-";
        const detailRow = document.createElement("tr");
        detailRow.className = "dm-detail-row";
        detailRow.innerHTML = `<td colspan="${row.children.length}">${this.detailShellHtml(stored.row)}</td>`;
        row.after(detailRow);

        const config = this.configForScope(button.dataset.scope);

        if (config.name === "Folder") {
            await this.loadFolderDetails(detailRow, stored.row);
            return;
        }

        if (config.name === "File") {
            await this.loadFileDetails(detailRow, stored.row);
        }
    },

    detailShellHtml: function () {
        return `<div class="dm-detail">` +
            `<div class="dm-tabs">` +
            `<button class="active" data-detail-tab="files" type="button">Files</button>` +
            `<button data-detail-tab="roles" type="button">Folder Roles</button>` +
            `</div>` +
            `<section class="dm-tab-panel active" data-detail-panel="files">` +
            `<div class="dm-detail-toolbar"><button data-create-child="File" type="button">Create File</button></div>` +
            `<div data-child-grid="File"></div>` +
            `</section>` +
            `<section class="dm-tab-panel" data-detail-panel="roles">` +
            `<div class="dm-detail-toolbar"><button data-create-child="FolderRole" type="button">Create Folder Role</button></div>` +
            `<div data-child-grid="FolderRole"></div>` +
            `</section>` +
            `</div>`;
    },

    loadFolderDetails: async function (detailRow, folder) {
        const context = { folder: folder.Id };

        detailRow.querySelectorAll("[data-detail-tab]").forEach(tab =>
            tab.addEventListener("click", () => this.showDetailTab(detailRow, tab.dataset.detailTab)));

        detailRow.querySelectorAll("[data-create-child]").forEach(button =>
            button.addEventListener("click", () => {
                const config = this.configs[button.dataset.createChild];
                this.openEditor(config, null, context, () => this.loadFolderDetails(detailRow, folder));
            }));

        await this.renderChildGrid(detailRow, this.configs.File, context);
        await this.renderChildGrid(detailRow, this.configs.FolderRole, context);
    },

    loadFileDetails: async function (detailRow, file) {
        const context = { file: file.Id };
        detailRow.querySelector("td").innerHTML =
            `<div class="dm-detail">` +
            `<div class="dm-detail-toolbar"><button data-create-child="FileContent" type="button">Create File Content</button></div>` +
            `<div data-child-grid="FileContent"></div>` +
            `</div>`;

        detailRow.querySelector("[data-create-child='FileContent']").addEventListener("click", () =>
            this.openEditor(this.configs.FileContent, null, context, () => this.loadFileDetails(detailRow, file)));

        await this.renderChildGrid(detailRow, this.configs.FileContent, context);
    },

    renderChildGrid: async function (container, config, context) {
        const grid = container.querySelector(`[data-child-grid='${config.name}']`);
        const rows = await this.read(config, context);
        const scope = `${config.name}-${this.contextValue(config.context.type, context)}`;
        grid.innerHTML = this.tableHtml(config, rows, scope, context);

        grid.querySelectorAll("[data-action]").forEach(button =>
            button.addEventListener("click", event => this.onAction(event)));
    },

    showDetailTab: function (detailRow, tabName) {
        detailRow.querySelectorAll("[data-detail-tab]").forEach(tab =>
            tab.classList.toggle("active", tab.dataset.detailTab === tabName));
        detailRow.querySelectorAll("[data-detail-panel]").forEach(panel =>
            panel.classList.toggle("active", panel.dataset.detailPanel === tabName));
    },

    canExpand: function (config) {
        return config.name === "Folder" || config.name === "File";
    },

    configForScope: function (scope) {
        const name = scope.split("-")[0];
        return this.configs[name];
    },

    openEditor: function (config, row, context, afterSave) {
        const dialog = document.getElementById("editor-dialog");
        const fields = document.getElementById("editor-fields");
        document.getElementById("editor-title").textContent = `${row ? "Edit" : "Create"} ${config.title}`;
        fields.innerHTML = Object.entries(config.fields)
            .filter(([, field]) => row || field.create !== false)
            .map(([name, field]) => this.fieldHtml(name, field, row, context, config))
            .join("");

        const form = dialog.querySelector("form");
        form.onsubmit = async event => {
            event.preventDefault();

            if (event.submitter?.id === "editor-close") {
                dialog.close();
                return;
            }

            await this.saveEditor(config, row, context);
            dialog.close();

            if (afterSave) {
                await afterSave();
            } else {
                await this.loadFolders();
            }
        };

        dialog.showModal();
    },

    fieldHtml: function (name, field, row, context, config) {
        const contextValue = config.context?.field === name
            ? this.contextValue(config.context.type, context)
            : null;
        const value = contextValue ?? row?.[name] ?? this.defaultValue(name, field);
        const readonly = field.readonly || contextValue !== null ? "readonly" : "";
        const input = field.type === "textarea"
            ? `<textarea name="${name}" ${readonly}>${this.escape(value)}</textarea>`
            : `<input name="${name}" value="${this.escape(value)}" ${readonly}>`;

        return `<label><span>${this.escape(field.label)}</span>${input}</label>`;
    },

    saveEditor: async function (config, row, context) {
        const data = this.editorPayload(config, row, context);

        if (row) {
            if (config.composite) {
                await this.deleteComposite(config, row, context);
                await DocumentManagementApi.post(`${this.apiRoot}/${config.name}`, data);
            } else {
                await DocumentManagementApi.put(`${this.apiRoot}/${config.name}(${row[config.key]})`, data);
            }

            DocumentManagementApi.notify(`${config.title} updated`);
            return;
        }

        await DocumentManagementApi.post(`${this.apiRoot}/${config.name}`, data);
        DocumentManagementApi.notify(`${config.title} created`);
    },

    editorPayload: function (config, row, context) {
        const form = document.getElementById("editor-fields");
        const payload = {};

        Object.entries(config.fields).forEach(([name, field]) => {
            if (field.payload === false) {
                return;
            }

            const input = form.querySelector(`[name='${name}']`);

            if (!input) {
                return;
            }

            payload[name] = this.coerceValue(input.value, field);
        });

        if (!row && config.key && config.fields[config.key]?.create !== false) {
            payload[config.key] = payload[config.key] || crypto.randomUUID();
        }

        if (config.context) {
            payload[config.context.field] = this.contextValue(config.context.type, context);
        }

        if (config.name === "File") {
            payload.CreatedBy = payload.CreatedBy || DocumentManagementApi.currentUserId();
        }

        if (config.name === "FileContent") {
            payload.CreatedBy = payload.CreatedBy || DocumentManagementApi.currentUserId();
            payload.RawData = btoa(form.querySelector("[name='RawDataText']")?.value || "");
        }

        return payload;
    },

    deleteRow: async function (config, row, context) {
        if (!confirm(`Delete ${config.title}?`)) {
            return;
        }

        if (config.composite) {
            await this.deleteComposite(config, row, context);
        } else {
            await DocumentManagementApi.delete(`${this.apiRoot}/${config.name}(${row[config.key]})`);
        }

        DocumentManagementApi.notify(`${config.title} deleted`);
        await this.loadFolders();
    },

    deleteComposite: function (config, row, context) {
        return DocumentManagementApi.post(
            `${this.apiRoot}/${config.name}/DeleteAll`,
            [this.compositePayload(config, row, context)]);
    },

    compositePayload: function (config, row, context) {
        const payload = {};

        Object.keys(config.fields).forEach(field => {
            payload[field] = row[field];
        });

        if (config.context) {
            payload[config.context.field] = this.contextValue(config.context.type, context);
        }

        return payload;
    },

    contextValue: function (type, context) {
        return context?.[type] ?? null;
    },

    rowKey: function (config, row) {
        if (!config.composite) {
            return row[config.key];
        }

        return Object.keys(config.fields)
            .map(field => row[field])
            .join("|");
    },

    label: function (config, column) {
        return config.fields[column]?.label ?? column;
    },

    defaultValue: function (name, field) {
        if (field.type === "number") {
            return 0;
        }

        if (name === "Id") {
            return crypto.randomUUID();
        }

        if (name === "Version") {
            return 1;
        }

        return "";
    },

    coerceValue: function (value, field) {
        if (field.nullable && !value) {
            return null;
        }

        if (field.type === "number") {
            return Number(value || 0);
        }

        return value;
    },

    displayValue: function (value) {
        if (value === null || value === undefined) {
            return "";
        }

        if (typeof value === "string" && value.length > 80) {
            return `${value.substring(0, 77)}...`;
        }

        return value;
    },

    escape: function (value) {
        return String(value ?? "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }
};

document.addEventListener("DOMContentLoaded", () => window.DocumentManagementGrids.init());
