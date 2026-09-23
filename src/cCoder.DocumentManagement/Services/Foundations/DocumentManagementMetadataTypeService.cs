// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Brokers.OData;
using cCoder.DocumentManagement.Models.OData;
using DmsFile = cCoder.Data.Models.DMS.File;
using FolderRole = cCoder.Data.Models.Security.FolderRole;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class DocumentManagementMetadataTypeService(
    IMetadataContainerBroker metadataContainerBroker)
    : IDocumentManagementMetadataTypeService
{
    private static readonly Dictionary<Type, string> TypeNames = new()
    {
        { typeof(short), "number" }, { typeof(int), "number" }, { typeof(long), "number" },
        { typeof(short?), "number" }, { typeof(int?), "number" }, { typeof(long?), "number" },
        { typeof(ushort), "number" }, { typeof(uint), "number" }, { typeof(ulong), "number" },
        { typeof(ushort?), "number" }, { typeof(uint?), "number" }, { typeof(ulong?), "number" },
        { typeof(byte), "number" }, { typeof(byte?), "number" },
        { typeof(decimal), "number" }, { typeof(decimal?), "number" },
        { typeof(string), "string" }, { typeof(DateTime), "date" }, { typeof(DateTime?), "date" },
        { typeof(TimeSpan), "time" }, { typeof(TimeSpan?), "time" },
        { typeof(DateTimeOffset), "date" }, { typeof(DateTimeOffset?), "date" },
        { typeof(Guid), "guid" }, { typeof(Guid?), "guid" },
        { typeof(bool), "bool" }, { typeof(bool?), "bool" },
        { typeof(double), "number" }, { typeof(double?), "number" },
        { typeof(float), "number" }, { typeof(float?), "number" }
    };

    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch(operation: IEnumerable<MetadataContainerSet> () =>
        [
            new MetadataContainerSet
            {
                Name = "DocumentManagement",
                UriBase = "DocumentManagement",
                Types =
                [
                    Entity<DmsFile>(),
                    Entity<FileContent>(),
                    Entity<Folder>(),
                    Entity<FolderRole>(),
                ],
            },
        ]);

    private ExtendedMetadataContainer Entity<T>()
    {
        ExtendedMetadataContainer container = CreateExtendedMetadataContainer(
            type: typeof(T),
            isEntity: true,
            hasEndpoint: true);

        container.Category = "DocumentManagement";
        return container;
    }

    private ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
        bool isEntity,
        bool hasEndpoint)
    {
        bool isValueType = IsValueType(type: type);

        return new ExtendedMetadataContainer
        {
            IsValueType = isValueType,
            Type = GetTypeName(type: type),
            Name = metadataContainerBroker.GetName(type: type),
            DisplayName = metadataContainerBroker.GetName(type: type),
            Description = metadataContainerBroker.GetName(type: type),
            ServerType = metadataContainerBroker.GetAssemblyQualifiedName(type: type),
            ServerTypeName = GetCSharpTypeName(type: type),
            Properties = isValueType
                ? []
                : metadataContainerBroker.GetProperties(type: type)
                    .Select(selector: CreatePropertyContainer)
                    .ToArray(),
            IsEntity = isEntity,
            IsJoinEntity = isEntity && IsJoinType(type: type),
            HasEndpoint = hasEndpoint,
        };
    }

    private PropertyContainer CreatePropertyContainer(PropertyInfo property)
    {
        Type propertyType = metadataContainerBroker.GetPropertyType(property: property);
        string propertyName = metadataContainerBroker.GetName(property: property);

        return new PropertyContainer
        {
            Name = propertyName,
            Type = GetTypeName(type: propertyType),
            ServerType = metadataContainerBroker.GetString(type: propertyType),
            ServerTypeName = GetCSharpTypeName(type: propertyType),
            IsValueType = IsValueType(type: propertyType),
            DisplayName = propertyName,
            ShortDisplayName = propertyName,
            Description = propertyName,
            IsReadOnly = !metadataContainerBroker.IsWritable(property: property),
            Template = metadataContainerBroker.HasRequired(property: property) || propertyName == "Id"
                ? "key"
                : propertyName,
            IsRequired = IsRequired(property: property, propertyType: propertyType),
        };
    }

    private string GetCSharpTypeName(Type type)
    {
        if (!metadataContainerBroker.IsGeneric(type: type))
        {
            return metadataContainerBroker.GetName(type: type);
        }

        IEnumerable<string> genericNames = metadataContainerBroker
            .GetGenericArguments(type: type)
            .Select(selector: GetCSharpTypeName);

        return $"{metadataContainerBroker.GetName(type: type)
            .Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>"
            .Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    private bool IsJoinType(Type type)
    {
        PropertyInfo[] properties = metadataContainerBroker.GetProperties(type: type);

        return metadataContainerBroker.HasTable(type: type)
            && properties.Length == 4
            && properties
                .Where(predicate: property =>
                    IsValueType(type: metadataContainerBroker.GetPropertyType(property: property)))
                .All(predicate: metadataContainerBroker.HasForeignKey);
    }

    private bool IsRequired(PropertyInfo property, Type propertyType) =>
        (!(metadataContainerBroker.IsGeneric(type: propertyType)
            && metadataContainerBroker.GetGenericTypeDefinition(type: propertyType) == typeof(Nullable<>))
        && metadataContainerBroker.IsValueType(type: propertyType))
        || metadataContainerBroker.HasRequired(property: property);

    private bool IsValueType(Type type) =>
        metadataContainerBroker.IsValueType(type: type)
        || type == typeof(string);

    private string GetTypeName(Type type) =>
        type == typeof(string)
            ? "string"
            : metadataContainerBroker.IsAssignableToEnumerable(type: type)
                ? "array"
                : TypeNames.TryGetValue(key: type, value: out string typeName)
                    ? typeName
                    : "object";
}