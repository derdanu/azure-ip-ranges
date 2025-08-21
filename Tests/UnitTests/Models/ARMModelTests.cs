using Xunit;
using FluentAssertions;
using dotnet.Models;

namespace Tests.UnitTests.Models;

public class ARMModelTests
{
    [Fact]
    public void ARMModel_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var model = new ARMModel();

        // Assert
        model.schema.Should().BeNull();
        model.contentVersion.Should().BeNull();
        model.parameters.Should().BeNull();
        model.variables.Should().BeNull();
        model.resources.Should().BeNull();
        model.outputs.Should().BeNull();
        model.filename.Should().BeNull();
        model.apiProfile.Should().BeNull();
        model.functions.Should().BeNull();
    }

    [Fact]
    public void ARMModel_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var model = new ARMModel();
        var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";
        var contentVersion = "1.0.0.0";
        var filename = "test.json";
        var parameters = new Parameters();
        var variables = new List<Variables>();
        var resources = new List<Resources>();
        var outputs = new List<Outputs>();
        var functions = new List<Functions>();

        // Act
        model.schema = schema;
        model.contentVersion = contentVersion;
        model.filename = filename;
        model.parameters = parameters;
        model.variables = variables;
        model.resources = resources;
        model.outputs = outputs;
        model.functions = functions;

        // Assert
        model.schema.Should().Be(schema);
        model.contentVersion.Should().Be(contentVersion);
        model.filename.Should().Be(filename);
        model.parameters.Should().BeSameAs(parameters);
        model.variables.Should().BeSameAs(variables);
        model.resources.Should().BeSameAs(resources);
        model.outputs.Should().BeSameAs(outputs);
        model.functions.Should().BeSameAs(functions);
    }

    [Fact]
    public void Parameters_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var parameters = new Parameters();
        var parameterOptions = new ParameterOptions();

        // Act
        parameters.name = parameterOptions;

        // Assert
        parameters.name.Should().BeSameAs(parameterOptions);
    }

    [Fact]
    public void ParameterOptions_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var parameterOptions = new ParameterOptions();
        var type = "string";
        var defaultValue = "TestValue";

        // Act
        parameterOptions.type = type;
        parameterOptions.defaultValue = defaultValue;

        // Assert
        parameterOptions.type.Should().Be(type);
        parameterOptions.defaultValue.Should().Be(defaultValue);
    }

    [Fact]
    public void Resources_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var resource = new Resources();
        var type = "Microsoft.Network/routeTables";
        var name = "TestRouteTable";
        var apiVersion = "2020-11-01";
        var location = "West US";
        var properties = new Properties();

        // Act
        resource.type = type;
        resource.name = name;
        resource.apiVersion = apiVersion;
        resource.location = location;
        resource.properties = properties;

        // Assert
        resource.type.Should().Be(type);
        resource.name.Should().Be(name);
        resource.apiVersion.Should().Be(apiVersion);
        resource.location.Should().Be(location);
        resource.properties.Should().BeSameAs(properties);
    }

    [Fact]
    public void Properties_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var properties = new Properties();
        var routes = new List<Route>();

        // Act
        properties.routes = routes;

        // Assert
        properties.routes.Should().BeSameAs(routes);
    }

    [Fact]
    public void Route_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var route = new Route();
        var name = "TestRoute";
        var routeProperties = new RouteProperties();

        // Act
        route.name = name;
        route.properties = routeProperties;

        // Assert
        route.name.Should().Be(name);
        route.properties.Should().BeSameAs(routeProperties);
    }

    [Fact]
    public void RouteProperties_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var routeProperties = new RouteProperties();
        var addressPrefix = "10.0.0.0/8";
        var nextHopType = "VirtualNetworkGateway";
        var nextHopIpAddress = "10.0.0.1";

        // Act
        routeProperties.addressPrefix = addressPrefix;
        routeProperties.nextHopType = nextHopType;
        routeProperties.nextHopIpAddress = nextHopIpAddress;

        // Assert
        routeProperties.addressPrefix.Should().Be(addressPrefix);
        routeProperties.nextHopType.Should().Be(nextHopType);
        routeProperties.nextHopIpAddress.Should().Be(nextHopIpAddress);
    }
}
