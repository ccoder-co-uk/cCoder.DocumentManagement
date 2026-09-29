// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal interface IFileContentEventService
{
    ValueTask RaiseFileContentAddEventAsync(FileContent fileContent);
    ValueTask RaiseFileContentUpdateEventAsync(FileContent fileContent);
    ValueTask RaiseFileContentDeleteEventAsync(FileContent fileContent);
}