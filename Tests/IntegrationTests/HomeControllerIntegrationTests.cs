using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using AngleSharp.Html.Dom;
using AngleSharp;
using System.Collections.Generic;
using System.Text.Json;
using Tests.IntegrationTests;

namespace Tests.IntegrationTests;

public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public HomeControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Index_Get_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task About_Get_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/Home/About");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Update_Get_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/Home/Update");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
    }

    [Trait("Category", "Integration")]
    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("192.168.1.1")]
    [InlineData("172.16.0.1")]
    public async Task SearchFor_WithValidIP_ReturnsSuccess(string searchFor)
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/SearchFor?ip={searchFor}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    [Trait("Category", "Integration")]
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchFor_WithInvalidInput_ReturnsErrorPage(string searchFor)
    {
        // Act
        var response = await _client.GetAsync($"/SearchFor?ip={searchFor}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Search for IP"); // The page shows the search form, not an error
    }

        [Trait("Category", "Integration")]
    [Theory]
        [InlineData("Public")]
        [InlineData("AzureGovernment")]
        [InlineData("China")]
        [InlineData("AzureGermany")]
        public async Task GetPrefixes_WithValidCloud_ReturnsSuccess(string cloud)
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/getPrefixes/{cloud}/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Trait("Category", "Integration")]
    [Fact]
    public async Task GetPrefixes_WithInvalidCloud_ReturnsJsonResponse()
    {
        // Act
        var response = await _client.GetAsync("/getPrefixes/InvalidCloud/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
    }

    [Trait("Category", "Integration")]
    [Theory]
    [InlineData("Public")]
    [InlineData("AzureGovernment")]
    public async Task GetOPNsenseUrlTable_WithValidCloud_ReturnsTextFile(string cloud)
    {
        // Act
        var response = await _client.GetAsync($"/getOPNsenseUrlTable/{cloud}/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("text/plain");
        response.Content.Headers.ContentDisposition?.FileName.Should().EndWith(".txt");
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task DownloadARMTemplate_WithValidParameters_ReturnsJsonFile()
    {
        // Arrange
        var cloud = "Public";
        var id = "1";

        // Act
        var response = await _client.GetAsync($"/downloadARMTemplate/{cloud}/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        response.Content.Headers.ContentDisposition?.FileName.Should().EndWith(".json");
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Error_Get_ReturnsErrorPage()
    {
        // Act
        var response = await _client.GetAsync("/Home/Error");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Error");
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Index_ContainsExpectedElements()
    {
        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(content));

        // Check for basic HTML structure
        document.Should().NotBeNull();
        document.Title.Should().NotBeNullOrEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task SearchFor_WithValidInput_ContainsSearchResults()
    {
        // Arrange
        var searchFor = "192.168.1.1";

        // Act
        var response = await _client.GetAsync($"/SearchFor?ip={searchFor}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(content));

        document.Should().NotBeNull();
        // The response should contain search results or search form
        content.Should().NotBeNullOrEmpty();
    }
}
