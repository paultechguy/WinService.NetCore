// <copyright file="CtrlCHelper.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Helpers;

public static class CtrlCHelper
{
	public static void ConfigureCtrlCHandler(ConsoleCancelEventHandler handler)
	{
		Console.CancelKeyPress += handler;
	}
}
