// <copyright file="CommandLineOptions.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Models;

using CommandLine;

public class CommandLineOptions
{
	[Option('x', "xxxxxx", Required = false, HelpText = "Example cmdline option")]
	public bool Xxxxxx { get; set; }
}
