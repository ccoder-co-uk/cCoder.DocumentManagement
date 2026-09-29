// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;


namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFileContentEventProcessingService
{
    ValueTask RaiseFileContentAddEventAsync(FileContent fileContent);
    ValueTask RaiseFileContentUpdateEventAsync(FileContent fileContent);
    ValueTask RaiseFileContentDeleteEventAsync(FileContent fileContent);
}