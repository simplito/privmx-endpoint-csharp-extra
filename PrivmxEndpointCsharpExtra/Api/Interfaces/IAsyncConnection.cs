// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncConnection.cs
// Last edit: 2025-02-17 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;

namespace PrivmxEndpointCsharpExtra.Api;

public interface IAsyncConnection
{
	public long GetConnectionId();
	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.ListContexts" />
	/// <param name="token">Cancellation token</param>
	ValueTask<PagingList<Context>> ListContexts(
		PagingQuery pagingQuery, CancellationToken token = default);
}