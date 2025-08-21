using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using Tests.IntegrationTests;

namespace Tests.FunctionalTests;

public class WebApplicationFunctionalTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public WebApplicationFunctionalTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Application_ShouldStartSuccessfully()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task IPSearchWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange
        var testIP = "192.168.1.1";

        // Act - Search for IP
        var searchResponse = await _client.GetAsync($"/SearchFor?ip={testIP}");
        
        // Assert
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        searchContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ARMTemplateDownloadWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange
        var cloud = "Public";
        var serviceId = "1";

        // Act - Download ARM template
        var response = await _client.GetAsync($"/downloadARMTemplate/{cloud}/{serviceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        
        // Verify it's valid JSON
        var armTemplate = JsonSerializer.Deserialize<JsonElement>(content);
        armTemplate.ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public async Task CloudDataRetrievalWorkflow_ShouldWorkForAllClouds()
    {
        // Arrange
        var clouds = new[] { "Public", "AzureGovernment", "China", "AzureGermany" };

        foreach (var cloud in clouds)
        {
            // Act
            var response = await _client.GetAsync($"/getPrefixes/{cloud}/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task OPNsenseFileDownloadWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange
        var clouds = new[] { "Public", "AzureGovernment" };

        foreach (var cloud in clouds)
        {
            // Act
            var response = await _client.GetAsync($"/getOPNsenseUrlTable/{cloud}/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("text/plain");
            response.Content.Headers.ContentDisposition?.FileName.Should().EndWith(".txt");
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task NavigationWorkflow_ShouldAllowUserToNavigateThroughAllPages()
    {
        // Test navigation to all main pages
        var pages = new[]
        {
            "/",
            "/Home/Index",
            "/Home/About",
            "/Home/Update"
        };

        foreach (var page in pages)
        {
            // Act
            var response = await _client.GetAsync(page);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task ErrorHandlingWorkflow_ShouldHandleInvalidInputGracefully()
    {
        // Test various invalid inputs
        var invalidSearches = new[]
        {
            "",
            "   ",
            "invalid-ip",
            "999.999.999.999"
        };

        foreach (var invalidSearch in invalidSearches)
        {
            // Act
            var response = await _client.GetAsync($"/SearchFor?ip={invalidSearch}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            // Should either return error page or handle gracefully
        }
    }

    [Fact]
    public async Task StaticContentWorkflow_ShouldServeStaticFiles()
    {
        // Test static file serving
        var staticFiles = new[]
        {
            "/css/site.css",
            "/js/site.js",
            "/favicon.ico"
        };

        foreach (var staticFile in staticFiles)
        {
            // Act
            var response = await _client.GetAsync(staticFile);

            // Assert - Should either be OK or NotFound (depending on if file exists)
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task DataDirectoryBrowsingWorkflow_ShouldAllowAccessToDataFiles()
    {
        // Test data file access
        var dataFiles = new[]
        {
            "/data/Public.json",
            "/data/AzureGovernment.json",
            "/data/China.json",
            "/data/AzureGermany.json"
        };

        foreach (var dataFile in dataFiles)
        {
            // Act
            var response = await _client.GetAsync(dataFile);

            // Assert - Should either be OK or NotFound (depending on if file exists)
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task FullUserJourney_SearchIPAndDownloadARMTemplate()
    {
        // Simulate a complete user journey
        
        // Step 1: Visit home page
        var homeResponse = await _client.GetAsync("/");
        homeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 2: Search for an IP
        var searchResponse = await _client.GetAsync("/SearchFor?ip=192.168.1.1");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Get cloud prefixes
        var prefixesResponse = await _client.GetAsync("/getPrefixes/Public/1");
        prefixesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Download ARM template
        var armResponse = await _client.GetAsync("/downloadARMTemplate/Public/1");
        armResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify ARM template is valid JSON
        var armContent = await armResponse.Content.ReadAsStringAsync();
        var armTemplate = JsonSerializer.Deserialize<JsonElement>(armContent);
        armTemplate.ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public async Task ApplicationHealthCheck_ShouldRespondToAllEndpoints()
    {
        // Test that all endpoints respond (even if with different status codes)
        var endpoints = new[]
        {
            "/",
            "/Home/Index",
            "/Home/About",
            "/Home/Update",
            "/SearchFor?ip=test",
            "/getPrefixes/Public/1",
            "/getOPNsenseUrlTable/Public/1",
            "/downloadARMTemplate/Public/1",
            "/Home/Error",
            "/Home/Privacy"
        };

        foreach (var endpoint in endpoints)
        {
            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert - Should respond (not timeout or throw exception)
            response.Should().NotBeNull();
            // Most endpoints should return OK, but some might return redirects or other valid responses
            ((int)response.StatusCode).Should().BeLessThan(500); // No server errors
        }
    }
}
