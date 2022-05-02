// <copyright file="IApplicationService.cs" company="CompanyName">
// Copyright (c) CompanyName. All rights reserved.
// </copyright>

namespace WinService.NetCore.Core;

using System.Threading.Tasks;

public interface IApplicationService
{
	Task ExecuteAsync(CancellationToken cancelToken);
}
