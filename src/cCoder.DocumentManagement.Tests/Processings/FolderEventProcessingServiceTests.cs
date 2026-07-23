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

public partial class FolderEventProcessingServiceTests
{
    private readonly Mock<IFolderEventService> folderEventServiceMock;
    private readonly FolderEventProcessingService service;

    public FolderEventProcessingServiceTests()
    {
        folderEventServiceMock = new Mock<IFolderEventService>(behavior: MockBehavior.Strict);
        service = new FolderEventProcessingService(eventService: folderEventServiceMock.Object);
    }

    private static Folder CreateRandomFolder() =>
        Builder<Folder>.CreateNew().Build();
}