using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        FileContent fileContent = CreateRandomFileContent();

        FileContent submitted = null;

        fileContentBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFileContent>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FileContent_create"));

        fileContentBrokerMock
            .Setup(x =>
                x.AddFileContentAsync(
                    It.Is<DataFileContent>(candidate => candidate.Id != fileContent.Id)
                )
            )
            .Callback<DataFileContent>(candidate =>
                submitted = new FileContent
                {
                    Id = candidate.Id,
                    FileId = candidate.FileId,
                    Description = candidate.Description,
                    Size = candidate.Size,
                    CreatedBy = candidate.CreatedBy,
                    CreatedOn = candidate.CreatedOn,
                    Version = candidate.Version,
                    RawData = candidate.RawData,
                }
            )
            .ReturnsAsync((DataFileContent value) => value);

        // When
        FileContent result = await fileContentService.AddAsync(fileContent);

        // Then
        result.Should().NotBeSameAs(fileContent);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(
                fileContent,
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
                fileContent,
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

        fileContentBrokerMock.Verify(
            x =>
                x.AddFileContentAsync(
                    It.Is<DataFileContent>(candidate => candidate.Id != fileContent.Id)
                ),
            Times.Once
        );
        fileContentBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataFileContent>()),
            Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FileContent_create"), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();

        fileContentBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFileContent>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FileContent_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileContentService.AddAsync(fileContent);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        fileContentBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataFileContent>()),
            Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FileContent_create"), Times.Once);
    }

}








