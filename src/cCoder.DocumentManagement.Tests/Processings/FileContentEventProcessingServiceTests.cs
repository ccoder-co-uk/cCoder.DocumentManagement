// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;
using cCoder.DocumentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentEventProcessingServiceTests
{
    private readonly Mock<IFileContentEventService> fileContentEventServiceMock;
    private readonly FileContentEventProcessingService service;

    public FileContentEventProcessingServiceTests()
    {
        fileContentEventServiceMock = new Mock<IFileContentEventService>(behavior: MockBehavior.Strict);
        service = new FileContentEventProcessingService(eventService: fileContentEventServiceMock.Object);
    }

    private static FileContent CreateRandomFileContent() =>
        Builder<FileContent>.CreateNew()
                                        .Build();
}