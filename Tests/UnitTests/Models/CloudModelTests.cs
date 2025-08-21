using Xunit;
using FluentAssertions;
using dotnet.Models;

namespace Tests.UnitTests.Models;

public class CloudModelTests
{
    [Fact]
    public void ServiceTagsModel_Properties_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var model = new ServiceTagsModel();

        // Assert
        model.changeNumber.Should().Be(0);
        model.cloud.Should().BeNull();
        model.values.Should().BeNull();
    }

    [Fact]
    public void ARMModel_Properties_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var model = new ARMModel();

        // Assert
        model.schema.Should().BeNull();
        model.contentVersion.Should().BeNull();
        model.parameters.Should().BeNull();
        model.variables.Should().BeNull();
        model.resources.Should().BeNull();
        model.outputs.Should().BeNull();
    }
}
