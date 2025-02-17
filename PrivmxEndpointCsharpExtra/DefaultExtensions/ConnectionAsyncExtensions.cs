// Module name: PrivmxEndpointCsharpExtra
// File name: ConnectionAsyncExtensions.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra;

/// <summary>
///     Extension methods that provide asynchronous method execution for objects implementing <see cref="IConnection" />
///     interface.
///     Internally operations are executed using default <see cref="ThreadPool" />.
/// </summary>
public static class ConnectionAsyncExtensions
{
	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.Connect" />
	/// <param name="token">Cancellation token</param>
	public static ValueTask<Connection> ConnectAsync(string userPrivKey, string solutionId,
		string platformUrl, CancellationToken token = default)
	{
		return WrapperCallsExecutor.Execute(
			() => Connection.Connect(userPrivKey, solutionId, platformUrl), token);
	}

	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.ConnectPublic" />
	/// <param name="token">Cancellation token</param>
	public static ValueTask<Connection> ConnectPublicAsync(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		return WrapperCallsExecutor.Execute(() => Connection.ConnectPublic(solutionId, platformUrl), token);
	}

	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.ListContexts" />
	/// <param name="token">Cancellation token</param>
	public static ValueTask<PagingList<Context>> ListContextsAsync(this IConnection connection,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (connection is null)
			throw new ArgumentNullException(nameof(connection));
		return WrapperCallsExecutor.Execute(() => connection.ListContexts(pagingQuery), token);
	}

	public static ValueTask DisconnectAsync(this IConnection connection, CancellationToken token = default)
	{
		if (connection is null)
			throw new ArgumentNullException(nameof(connection));
		return WrapperCallsExecutor.Execute(connection.Disconnect, token);
	}

	public static ValueTask<long> GetConnectionIdAsync(this IConnection connection, CancellationToken token = default)
	{
		if (connection is null)
			throw new ArgumentNullException(nameof(connection));
		return WrapperCallsExecutor.Execute(connection.GetConnectionId, token);
	}
}