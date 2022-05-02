// <copyright file="IEmailSender.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Core;

using System.Threading.Tasks;

public interface IEmailSender
{
	Task SendPlainAsync(
		string from,
		string to,
		string subject,
		string body);

	Task SendHtmlAsync(
		string from,
		string to,
		string subject,
		string body);

	Task SendAsync(
		string from,
		string to,
		string subject,
		string? replyTo = null,
		string? bodyText = null,
		string? bodyHtml = null);
}