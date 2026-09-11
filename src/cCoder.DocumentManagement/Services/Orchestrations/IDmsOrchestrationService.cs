// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal interface IDmsOrchestrationService
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