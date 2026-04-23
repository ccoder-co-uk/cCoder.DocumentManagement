using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        FileEntity file = CreateRandomFile();

        FileEntity submitted = null;

        fileBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFile>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "File_create"));

        fileBrokerMock
            .Setup(x =>
                x.AddFileAsync(It.Is<DataFile>(candidate => !ReferenceEquals(candidate, file)))
            )
            .Callback<DataFile>(candidate =>
                submitted = new FileEntity
                {
                    Id = candidate.Id,
                    FolderId = candidate.FolderId,
                    Name = candidate.Name,
                    Description = candidate.Description,
                    Path = candidate.Path,
                    MimeType = candidate.MimeType,
                    CreatedBy = candidate.CreatedBy,
                    CreatedOn = candidate.CreatedOn,
                    Size = candidate.Size,
                }
            )
            .ReturnsAsync((DataFile value) => value);

        // When
        FileEntity result = await fileService.AddAsync(file);

        // Then
        result.Should().BeSameAs(file);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(file);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                file,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
                        .Excluding(candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
                file,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
                        .Excluding(candidate => candidate.Id)
            );

        fileBrokerMock.Verify(
            x => x.AddFileAsync(It.Is<DataFile>(candidate => !ReferenceEquals(candidate, file))),
            Times.Once
        );
        fileBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFile>()), Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "File_create"), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FileEntity file = CreateRandomFile();

        fileBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFile>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "File_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileService.AddAsync(file);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        fileBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFile>()), Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "File_create"), Times.Once);
    }

}









