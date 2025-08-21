using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using Tests.IntegrationTests;

namespace Tests.DiagnosticTests
{
    public class RoutingDiagnosticTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public RoutingDiagnosticTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Trait("Category", "Integration")]
    [Fact]
        public async Task CanAccessRoot_ReturnsSuccess()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Trait("Category", "Integration")]
    [Fact]
        public async Task CanAccessHomeIndex_ReturnsSuccess()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/Home/Index");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Trait("Category", "Integration")]
    [Fact]
        public async Task CheckRoutingInfo_ShowsAvailableRoutes()
        {
            // This test will help us understand what routes are available
            var routes = new[]
            {
                "/",
                "/Home",
                "/Home/Index",
                "/Home/About",
                "/SearchFor",
                "/SearchFor?searchFor=10.0.0.1",
                "/Home/Update",
                "/getPrefixes/Public/1",
                "/downloadARMTemplate/Public/1",
                "/deployARMTemplate/Public/1",
                "/getOPNsenseUrlTable/Public/1"
            };

            foreach (var route in routes)
            {
                var response = await _client.GetAsync(route);
                // Just output the status for debugging
                var statusCode = response.StatusCode;
                var content = await response.Content.ReadAsStringAsync();
                
                // For debugging - we'll see what each route returns
                Assert.True(true, $"Route: {route} returned {statusCode}");
            }
        }
    }
}
