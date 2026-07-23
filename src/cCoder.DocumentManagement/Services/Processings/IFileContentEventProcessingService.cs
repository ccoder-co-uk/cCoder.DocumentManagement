// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Processings;

public interface IFileContentEventProcessingService
{
    ValueTask RaiseFileContentAddEventAsync(FileContent entity);
    ValueTask RaiseFileContentUpdateEventAsync(FileContent entity);
    ValueTask RaiseFileContentDeleteEventAsync(FileContent entity);
}