// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(value: new User { Id = "test-user" });
        FileEntity file = CreateRandomFile();

        FileEntity submitted = null;

        fileBrokerMock.Setup(expression: x => x.GetAppId(It.IsAny<DataFile>())).Returns(value: (int?)7);
        authorizationBrokerMock.Setup(expression: x => x.Authorize((int?)7, "File_create"));

        fileBrokerMock
            .Setup(expression: x =>
                x.AddFileAsync(It.Is<DataFile>(candidate => !ReferenceEquals(candidate, file)))
            )
            .Callback<DataFile>(action: candidate =>
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
            .ReturnsAsync(valueFunction: (DataFile value) => value);

        // When
        FileEntity result = await fileService.AddAsync(file: file);

        // Then
        result.Should().BeSameAs(expected: file);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected: file);
        result.Should().NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                expectation: file,
                config: options =>
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
                expectation: file,
                config: options =>
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
            expression: x => x.AddFileAsync(It.Is<DataFile>(candidate => !ReferenceEquals(candidate, file))),
            times: Times.Once
        );
        fileBrokerMock.Verify(expression: x => x.GetAppId(It.IsAny<DataFile>()), times: Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize((int?)7, "File_create"), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FileEntity file = CreateRandomFile();

        fileBrokerMock.Setup(expression: x => x.GetAppId(It.IsAny<DataFile>())).Returns(value: (int?)7);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize((int?)7, "File_create"))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileService.AddAsync(file: file);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        fileBrokerMock.Verify(expression: x => x.GetAppId(It.IsAny<DataFile>()), times: Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize((int?)7, "File_create"), times: Times.Once);
    }

}