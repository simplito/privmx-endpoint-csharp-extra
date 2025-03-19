// Module name: PrivmxEndpointCsharpExtra
// File name: ConnectionSession.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Extra.Api;
using PrivMX.Endpoint.Extra.Api.Interfaces;
using PrivMX.Endpoint.Extra.Events.Internal;
using PrivMX.Endpoint.Extra.Internals;
using System.Diagnostics.CodeAnalysis;

namespace PrivMX.Endpoint.Extra;

/// <summary>
///     Container class that wraps single connection and manages its state.
///     Exposes all APIs that are available for the connection.
///     Disposing connection session automatically disposes all APIs.
/// </summary>
public sealed class ConnectionSession : IAsyncDisposable
{
	private static readonly Logger.SourcedLogger<ConnectionSession> Logger = default;
	private readonly AsyncConnection _connection;
	private readonly AsyncInboxApi _inboxApi;
	private readonly AsyncStoreApi _storeApi;
	private readonly AsyncThreadApi _threadApi;
	private DisposeBool _disposed;

	private ConnectionSession(string publicKey, string privateKey, AsyncConnection connection, AsyncThreadApi threadApi,
		AsyncStoreApi storeApi, AsyncInboxApi inboxApi)
	{
		_connection = connection;
		_threadApi = threadApi;
		_storeApi = storeApi;
		_inboxApi = inboxApi;
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

	/// <summary>
	///     Connection asynchronous api.
	/// </summary>
	/// <exception cref="ObjectDisposedException">Thrown if ConnectionSession is disposed.</exception>
	public IAsyncConnection Connection =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _connection;

	/// <summary>
	///     Threads asynchronous api.
	/// </summary>
	/// <exception cref="ObjectDisposedException">Thrown if ConnectionSession is disposed.</exception>
	public IAsyncThreadApi ThreadApi =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _threadApi;

	/// <summary>
	///     Store asynchronous api.
	/// </summary>
	/// <exception cref="ObjectDisposedException">Thrown if ConnectionSession is disposed.</exception>
	public IAsyncStoreApi StoreApi =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _storeApi;

	/// <summary>
	///     Inbox asynchronous api.
	/// </summary>
	/// <exception cref="ObjectDisposedException">Thrown if ConnectionSession is disposed.</exception>
	public IAsyncInboxApi InboxApi =>
		_disposed ? throw new ObjectDisposedException(nameof(ConnectionSession)) : _inboxApi;

	/// <summary>
	///     Disposes session and frees are underlying resources.
	/// </summary>
	[SuppressMessage("Reliability", "CA2012")]
	public async ValueTask DisposeAsync()
	{
		if (!_disposed.PerformDispose())
			return;
		try
		{
			await ValueTaskTools.WhenAll(_connection.DisposeAsync(), _threadApi.DisposeAsync(),
				_storeApi.DisposeAsync(), _inboxApi.DisposeAsync());
		}
		catch (Exception e)
		{
			Logger.Log(LogLevel.Error, "Unhandled exception during dispose.", e);
			Internals.Logger.PublishUnobservedException(e);
		}
	}

	/// <summary>
	///     Creates new session with authenticated user.
	/// </summary>
	/// <param name="userPrivateKey">User private key.</param>
	/// <param name="publicKey">User public key.</param>
	/// <param name="solutionId">Bridge solution ID.</param>
	/// <param name="platformUrl">Bridge url.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Authorized connection session.</returns>
	public static async ValueTask<ConnectionSession> Create(string userPrivateKey, string publicKey, string solutionId,
		string platformUrl, CancellationToken token = default)
	{
		var connection = await ConnectionAsyncExtensions.ConnectAsync(userPrivateKey, solutionId, platformUrl, token);
		return CreateInternal(connection, publicKey, userPrivateKey);
	}

	/// <summary>
	///     Creates new public session (not authenticated user).
	/// </summary>
	/// <param name="solutionId">Bridge solution ID.</param>
	/// <param name="platformUrl">Bridge url.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Not authorized connection session.</returns>
	public static async ValueTask<ConnectionSession> CreatePublic(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		var connection = await ConnectionAsyncExtensions.ConnectPublicAsync(solutionId, platformUrl, token);
		return CreateInternal(connection, string.Empty, string.Empty);
	}

	private static ConnectionSession CreateInternal(Connection connection, string publicKey, string privateKey)
	{
		var connectionId = connection.GetConnectionId();
		var asyncConnection = new AsyncConnection(connection);
		var threadApi = PrivMX.Endpoint.Thread.ThreadApi.Create(connection);
		var storeApi = PrivMX.Endpoint.Store.StoreApi.Create(connection);
		var inboxApi = PrivMX.Endpoint.Inbox.InboxApi.Create(connection, threadApi, storeApi);
		var eventDispatcher = PrivMXEventDispatcher.Instance;
		var asyncThreadApi = new AsyncThreadApi(threadApi, connectionId, eventDispatcher);
		var asyncStoreApi = new AsyncStoreApi(storeApi, connectionId, eventDispatcher);
		var asyncInboxApi = new AsyncInboxApi(inboxApi, connectionId, eventDispatcher);
		return new ConnectionSession(publicKey, privateKey, asyncConnection, asyncThreadApi, asyncStoreApi,
			asyncInboxApi);
	}
}