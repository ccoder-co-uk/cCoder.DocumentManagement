// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;

namespace cCoder.DocumentManagement.Brokers.OData;

internal interface IMetadataContainerBroker
{
    string GetAssemblyQualifiedName(Type type);
    Type[] GetGenericArguments(Type type);
    Type GetGenericTypeDefinition(Type type);
    string GetName(Type type);
    string GetName(PropertyInfo property);
    string GetString(Type type);
    PropertyInfo[] GetProperties(Type type);
    Type GetPropertyType(PropertyInfo property);
    bool HasForeignKey(PropertyInfo property);
    bool HasRequired(PropertyInfo property);
    bool HasTable(Type type);
    bool IsAssignableToEnumerable(Type type);
    bool IsGeneric(Type type);
    bool IsValueType(Type type);
    bool IsWritable(PropertyInfo property);
}