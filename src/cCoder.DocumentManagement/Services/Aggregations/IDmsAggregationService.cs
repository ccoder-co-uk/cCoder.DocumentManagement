// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Services.Aggregations;

internal interface IDmsAggregationService
{
    DmsOperation GetFilesZippedDmsOperation(DmsOperation dmsOperation);

    DmsOperation GetDmsOperation(DmsOperation dmsOperation);

    DmsOperation SearchFilesDmsOperation(DmsOperation dmsOperation);

    ValueTask<DmsOperation> UnpackDmsOperationAsync(DmsOperation dmsOperation);

    ValueTask<DmsOperation> SaveDmsOperationAsync(DmsOperation dmsOperation);

    ValueTask<DmsOperation> DropDmsOperationAsync(DmsOperation dmsOperation);

    ValueTask<DmsOperation> CopyDmsOperationAsync(DmsOperation dmsOperation);

    ValueTask<DmsOperation> MoveDmsOperationAsync(DmsOperation dmsOperation);
}