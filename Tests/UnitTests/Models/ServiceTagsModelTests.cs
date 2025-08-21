using Xunit;
using FluentAssertions;
using dotnet.Models;

namespace Tests.UnitTests.Models;

public class ServiceTagsModelTests
{
    [Fact]
    public void ServiceTagsModel_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var model = new ServiceTagsModel();

        // Assert
        model.changeNumber.Should().Be(0);
        model.cloud.Should().BeNull();
        model.values.Should().BeNull();
    }

    [Fact]
    public void ServiceTagsModel_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var model = new ServiceTagsModel();
        var changeNumber = 123;
        var cloud = "TestCloud";
        var values = new List<ValuesModel>();

        // Act
        model.changeNumber = changeNumber;
        model.cloud = cloud;
        model.values = values;

        // Assert
        model.changeNumber.Should().Be(changeNumber);
        model.cloud.Should().Be(cloud);
        model.values.Should().BeSameAs(values);
    }

    [Fact]
    public void ValuesModel_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var model = new ValuesModel();
        var name = "TestService";
        var id = "TestService.WestUS";
        var properties = new PropertiesModel();

        // Act
        model.name = name;
        model.id = id;
        model.properties = properties;

        // Assert
        model.name.Should().Be(name);
        model.id.Should().Be(id);
        model.properties.Should().BeSameAs(properties);
    }

    [Fact]
    public void PropertiesModel_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var model = new PropertiesModel();
        var changeNumber = 456;
        var region = "WestUS";
        var platform = "Azure";
        var systemService = "Storage";
        var addressPrefixes = new List<string> { "10.0.0.0/8", "192.168.1.0/24" };

        // Act
        model.changeNumber = changeNumber;
        model.region = region;
        model.platform = platform;
        model.systemService = systemService;
        model.addressPrefixes = addressPrefixes;

        // Assert
        model.changeNumber.Should().Be(changeNumber);
        model.region.Should().Be(region);
        model.platform.Should().Be(platform);
        model.systemService.Should().Be(systemService);
        model.addressPrefixes.Should().BeSameAs(addressPrefixes);
    }
}
