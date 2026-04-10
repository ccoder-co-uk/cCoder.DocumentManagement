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
        var orchestrationServiceMock = new Mock<IDmsOrchestrationService>(MockBehavior.Strict);
        orchestrationServiceMock.Setup(service => service.Search("needle")).Returns(expectedFiles);

        var factory = new DmsInstanceFactory(orchestrationServiceMock.Object);

        IDms dms = factory.CreateDms();
        IEnumerable<DataFile> actualFiles = dms.Search("needle");

        actualFiles.Should().BeSameAs(expectedFiles);
        orchestrationServiceMock.Verify(service => service.Search("needle"), Times.Once);
        orchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCreateDmsThatDelegatesSaveAsync()
    {
        var orchestrationServiceMock = new Mock<IDmsOrchestrationService>(MockBehavior.Strict);
        var path = new DmsPath("content/file.txt");
        using var content = new MemoryStream([1, 2, 3]);

        orchestrationServiceMock
            .Setup(service => service.SaveAsync(path, content))
            .Returns(ValueTask.CompletedTask);

        var factory = new DmsInstanceFactory(orchestrationServiceMock.Object);

        IDms dms = factory.CreateDms();
        await dms.SaveAsync(path, content);

        orchestrationServiceMock.Verify(service => service.SaveAsync(path, content), Times.Once);
        orchestrationServiceMock.VerifyNoOtherCalls();
    }
}
