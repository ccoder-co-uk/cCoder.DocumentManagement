// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Extensions.OData;

namespace cCoder.DocumentManagement.Brokers.OData;

public interface IODataModelBroker
{
    ODataModel Build();
}