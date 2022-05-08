// <copyright file="AppService.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Application;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WinService.NetCore.Core;
using WinService.NetCore.Core.Models;

public class ApplicationService : IApplicationService
{
	private readonly ILogger<ApplicationService> logger;
	private readonly ApplicationSettings appSettings;
	private readonly IEmailSender emailSender;
	private const int sleepMs = 5000;

	public ApplicationService(
		ILogger<ApplicationService> logger,
		IOptions<ApplicationSettings> appSettings,
		IEmailSender emailSender)
	{
		this.logger = logger;
		this.appSettings = appSettings.Value;
		this.emailSender = emailSender;
	}

	public Task ExecuteAsync(CancellationToken cancelToken)
	{
		this.logger.LogInformation($"Starting: {nameof(ApplicationService)}.{nameof(this.ExecuteAsync)}");

		this.SendEmailAsync(
				$"Email from {nameof(ApplicationService)}",
				$"<html><head></head><body><h1>Hello World!</h1><p>I like it.</p></body></html>"
			).Wait(cancelToken); // wait

		while (!cancelToken.IsCancellationRequested)
		{
			this.logger.LogInformation($"Doing somthing important every {sleepMs} milliseconds: {nameof(ApplicationService)}.{nameof(this.ExecuteAsync)}...");
			cancelToken.WaitHandle.WaitOne(sleepMs);
		}

		this.logger.LogInformation($"Ending: {nameof(ApplicationService)}.{nameof(this.ExecuteAsync)}");

		return Task.CompletedTask;
	}

	private async Task SendEmailAsync(string subject, string body)
	{
		// if it appears we have email settings, send an email we are starting
		if (!string.IsNullOrWhiteSpace(this.appSettings.MessageFromEmailAddress)
			&& !string.IsNullOrWhiteSpace(this.appSettings.MessageToEmailAddress))
		{
			this.logger.LogInformation("Sending email; to:{to}, subject:{subject}", this.appSettings.MessageToEmailAddress, subject);

			await emailSender.SendHtmlAsync(
				this.appSettings.MessageFromEmailAddress,
				this.appSettings.MessageToEmailAddress,
				subject,
				body);
		}
	}
}
