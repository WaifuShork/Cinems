using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Cinems
{
	internal static class Program
	{
		private static async Task Main(string[] args)
		{
			// Build logging
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Async(lConfig =>
				{
					lConfig.Console();
					lConfig.File(Path.Combine("logs", "logs-.txt"), rollingInterval: RollingInterval.Day, shared: true);
				})
				.CreateLogger();

			try
			{
				var host = CreateHostBuilder(args).Build();
				await host.RunAsync();
			}
			catch (Exception exception)
			{
				Log.Fatal(exception, "Host terminated unexpectedly");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.UseSerilog()
				.ConfigureHostConfiguration(config =>
				{
					config.SetBasePath(Directory.GetCurrentDirectory());
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
		}
	}
}