// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.DocumentManagement.Models.OData;

namespace cCoder.DocumentManagement.Dependencies;

internal static class MetadataContainerDependency
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

    internal static MetadataContainer CreateMetadataContainer(Type type, bool isEntity = false, bool hasEndpoint = false) =>
        PopulateMetadataContainer(container: new MetadataContainer(), type: type, isEntity: isEntity, hasEndpoint: hasEndpoint);

    internal static ExtendedMetadataContainer CreateExtendedMetadataContainer(Type type, bool isEntity = false, bool hasEndpoint = false) =>
        PopulateMetadataContainer(container: new ExtendedMetadataContainer(), type: type, isEntity: isEntity, hasEndpoint: hasEndpoint);

    private static T PopulateMetadataContainer<T>(T container, Type type, bool isEntity, bool hasEndpoint)
        where T : MetadataContainer
    {
        container.IsValueType = type.IsValueType || type == typeof(string);
        container.Type = GetTypeName(type: type);
        container.Name = type.Name;
        container.DisplayName = type.Name;
        container.Description = type.Name;
        container.ServerType = type.AssemblyQualifiedName;
        container.ServerTypeName = type.GetCSharpTypeName();
        container.Properties = container.IsValueType
            ? []
            : type.GetProperties()
                .Select(selector: CreatePropertyContainer)
                .ToArray();
        container.IsEntity = isEntity;
        container.IsJoinEntity = isEntity && type.IsJoinType();
        container.HasEndpoint = hasEndpoint;
        return container;
    }

    private static PropertyContainer CreatePropertyContainer(PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetTypeName(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = property.PropertyType.GetCSharpTypeName(),
            IsValueType = property.PropertyType.IsValueType || property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = property.GetCustomAttribute<KeyAttribute>() is not null || property.Name == "Id" ? "key" : property.Name,
            IsRequired = (!(property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                && property.PropertyType.IsValueType)
                || property.GetCustomAttribute<RequiredAttribute>() is not null
        };

    private static string GetTypeName(Type type) =>
        type == typeof(string)
            ? "string"
            : typeof(IEnumerable).IsAssignableFrom(c: type)
                ? "array"
                : TypeNames.TryGetValue(key: type, value: out string typeName) ? typeName : "object";
}