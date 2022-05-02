// <copyright file="WindowsBackgroundService.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Core;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class WindowsBackgroundService : BackgroundService
{
	private readonly IApplicationService appService;
	private readonly ILogger<WindowsBackgroundService> logger;

	public WindowsBackgroundService(
		 IApplicationService appService,
		 ILogger<WindowsBackgroundService> logger) =>
		 (this.appService, this.logger) = (appService, logger);

	protected override async Task ExecuteAsync(CancellationToken cancelToken)
	{
		while (!cancelToken.IsCancellationRequested)
		{
			this.logger.LogInformation($"Starting {nameof(WindowsBackgroundService)}.{nameof(this.ExecuteAsync)}");

			try
			{
				await this.appService.ExecuteAsync(cancelToken);
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Exception: {ex}");
				break;
			}
		}

		this.logger.LogInformation($"Ending {nameof(WindowsBackgroundService)}.{nameof(this.ExecuteAsync)}");
	}
}