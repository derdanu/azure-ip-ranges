using Xunit;
using FluentAssertions;
using dotnet.Models;

namespace Tests.UnitTests.Models;

public class ErrorViewModelTests
{
    [Fact]
    public void ErrorViewModel_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var model = new ErrorViewModel();

        // Assert
        model.RequestId.Should().BeNull();
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_WithNullRequestId_ShouldReturnFalse()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };

        // Act & Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    public void ShowRequestId_WithEmptyRequestId_ShouldReturnFalse(string requestId)
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = requestId };

        // Act & Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(" \r\n \t ")]
    public void ShowRequestId_WithWhitespaceRequestId_ShouldReturnTrue(string requestId)
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = requestId };

        // Act & Assert - The implementation uses IsNullOrEmpty, so whitespace returns true
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void ShowRequestId_WithValidRequestId_ShouldReturnTrue()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "abc123" };

        // Act & Assert
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void RequestId_SetAndGet_ShouldWorkCorrectly()
    {
        // Arrange
        var requestId = "test-request-123";
        var model = new ErrorViewModel();

        // Act
        model.RequestId = requestId;

        // Assert
        model.RequestId.Should().Be(requestId);
        model.ShowRequestId.Should().BeTrue();
    }
}
