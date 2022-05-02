// <copyright file="AppSettings.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Core.Models;

public class ApplicationSettings
{
	public const string ConfigurationName = "ApplicationSettings";

	// email send

	public string? MessageToEmailAddress { get; set; } = string.Empty;

	public string? MessageFromEmailAddress { get; set; } = string.Empty;

	public string? MessageReplyToEmailAddress { get; set; }

	public EmailSettings Email { get; set; }

	public ApplicationSettings()
	{
		this.Email = new EmailSettings(); // avoid nullable type warning
	}
}
