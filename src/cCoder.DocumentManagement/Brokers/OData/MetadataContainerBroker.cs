// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace cCoder.DocumentManagement.Brokers.OData;

internal sealed class MetadataContainerBroker : IMetadataContainerBroker
{
    public string GetAssemblyQualifiedName(Type type) =>
        type.AssemblyQualifiedName;

    public Type[] GetGenericArguments(Type type) =>
        type.GenericTypeArguments;

    public Type GetGenericTypeDefinition(Type type) =>
        type.GetGenericTypeDefinition();

    public string GetName(Type type) =>
        type.Name;

    public string GetName(PropertyInfo property) =>
        property.Name;

    public string GetString(Type type) =>
        type.ToString();

    public PropertyInfo[] GetProperties(Type type) =>
        type.GetProperties();

    public Type GetPropertyType(PropertyInfo property) =>
        property.PropertyType;

    public bool HasForeignKey(PropertyInfo property) =>
        property.GetCustomAttribute<ForeignKeyAttribute>() is not null;

    public bool HasRequired(PropertyInfo property) =>
        property.GetCustomAttribute<RequiredAttribute>() is not null;

    public bool HasTable(Type type) =>
        type.GetCustomAttribute<TableAttribute>() is not null;

    public bool IsAssignableToEnumerable(Type type) =>
        typeof(IEnumerable).IsAssignableFrom(c: type);

    public bool IsGeneric(Type type) =>
        type.IsGenericType;

    public bool IsValueType(Type type) =>
        type.IsValueType;

    public bool IsWritable(PropertyInfo property) =>
        property.CanWrite;
}