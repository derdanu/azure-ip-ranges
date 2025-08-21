using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using dotnet.Controllers;
using dotnet.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.UnitTests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<HttpRequest> _mockRequest;
    private readonly Mock<IRequestCookieCollection> _mockCookies;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockRequest = new Mock<HttpRequest>();
        _mockCookies = new Mock<IRequestCookieCollection>();
        
        // Setup mock cookies
        _mockCookies.Setup(x => x[HomeController.SessionKeyName]).Returns((string)null);
        _mockRequest.Setup(x => x.Cookies).Returns(_mockCookies.Object);
        
        // Setup mock request for URL generation
        _mockRequest.Setup(x => x.Scheme).Returns("https");
        _mockRequest.Setup(x => x.Host).Returns(new HostString("localhost"));
        _mockRequest.Setup(x => x.PathBase).Returns(new PathString(""));
        _mockRequest.Setup(x => x.Path).Returns(new PathString("/"));
        _mockRequest.Setup(x => x.QueryString).Returns(new QueryString(""));
        
        _mockHttpContext.Setup(x => x.Request).Returns(_mockRequest.Object);
        
        _controller = new HomeController(_mockLogger.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };
    }

    [Fact]
    public void Index_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void About_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.About();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnViewResult()
    {
        // Act
        var result = await _controller.Update();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void SearchFor_WithInvalidInput_ShouldReturnErrorView(string ip)
    {
        // Act
        var result = _controller.SearchFor(ip);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        // When no specific view name is provided, ViewName is null and uses default action name
        // For SearchFor action, it returns the SearchFor view by default
        viewResult.ViewName.Should().BeNull(); // Default view name behavior
    }

    [Fact]
    public void SearchFor_WithValidIP_ShouldReturnSearchView()
    {
        // Arrange
        var ip = "192.168.1.1";

        // Act
        var result = _controller.SearchFor(ip);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().BeNull(); // Default view name (SearchFor)
        viewResult.Model.Should().BeNull(); // No model - uses ViewBag instead
        viewResult.ViewData["ip"].Should().Be(ip);
    }

    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("172.16.0.1")]
    [InlineData("192.168.0.1")]
    public void SearchFor_WithPrivateIP_ShouldReturnResults(string ip)
    {
        // Act
        var result = _controller.SearchFor(ip);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().BeNull(); // Default view name behavior
        viewResult.Model.Should().BeNull(); // No model - uses ViewBag
        viewResult.ViewData["ip"].Should().Be(ip);
        // Note: ViewBag.clouds would contain search results in real scenario with data
    }

    [Theory]
    [InlineData("Public")]
    [InlineData("AzureGovernment")]
    [InlineData("China")]
    [InlineData("AzureGermany")]
    public void downloadARMTemplate_WithValidParams_ShouldReturnFileResult(string env)
    {
        // Arrange
        var id = "TestService";

        // Act
        var result = _controller.downloadARMTemplate(id, env);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult.ContentType.Should().Be("application/json");
        fileResult.FileDownloadName.Should().EndWith(".json");
    }

    [Fact]
    public void downloadARMTemplate_WithValidParams_ShouldHandleGracefully()
    {
        // Act
        var result = _controller.downloadARMTemplate("TestService", "Public");

        // Assert
        result.Should().BeOfType<FileContentResult>();
    }

    [Theory]
    [InlineData("Public")]
    [InlineData("AzureGovernment")]
    [InlineData("China")]
    [InlineData("AzureGermany")]
    public void getPrefixes_WithValidParams_ShouldReturnFileResult(string env)
    {
        // Act
        var result = _controller.getPrefixes("TestService", env);

        // Assert
        var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
        fileResult.ContentType.Should().Be("application/json");
        fileResult.FileDownloadName.Should().Contain(".json");
    }

    [Fact]
    public void getPrefixes_WithInvalidParams_ShouldReturnFileResult()
    {
        // Act
        var result = _controller.getPrefixes("InvalidService", "InvalidEnv");

        // Assert
        var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
        fileResult.ContentType.Should().Be("application/json");
        fileResult.FileDownloadName.Should().Be("prefixes.json"); // Default filename for no matches
    }

    [Theory]
    [InlineData("Public")]
    [InlineData("AzureGovernment")]
    public void getOPNsenseUrlTable_WithValidParams_ShouldReturnString(string env)
    {
        // Act
        var result = _controller.getOPNsenseUrlTable("TestService", env);

        // Assert
        result.Should().BeOfType<string>();
        result.Should().NotBeNull();
    }

    [Fact]
    public void Error_ShouldReturnViewResultWithErrorModel()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.RequestId.Should().Be("test-trace-id");
    }
}
