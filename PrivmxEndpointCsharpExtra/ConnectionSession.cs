// Module name: PrivmxEndpointCsharpExtra
// File name: ConnectionSession.cs
// Last edit: 2025-02-19 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivmxEndpointCsharpExtra.Api;
using PrivmxEndpointCsharpExtra.Api.Interfaces;

namespace PrivmxEndpointCsharpExtra;

/// <summary>
///     Container class that wraps single connection and manages its state.
/// </summary>
public sealed class ConnectionSession : IAsyncDisposable
{
	private readonly AsyncConnection _connection;
	private readonly AsyncStoreApi _storeApi;
	private readonly AsyncThreadApi _threadApi;
	private DisposeBool _disposed;

	private ConnectionSession(string publicKey, string privateKey, AsyncConnection connection, AsyncThreadApi threadApi,
		AsyncStoreApi storeApi)
	{
		_connection = connection;
		_threadApi = threadApi;
		_storeApi = storeApi;
		PublicKey = publicKey;
		PrivateKey = privateKey;
	}

	/// <summary>
	///     Public key of the user. If connection is anonymous, this will be empty.
	/// </summary>
	public string PublicKey { get; }

	/// <summary>
	///     Private key of the user. If connection is anonymous, this will be empty.
	/// </summary>
	public string PrivateKey { get; }

	public IAsyncConnection Connection =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _connection;

	public IAsyncThreadApi ThreadApi =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _threadApi;

	public IAsyncStoreApi StoreApi =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _storeApi;

	public async ValueTask DisposeAsync()
	{
		if (!_disposed.PerformDispose())
			return;
		await _connection.DisposeAsync();
		await _threadApi.DisposeAsync();
		await _storeApi.DisposeAsync();
	}

	public static async ValueTask<ConnectionSession> Create(string userPrivateKey, string publicKey, string solutionId,
		string platformUrl, CancellationToken token = default)
	{
		var connection = await ConnectionAsyncExtensions.ConnectAsync(userPrivateKey, solutionId, platformUrl, token);
		return new ConnectionSession(publicKey, userPrivateKey, new AsyncConnection(connection),
			new AsyncThreadApi(connection), new AsyncStoreApi(connection));
	}

	public static async ValueTask<ConnectionSession> CreatePublic(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		var connection = await ConnectionAsyncExtensions.ConnectPublicAsync(solutionId, platformUrl, token);
		return new ConnectionSession(string.Empty, string.Empty, new AsyncConnection(connection),
			new AsyncThreadApi(connection), new AsyncStoreApi(connection));
	}
}