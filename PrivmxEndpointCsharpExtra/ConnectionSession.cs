// Module name: PrivmxEndpointCsharpExtra
// File name: ConnectionSession.cs
// Last edit: 2025-02-17 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Thread;
using PrivmxEndpointCsharpExtra.Api;

namespace PrivmxEndpointCsharpExtra;

/// <summary>
/// Container class that wraps single connection and manages its state.
///  
/// </summary>
public sealed class ConnectionSession : IAsyncDisposable
{
	private DisposeBool _disposed;
	private AsyncConnection _connection;
	private AsyncThreadApi _threadApi;
	private AsyncStoreApi _storeApi;
	public IAsyncConnection Connection => _disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _connection;
	public IAsyncThreadApi ThreadApi => _disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _threadApi;

	private ConnectionSession(AsyncConnection connection, AsyncThreadApi threadApi)
	{
		_connection = connection;
		_threadApi = threadApi;
	}

	public static async ValueTask<ConnectionSession> Create(string userPrivateKey, string solutionId, string platformUrl, CancellationToken token = default)
	{
		var connection = await ConnectionAsyncExtensions.ConnectAsync(userPrivateKey, solutionId, platformUrl, token);
		return new ConnectionSession(new AsyncConnection(connection), new AsyncThreadApi(connection));
	}

	public static async ValueTask<ConnectionSession> CreatePublic(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		var connection = await ConnectionAsyncExtensions.ConnectPublicAsync(solutionId, platformUrl, token);
		return new ConnectionSession(new AsyncConnection(connection), new AsyncThreadApi(connection));
	}

	public async ValueTask DisposeAsync()
	{
		if(!_disposed.PerformDispose())
			return;
		await _threadApi.DisposeAsync();
		_threadApi = null!;
		await _connection.DisposeAsync();
		_storeApi = null!;
		_connection = null!;
	}
}