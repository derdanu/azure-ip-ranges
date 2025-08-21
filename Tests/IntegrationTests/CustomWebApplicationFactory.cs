using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<dotnet.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add any test-specific services here
            
            // Configure test logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Load test configuration
            var testConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Testing.json");
            if (File.Exists(testConfigPath))
            {
                config.AddJsonFile("appsettings.Testing.json", optional: true);
            }
        });

        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
