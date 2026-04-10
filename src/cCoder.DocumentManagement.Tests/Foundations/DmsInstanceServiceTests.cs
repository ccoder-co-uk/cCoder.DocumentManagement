using cCoder.Data;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Services.Foundations;
using Moq;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    private readonly Mock<IDmsInstanceBroker> dmsInstanceBrokerMock;
    private readonly DmsInstanceService dmsInstanceService;

    public DmsInstanceServiceTests()
    {
        dmsInstanceBrokerMock = new Mock<IDmsInstanceBroker>(MockBehavior.Strict);
        dmsInstanceBrokerMock = new();
        dmsInstanceService = new DmsInstanceService(dmsInstanceBrokerMock.Object);
    }

    private static DmsPath CreatePath(string fullPath) => new(fullPath);

    private static DMSResult CreateDmsResult(string contentType = "application/json") =>
        new() { MimeType = contentType, Data = new MemoryStream([1, 2, 3]) };

    private static cCoder.Data.Models.DMS.File CreateFileAsync() =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = $"file-{Guid.NewGuid():N}.txt",
            Path = $"folder/file-{Guid.NewGuid():N}.txt",
        };
}










