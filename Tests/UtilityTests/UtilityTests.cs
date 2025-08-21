using Xunit;
using FluentAssertions;
using System.Net;
using NetTools;
using System.Collections.Generic;
using System.Text.Json;
using dotnet.Models;

namespace Tests.UtilityTests
{
    public class IPAddressUtilityTests
    {
        [Theory]
        [InlineData("192.168.1.1", "192.168.1.0/24", true)]
        [InlineData("10.0.0.1", "10.0.0.0/8", true)]
        [InlineData("172.16.1.1", "172.16.0.0/12", true)]
        [InlineData("192.168.1.1", "10.0.0.0/8", false)]
        [InlineData("8.8.8.8", "192.168.1.0/24", false)]
        public void IPAddress_Should_Be_Correctly_Identified_In_Range(string ipAddress, string cidr, bool expectedResult)
        {
            // Arrange
            var ip = IPAddress.Parse(ipAddress);
            var range = IPAddressRange.Parse(cidr);

            // Act
            var result = range.Contains(ip);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("13.66.60.119/32")]
        [InlineData("13.66.143.220/30")]
        [InlineData("20.190.128.0/18")]
        [InlineData("40.126.0.0/18")]
        public void Azure_IP_Ranges_Should_Parse_Correctly(string cidr)
        {
            // Act
            var act = () => IPAddressRange.Parse(cidr);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Azure_ActionGroup_IP_Should_Be_In_Range()
        {
            // Arrange
            var testIp = IPAddress.Parse("13.66.60.119");
            var actionGroupRange = IPAddressRange.Parse("13.66.60.119/32");

            // Act
            var result = actionGroupRange.Contains(testIp);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("invalid.ip")]
        [InlineData("999.999.999.999")]
        [InlineData("")]
        // Note: "192.168.1" is actually parsed as "192.168.1.0" by .NET, so removing it
        public void Invalid_IP_Addresses_Should_Throw_Exception(string invalidIp)
        {
            // Act
            var act = () => IPAddress.Parse(invalidIp);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Theory]
        [InlineData("192.168.1.0/24")]
        [InlineData("10.0.0.0/8")]
        [InlineData("172.16.0.0/12")]
        [InlineData("127.0.0.1/32")]
        public void Valid_CIDR_Ranges_Should_Parse_Successfully(string cidr)
        {
            // Act
            var act = () => IPAddressRange.Parse(cidr);

            // Assert
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("192.168.1.0/33")]
        [InlineData("invalid/24")]
        [InlineData("192.168.1.0/")]
        // Note: "192.168.1.0" without CIDR suffix is treated as valid by IPAddressRange library
        public void Invalid_CIDR_Ranges_Should_Throw_Exception(string invalidCidr)
        {
            // Act
            var act = () => IPAddressRange.Parse(invalidCidr);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Theory]
        [InlineData("192.168.1", "192.168.0.1")] // .NET auto-completes partial IPs (last octet becomes host part)
        public void Partial_IP_Addresses_Should_Be_Auto_Completed(string partialIp, string expectedResult)
        {
            // Act
            var result = IPAddress.Parse(partialIp);

            // Assert
            result.ToString().Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("192.168.1.0")] // IPAddressRange accepts bare IPs as single host ranges
        public void Bare_IP_Addresses_Should_Be_Valid_CIDR_Ranges(string bareIp)
        {
            // Act
            var act = () => IPAddressRange.Parse(bareIp);

            // Assert
            act.Should().NotThrow();
        }
    }

    public class JsonSerializationTests
    {
        [Fact]
        public void ServiceTagsModel_Should_Serialize_And_Deserialize_Correctly()
        {
            // Arrange
            var original = new ServiceTagsModel
            {
                changeNumber = 123,
                cloud = "Public",
                values = new List<ValuesModel>
                {
                    new ValuesModel
                    {
                        name = "ActionGroup",
                        id = "ActionGroup",
                        properties = new PropertiesModel
                        {
                            changeNumber = 1,
                            region = "",
                            platform = "Azure",
                            systemService = "ActionGroup",
                            addressPrefixes = new List<string> { "13.66.60.119/32" }
                        }
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            // Act
            var json = JsonSerializer.Serialize(original, options);
            var deserialized = JsonSerializer.Deserialize<ServiceTagsModel>(json, options);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.changeNumber.Should().Be(original.changeNumber);
            deserialized.cloud.Should().Be(original.cloud);
            deserialized.values.Should().HaveCount(1);
            deserialized.values[0].name.Should().Be("ActionGroup");
            deserialized.values[0].properties.addressPrefixes.Should().Contain("13.66.60.119/32");
        }

        [Fact]
        public void ARMModel_Should_Serialize_With_Correct_Schema_Property()
        {
            // Arrange
            var armModel = new ARMModel
            {
                schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                contentVersion = "1.0.0.0"
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            // Act
            var json = JsonSerializer.Serialize(armModel, options);

            // Assert
            json.Should().Contain("\"$schema\"");
            json.Should().Contain("deploymentTemplate.json");
            json.Should().NotContain("\"filename\""); // Should be ignored due to JsonIgnore
        }

        [Fact]
        public void Empty_ServiceTagsModel_Should_Handle_Null_Values()
        {
            // Arrange
            var json = "{}";
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Act
            var result = JsonSerializer.Deserialize<ServiceTagsModel>(json, options);

            // Assert
            result.Should().NotBeNull();
            result.changeNumber.Should().Be(0);
            result.cloud.Should().BeNull();
            result.values.Should().BeNull();
        }
    }

    public class StringUtilityTests
    {
        [Theory]
        [InlineData("ActionGroup", new[] { "ActionGroup" }, 0)]
        [InlineData("ActionGroup", new[] { "AzureActiveDirectory", "ActionGroup" }, 1)]
        [InlineData("NonExistent", new[] { "ActionGroup", "AzureActiveDirectory" }, -1)]
        public void Array_IndexOf_Should_Work_Correctly(string searchValue, string[] array, int expectedIndex)
        {
            // Act
            var result = System.Array.IndexOf(array, searchValue);

            // Assert
            result.Should().Be(expectedIndex);
        }

        [Theory]
        [InlineData("ActionGroup;AzureActiveDirectory", new[] { "ActionGroup", "AzureActiveDirectory" })]
        [InlineData("Single", new[] { "Single" })]
        [InlineData("", new[] { "" })]
        public void String_Split_Should_Work_Correctly(string input, string[] expected)
        {
            // Act
            var result = input.Split(';');

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("   ", false)]
        [InlineData("value", false)]
        public void String_IsNullOrEmpty_Should_Work_Correctly(string input, bool expected)
        {
            // Act
            var result = string.IsNullOrEmpty(input);

            // Assert
            result.Should().Be(expected);
        }
    }

    public class PathUtilityTests
    {
        [Fact]
        public void Path_Combine_Should_Work_Correctly()
        {
            // Arrange
            var basePath = "wwwroot";
            var subPath = "data";
            var fileName = "Public.json";

            // Act
            var result = System.IO.Path.Combine(basePath, subPath, fileName);

            // Assert
            result.Should().Be(System.IO.Path.Combine("wwwroot", "data", "Public.json"));
        }

        [Theory]
        [InlineData("test.json", "")]
        [InlineData("path/to/file.txt", "path/to")]
        [InlineData("noextension", "")]
        public void Path_GetDirectoryName_Should_Work_Correctly(string path, string expected)
        {
            // Act
            var result = System.IO.Path.GetDirectoryName(path);

            // Assert
            result.Should().Be(expected);
        }
    }
}
