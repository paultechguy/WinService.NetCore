// <copyright file="EmailSender.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Email;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using WinService.NetCore.Core;
using WinService.NetCore.Core.Models;

public class EmailSender : IEmailSender
{
	private readonly EmailSettings emailSettings;
	private readonly ILogger<EmailSender> logger;

	public EmailSender(
		IOptions<EmailSettings> emailSettings,
		ILogger<EmailSender> logger)
	{
		this.emailSettings = emailSettings.Value;
		this.logger = logger;
	}

	public async Task SendPlainAsync(
		string from,
		string to,
		string subject,
		string body)
	{
		await this.SendAsync(from, to, subject, bodyPlain: body, bodyHtml: null, replyTo: null);
	}

	public async Task SendHtmlAsync(
		string from,
		string to,
		string subject,
		string body)
	{
		await this.SendAsync(from, to, subject, bodyPlain: null, bodyHtml: body, replyTo: null);
	}

	public async Task SendAsync(
		string from,
		string to,
		string subject,
		string? bodyPlain = null,
		string? bodyHtml = null,
		string? replyTo = null)
	{
		using var message = new MailMessage();
		message.SubjectEncoding = System.Text.Encoding.UTF8;
		message.From = new MailAddress(from ?? throw new ArgumentNullException(nameof(from)));
		message.To.Add(to ?? throw new ArgumentNullException(nameof(to)));
		message.Body = bodyPlain ?? string.Empty; // blank body text if none provided
		message.Subject = subject ?? throw new ArgumentNullException(nameof(subject));

		// do we have a reply to email address
		if (replyTo is not null)
		{
			message.ReplyToList.Add(replyTo);
		}

		// html version
		if (bodyHtml is not null)
		{
			var htmlView = AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, "text/html");
			message.AlternateViews.Add(htmlView);
		}

		using var client = new SmtpClient(this.emailSettings.ServerHost, this.emailSettings.ServerPort);
		client.EnableSsl = this.emailSettings.ServerEnableSsl;
		client.UseDefaultCredentials = string.IsNullOrWhiteSpace(this.emailSettings.ServerUsername);
		if (!client.UseDefaultCredentials)
		{
			client.Credentials = new NetworkCredential(this.emailSettings.ServerUsername, this.emailSettings.ServerPassword);
		}

		await client.SendMailAsync(message);
	}
}
