// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncConnection.cs
// Last edit: 2025-02-19 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.ComponentModel;
using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Api;

/// <summary>
///     Main container that manages connection and exposes asynchronous API.
/// </summary>
public sealed class AsyncConnection : IAsyncDisposable, IAsyncConnection
{
	private static readonly Logger.SourcedLogger<AsyncConnection> Logger = default;
	private DisposeBool _disposeBool;

	/// <summary>
	///     Wraps existing connection into async connection.
	///     It's user responsibility to provide valid (connected) connection.
	/// </summary>
	/// <param name="connection">Connection to wrap</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncConnection(IConnection connection)
	{
		Connection = connection;
	}

	private IConnection Connection { get; }

	public long GetConnectionId()
	{
		_disposeBool.ThrowIfDisposed(nameof(AsyncConnection));
		return Connection.GetConnectionId();
	}

	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.ListContexts" />
	/// <param name="token">Cancellation token</param>
	public ValueTask<PagingList<Context>> ListContexts(
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		_disposeBool.ThrowIfDisposed(nameof(Connection));
		return Connection.ListContextsAsync(pagingQuery, token);
	}

	/// <summary>
	///     Disposes async connection with all related resources.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		if (_disposeBool.PerformDispose())
			return Connection.DisconnectAsync();
		return default;
	}

	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.Connect" />
	/// <param name="token">Cancellation token</param>
	/// <returns>Async connection.</returns>
	public static async Task<AsyncConnection> Connect(string userPrivateKey, string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		Logger.Log(LogLevel.Trace, "Connecting to {0}, solution {1}, with userKey {2}", platformUrl, solutionId,
			userPrivateKey);
		var connection = await ConnectionAsyncExtensions.ConnectAsync(userPrivateKey, solutionId, platformUrl, token);
		return new AsyncConnection(connection);
	}

	/// <inheritdoc cref="PrivMX.Endpoint.Core.Connection.ConnectPublic" />
	/// <param name="token">Cancellation token</param>
	/// <returns>Async connection.</returns>
	public static async Task<AsyncConnection> ConnectPublic(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		Logger.Log(LogLevel.Trace, "Connecting to {0}, solution {1} as public", platformUrl, solutionId);
		var connection = await ConnectionAsyncExtensions.ConnectPublicAsync(solutionId, platformUrl, token);
		return new AsyncConnection(connection);
	}
}