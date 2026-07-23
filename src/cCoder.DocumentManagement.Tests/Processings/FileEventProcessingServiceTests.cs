// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Foundations.Events;
using cCoder.DocumentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileEventProcessingServiceTests
{
    private readonly Mock<IFileEventService> fileEventServiceMock;
    private readonly FileEventProcessingService service;

    public FileEventProcessingServiceTests()
    {
        fileEventServiceMock = new Mock<IFileEventService>(behavior: MockBehavior.Strict);
        service = new FileEventProcessingService(eventService: fileEventServiceMock.Object);
    }

    private static FileEntity CreateRandomFileEntity() =>
        Builder<FileEntity>.CreateNew().Build();
}