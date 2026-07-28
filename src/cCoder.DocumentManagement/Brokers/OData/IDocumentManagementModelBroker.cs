// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Brokers.OData;

public interface IDocumentManagementModelBroker : IODataModelBroker
{
    void Configure();
}