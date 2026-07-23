// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Brokers;

public class DmsInstanceFactoryTests
{
    [Fact]
    public void ShouldCreateDmsThatDelegatesSearch()
    {
        IEnumerable<DataFile> expectedFiles = [new() { Id = Guid.NewGuid(), Name = "file.txt" }];
        var orchestrationServiceMock = new Mock<IDmsOrchestrationService>(behavior: MockBehavior.Strict);

        orchestrationServiceMock.Setup(expression: service => service.Search(needle: "needle"))
            .Returns(value: expectedFiles);

        var factory = new DmsInstanceFactory(dmsOrchestrationService: orchestrationServiceMock.Object);

        IDms dms = factory.CreateDms();
        IEnumerable<DataFile> actualFiles = dms.Search(needle: "needle");

        actualFiles.Should()
            .BeSameAs(expected: expectedFiles);

        orchestrationServiceMock.Verify(expression: service => service.Search(needle: "needle"), times: Times.Once);
        orchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCreateDmsThatDelegatesSaveAsync()
    {
        var orchestrationServiceMock = new Mock<IDmsOrchestrationService>(behavior: MockBehavior.Strict);
        var path = new DmsPath(path: "content/file.txt");
        using var content = new MemoryStream(buffer: [1, 2, 3]);

        orchestrationServiceMock
            .Setup(expression: service => service.SaveAsync(path: path, content: content))
            .Returns(value: ValueTask.CompletedTask);

        var factory = new DmsInstanceFactory(dmsOrchestrationService: orchestrationServiceMock.Object);

        IDms dms = factory.CreateDms();
        await dms.SaveAsync(path: path, content: content);

        orchestrationServiceMock.Verify(expression: service => service.SaveAsync(path: path, content: content), times: Times.Once);
        orchestrationServiceMock.VerifyNoOtherCalls();
    }
}