// <copyright file="Program.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore;

using System;
using System.Threading;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WinService.NetCore.Application;
using WinService.NetCore.Core;
using WinService.NetCore.Core.Models;
using WinService.NetCore.Email;
using WinService.NetCore.Helpers;
using WinService.NetCore.Models;

public class Program : IDisposable
{
	// this value really doesn't do much since the service name is passed in
	// via the CLI, Windows sc.exe command, for the various commands like
	// create, start, stop, etc.
	private const string ServiceName = $"NET Core Windows Service";

	private static readonly CancellationTokenSource CancelTokenSource = new CancellationTokenSource();

	// assume we are in a production environment; also note the "DOTNETCORE_" vs. "ASPNETCORE_" environment
	private static string environmentName = (Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT") ?? "production").ToLower();

	private bool disposed = false;

	public void Dispose()
	{
		// Dispose of unmanaged resources.
		this.Dispose(true);

		// Suppress finalization.
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (this.disposed)
		{
			return;
		}

		if (disposing)
		{
			CancelTokenSource.Dispose();
		}

		// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
		// TODO: set large fields to null.

		this.disposed = true;
	}

	private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
	{
		// logging
		CreateLogger(hostContext.Configuration);

		// app settings DI using IOptions pattern
		services.Configure<ApplicationSettings>(hostContext.Configuration.GetSection(ApplicationSettings.ConfigurationName));
		services.Configure<EmailSettings>(hostContext.Configuration.GetSection(EmailSettings.ConfigurationName));

		// normal DI stuff
		services.AddTransient<IApplicationService, ApplicationService>();
		services.AddTransient<IEmailSender, EmailSender>();
		services.AddHostedService<WindowsBackgroundService>();
	}

	private static void CreateLogger(IConfiguration configuration)
	{
		// uses Serilog
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();
	}

	private static IHostBuilder CreateHostBuilder(string[] args)
	{
		var builder = Host.CreateDefaultBuilder(args)
			.ConfigureHostConfiguration(configHost =>
			{
				configHost.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
					.AddEnvironmentVariables()
					.AddCommandLine(Environment.GetCommandLineArgs());
			})
			.UseWindowsService(options =>
			{
				options.ServiceName = ServiceName;
			})
			.ConfigureServices(ConfigureServices)
			.UseSerilog();

		return builder;
	}

	/// <summary>
	/// -----------------------------------------------------
	/// MAIN WINDOWS SERVICE HERE.
	/// -----------------------------------------------------
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	/// <returns>Zero if success; otherwise a non-zero error code.</returns>
	private static int Main(string[] args)
	{
		int exit = 0;
		ParserResult<CommandLineOptions> result = Parser.Default.ParseArguments<CommandLineOptions>(args)
		.WithParsed(options =>
			{
				try
				{
					InitializeEnvironment();
					var host = CreateHostBuilder(args).Build();
					NotifyInteractiveUser();
					host.RunAsync(CancelTokenSource.Token).Wait(); // wait
				}
				catch (Exception ex)
				{
					string exMesage = $"Top-level application exception caught: {ex}";

					// logger never initialized successfully
					if (Log.Logger.GetType().Name == "SilentLogger")
					{
						Console.Error.WriteLine(exMesage);
					}
					else
					{
						Log.Logger.Information(exMesage);
					}
				}
			})
		.WithNotParsed(errors => // errors is a sequence of type IEnumerable<Error>
		{
			exit = 0;
		});

		// all done...flush output
		FlushOutput();

		return exit;
	}

	private static void NotifyInteractiveUser()
	{
		if (Environment.UserInteractive)
		{
			Log.Logger.Information("{msg}", "Press Ctrl-C to cancel");
		}
	}

	private static void FlushOutput()
	{
		Log.CloseAndFlush();

		if (Environment.UserInteractive)
		{
			Console.Out.Flush();
		}
	}

	private static void InitializeEnvironment()
	{
		// set directory to the main app so things like the app settings file(s) are found
		Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

		// allow ctrl-c in case running in console mode
		ConfigureCtrlCHandler();
	}

	private static void ConfigureCtrlCHandler()
	{
		// allow cancellable Ctrl-C if interactive
		if (Environment.UserInteractive)
		{
			CtrlCHelper.ConfigureCtrlCHandler((sender, e) =>
			{
				// if ctrl-c pressed
				if (!e.Cancel && e.SpecialKey == ConsoleSpecialKey.ControlC)
				{
					CancelTokenSource.Cancel();
					e.Cancel = true;
				}
			});
		}
	}
}
