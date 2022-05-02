// <copyright file="EmailSettings.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Core.Models;

public class EmailSettings
{
	public const string ConfigurationName = "EmailSettings";

	// email server

	public string? ServerHost { get; set; }

	public int ServerPort { get; set; }

	public bool ServerEnableSsl { get; set; }

	public string? ServerUsername { get; set; }

	public string? ServerPassword { get; set; }
}
