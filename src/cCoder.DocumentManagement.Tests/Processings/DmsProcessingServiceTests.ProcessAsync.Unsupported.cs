// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenMethodIsUnsupported()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "PATCH", requestPath: "/api/dms/folder/file.txt");

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request: request);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "Unsupported DMS method: PATCH");

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}