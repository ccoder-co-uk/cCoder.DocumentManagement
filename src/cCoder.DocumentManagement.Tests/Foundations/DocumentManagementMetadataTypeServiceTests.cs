// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models.OData;
using cCoder.DocumentManagement.Services.Foundations;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DocumentManagementMetadataTypeServiceTests
{
    [Fact]
    public void ShouldExposeDocumentManagementMetadataContract()
    {
        // Given
        DocumentManagementMetadataTypeService service = new();

        // When
        MetadataContainerSet[] metadata = [.. service.GetKnownMetadata()];

        // Then
        metadata.Should()
            .ContainSingle();

        MetadataContainerSet container = metadata.Single();

        container.Name.Should()
            .Be(expected: "DocumentManagement");

        container.UriBase.Should()
            .Be(expected: "DocumentManagement");

        container.Types.Should()
            .HaveCount(expected: 4)
            .And.OnlyContain(predicate: type =>
                type.Category == "DocumentManagement"
                && type.IsEntity
                && type.HasEndpoint);
    }
}