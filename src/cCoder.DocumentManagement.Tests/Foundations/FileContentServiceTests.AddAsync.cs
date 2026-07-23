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
using DataFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        FileContent fileContent = CreateRandomFileContent();

        FileContent submitted = null;

        fileContentBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_create"));

        fileContentBrokerMock
            .Setup(expression: x =>
                x.InsertFileContentAsync(
                    entity: It.Is<DataFileContent>(match: candidate => !ReferenceEquals(objA: candidate, objB: fileContent))
                )
            )
            .Callback<DataFileContent>(action: candidate =>
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
            .ReturnsAsync(valueFunction: (DataFileContent value) => value);

        // When
        FileContent result = await fileContentService.AddAsync(fileContent: fileContent);

        // Then
        result.Should()
            .BeSameAs(expected: fileContent);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: fileContent);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                expectation: fileContent,
                config: options =>
                    options
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedOn")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedBy")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdated")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedBy")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedOn")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "UpdatedBy")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "Created")
                        )
                        .Excluding(expression: candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
                expectation: fileContent,
                config: options =>
                    options
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedOn")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedBy")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdated")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedBy")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedOn")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "UpdatedBy")
                        )
                        .Excluding(
                            predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "Created")
                        )
                        .Excluding(expression: candidate => candidate.Id)
            );

        fileContentBrokerMock.Verify(
            expression: x =>
                x.InsertFileContentAsync(
                    entity: It.Is<DataFileContent>(match: candidate => !ReferenceEquals(objA: candidate, objB: fileContent))
                ),
            times: Times.Once
        );

        fileContentBrokerMock.Verify(
            expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()),
            times: Times.AtMostOnce()
        );

        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_create"), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();

        fileContentBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await fileContentService.AddAsync(fileContent: fileContent);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        fileContentBrokerMock.Verify(
            expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()),
            times: Times.AtMostOnce()
        );

        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_create"), times: Times.Once);
    }

}