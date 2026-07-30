// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal interface IDmsOrchestrationService
{
    DmsOperation GetFilesZippedDmsOperation(DmsOperation operation);

    DmsOperation GetDmsOperation(DmsOperation operation);

    DmsOperation SearchFilesDmsOperation(DmsOperation operation);

    ValueTask<DmsOperation> UnpackDmsOperationAsync(DmsOperation operation);

    ValueTask<DmsOperation> SaveDmsOperationAsync(DmsOperation operation);

    ValueTask<DmsOperation> DropDmsOperationAsync(DmsOperation operation);

    ValueTask<DmsOperation> CopyDmsOperationAsync(DmsOperation operation);

    ValueTask<DmsOperation> MoveDmsOperationAsync(DmsOperation operation);
}