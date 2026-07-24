// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Services.Foundations;
using Moq;
using DMSResult = cCoder.DocumentManagement.Dependencies.DMSResult;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    private readonly Mock<IDmsInstanceBroker> dmsInstanceBrokerMock;
    private readonly DmsInstanceService dmsInstanceService;

    public DmsInstanceServiceTests()
    {
        dmsInstanceBrokerMock = new Mock<IDmsInstanceBroker>(behavior: MockBehavior.Strict);
        dmsInstanceBrokerMock = new();
        dmsInstanceService = new DmsInstanceService(dmsInstanceBroker: dmsInstanceBrokerMock.Object);
    }

    private static DmsPath CreatePath(string fullPath) =>
        new(path: fullPath);

    private static DMSResult CreateDmsResult(string contentType = "application/json") =>
        new() { MimeType = contentType, Data = new MemoryStream(buffer: [1, 2, 3]) };

    private static cCoder.Data.Models.DMS.File CreateLocalFileAsync() =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = $"file-{Guid.NewGuid():N}.txt",
            Path = $"folder/file-{Guid.NewGuid():N}.txt",
        };
}