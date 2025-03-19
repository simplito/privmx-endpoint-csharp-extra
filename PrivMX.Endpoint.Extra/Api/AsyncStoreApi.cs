// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncStoreApi.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Abstractions;
using PrivMX.Endpoint.Extra.Api.Interfaces;
using PrivMX.Endpoint.Extra.Events;
using PrivMX.Endpoint.Extra.Events.Internal;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Extra.Store;
using PrivMX.Endpoint.Store;
using PrivMX.Endpoint.Store.Models;
using PrivMX.Endpoint.Thread.Models;
using System.ComponentModel;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivMX.Endpoint.Extra.Api;

/// <summary>
///     Store api container that manages connection and exposes asynchronous API.
/// </summary>
public class AsyncStoreApi : IAsyncDisposable, IDisposable, IAsyncStoreApi
{
	private readonly long _connectionId;
	private readonly IEventDispatcher _eventDispatcher;
	private DisposeBool _disposed;
	private IStoreApi _storeApi;
	private StoreChannelEnventDispatcher _storeChannelEventDispatcher;
	private Dictionary<string, StoreFileEventDispatcher> _storeFileDispatchers;

	/// <summary>
	///     Wraps existing thread api into async thread api.
	///     This constructor is meant to be used in advanced scenarios like object mocking and testing.
	/// </summary>
	/// <param name="storeApi">Existing thread API.</param>
	/// <param name="connectionId">ID of user connection.</param>
	/// <param name="eventDispatcher">Event dispatcher used as event source.</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncStoreApi(IStoreApi storeApi, long connectionId, IEventDispatcher eventDispatcher)
	{
		_storeApi = storeApi;
		_connectionId = connectionId;
		_eventDispatcher = eventDispatcher;
		_storeChannelEventDispatcher = new StoreChannelEnventDispatcher(_storeApi, connectionId, eventDispatcher);
		_storeFileDispatchers = new Dictionary<string, StoreFileEventDispatcher>();
	}

	/// <summary>
	///     Creates a new instance of the store api over real privmx connection.
	/// </summary>
	/// <param name="connection"></param>
	public AsyncStoreApi(Connection connection) : this(StoreApi.Create(connection), connection.GetConnectionId(),
		PrivMXEventDispatcher.Instance)
	{
	}

	/// <summary>
	///     Disposes store api.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

	/// <summary>
	///     Creates a new Store in given Context.
	/// </summary>
	/// <param name="contextId">ID of the Context to create the Store in.</param>
	/// <param name="users">Array of UserWithPubKey structs which indicates who will have access to the created Store.</param>
	/// <param name="managers">
	///     Array of UserWithPubKey structs which indicates who will have access (and management rights) to
	///     the created Store.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) metadata.</param>
	/// <param name="privateMeta">Private (encrypted) metadata.</param>
	/// <param name="containerPolicy">Store policy.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Created Store ID.</returns>
	/// ///
	/// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
	public ValueTask<string> CreateStore(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, ContainerPolicy containerPolicy, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.CreateStoreAsync(contextId, users, managers, publicMeta, privateMeta, containerPolicy, token);
	}

	/// <summary>
	///     Updates an existing Store.
	/// </summary>
	/// <param name="storeId">ID of the Store to update.</param>
	/// <param name="users">Array of UserWithPubKey structs which indicates who will have access to the created Store.</param>
	/// <param name="managers">
	///     Array of UserWithPubKey structs which indicates who will have access (and management rights) to
	///     the created Store.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) metadata.</param>
	/// <param name="privateMeta">Private (encrypted) metadata.</param>
	/// <param name="version">Current version of the updated Store.</param>
	/// <param name="force">Force update (without checking version).</param>
	/// <param name="forceGenerateNewKey">Force to regenerate a key for the Store.</param>
	/// <param name="containerPolicy">(optional) Store policy.</param>
	/// <param name="token">Cancellation token.</param>
	/// ///
	/// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
	public ValueTask UpdateStore(string storeId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? containerPolicy = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.UpdateStoreAsync(storeId, users, managers, publicMeta, privateMeta, version, force,
			forceGenerateNewKey, containerPolicy, token);
	}

	/// <summary>
	///     Deletes a Store by given Store ID.
	/// </summary>
	/// <param name="storeId">ID of the Store to delete.</param>
	/// <param name="token">Cancellation token.</param>
	/// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
	public ValueTask DeleteStore(string storeId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.DeleteStoreAsync(storeId, token);
	}

	/// <summary>
	///     Gets a single Store by given Store ID.
	/// </summary>
	/// <param name="storeId">ID of the store to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Information about about the Store.</returns>
	public ValueTask<PrivMX.Endpoint.Store.Models.Store> GetStore(string storeId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.GetStoreAsync(storeId, token);
	}

	/// <summary>
	///     Gets information about existing file.
	/// </summary>
	/// <param name="fileId">ID of the file to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Store file metadata.</returns>
	public ValueTask<File> GetFile(string fileId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.GetFileAsync(fileId, token);
	}

	/// <summary>
	///     Gets a list of Stores in given Context.
	/// </summary>
	/// <param name="contextId">ID of the Context to get the Stores from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of Stores.</returns>
	public ValueTask<PagingList<PrivMX.Endpoint.Store.Models.Store>> ListStores(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.ListStoresAsync(contextId, pagingQuery, token);
	}

	/// <inheritdoc />
	public ValueTask<PagingList<File>> ListFiles(string storeId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.ListFilesAsync(storeId, pagingQuery, token);
	}

	/// <summary>
	///     Creates a new file in a Store.
	/// </summary>
	/// <param name="storeId">ID of the Store to create the file in.</param>
	/// <param name="publicMeta">Public file meta_data.</param>
	/// <param name="privateMeta">Private file meta_data.</param>
	/// <param name="size">Size of the file.</param>
	/// <param name="fillValue">Optional value to fill empty space in file stream on close.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Fixed size file stream that supports write operations.</returns>
	public async ValueTask<PrivmxFileStream> CreateFile(string storeId, long size, byte[] publicMeta,
		byte[] privateMeta, byte? fillValue = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		token.ThrowIfCancellationRequested();
		var handle = await _storeApi.CreateFileAsync(storeId, publicMeta, privateMeta, size, token);
		return new StoreWriteFileStream(null, handle, size, publicMeta, privateMeta, fillValue, _storeApi);
	}

	/// <inheritdoc />
	public ValueTask DeleteFile(string fileId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		token.ThrowIfCancellationRequested();
		return _storeApi.DeleteFileAsync(fileId, token);
	}

	/// <summary>
	///     Opens a file for write.
	/// </summary>
	/// <param name="fileId">ID of the file to update.</param>
	/// <param name="size">New file size.</param>
	/// <param name="publicMeta">Public file meta data.></param>
	/// <param name="privateMeta">Private file meta data.</param>
	/// <param name="fillValue">Optional value to fill empty space in file stream on close.</param>
	/// <param name="token">Cancellation token</param>
	/// <returns>Fixed size file stream that supports write operations.</returns>
	public async ValueTask<PrivmxFileStream> OpenFileForWrite(string fileId, long size, byte[] publicMeta,
		byte[] privateMeta, byte? fillValue = null,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		token.ThrowIfCancellationRequested();
		var handle = await _storeApi.UpdateFileAsync(fileId, publicMeta, privateMeta, size, token);
		return new StoreWriteFileStream(fileId, handle, size, publicMeta, privateMeta, fillValue, _storeApi);
	}

	/// <inheritdoc />
	public async ValueTask<PrivmxFileStream> OpenFileForRead(string fileId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		var meta = _storeApi.GetFileAsync(fileId, token).AsTask();
		var handle = _storeApi.OpenFileAsync(fileId, token).AsTask();
		await Task.WhenAll(meta, handle);
		return new StoreReadonlyFileStream(fileId, meta.Result.Size, handle.Result, _storeApi,
			meta.Result.PublicMeta, meta.Result.PrivateMeta);
	}

	/// <inheritdoc />
	public ValueTask UpdateFileMetaAsync(string fileId, byte[] publicMeta, byte[] privateMeta,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.UpdateFileMetaAsync(fileId, publicMeta, privateMeta, token);
	}

	/// <inheritdoc />
	public IObservable<StoreEvent> GetStoreEvents()
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeChannelEventDispatcher;
	}

	/// <inheritdoc />
	public IObservable<StoreFileEvent> GetFileEvents(string storeId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		lock (_storeFileDispatchers)
		{
			if (!_storeFileDispatchers.TryGetValue(storeId, out var dispatcher))
			{
				dispatcher =
					new StoreFileEventDispatcher(storeId, _storeApi, _connectionId,
						_eventDispatcher);
				_storeFileDispatchers.Add(storeId, dispatcher);
			}

			return dispatcher;
		}
	}

	/// <summary>
	///     Disposes store api.
	/// </summary>
	/// <exception cref="AggregateException">Grouped exception that may occur during dispose.</exception>
	public void Dispose()
	{
		if (!_disposed.PerformDispose())
			return;
		var exceptions = _storeFileDispatchers.Values.ForEachNotThrowing(dispatcher => dispatcher.Dispose());
		_storeFileDispatchers = null!;
		try
		{
			_storeChannelEventDispatcher.Dispose();
		}
		catch (Exception exception)
		{
			(exceptions ??= new List<Exception>()).Add(exception);
		}

		_storeChannelEventDispatcher = null!;
		_storeApi = null!;
		if (exceptions is not null)
		{
			var exception = new AggregateException(exceptions);
			throw exception;
		}
	}

	private class StoreChannelEnventDispatcher(
		IStoreApi connection,
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<StoreEvent>("store", connectionId, eventDispatcher)
	{
		private IStoreApi Connection { get; } = connection;

		protected override void OpenChanel()
		{
			Connection.SubscribeForStoreEvents();
		}

		protected override void CloseChanel()
		{
			Connection.UnsubscribeFromStoreEvents();
		}

		public override void HandleEvent(Event @event)
		{
			switch (@event)
			{
				case StoreCreatedEvent createdEvent:
					WrappedInvokeObservable.Send(
						new StoreEvent(createdEvent));
					break;
				case StoreDeletedEvent deletedEvent:
					WrappedInvokeObservable.Send(
						new StoreEvent(deletedEvent));
					break;
				case StoreUpdatedEvent updatedEvent:
					WrappedInvokeObservable.Send(
						new StoreEvent(updatedEvent));
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}

	private class StoreFileEventDispatcher(
		string storeId,
		IStoreApi connection,
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<StoreFileEvent>($"store/{storeId}/files", connectionId, eventDispatcher)
	{
		private IStoreApi Connection { get; } = connection;

		protected override void OpenChanel()
		{
			Connection.SubscribeForFileEvents(storeId);
		}

		protected override void CloseChanel()
		{
			Connection.UnsubscribeFromFileEvents(storeId);
		}

		public override void HandleEvent(Event @event)
		{
			switch (@event)
			{
				case StoreFileCreatedEvent createdEvent:
					WrappedInvokeObservable.Send(
						new StoreFileEvent(createdEvent));
					break;
				case StoreFileDeletedEvent deletedEvent:
					WrappedInvokeObservable.Send(
						new StoreFileEvent(deletedEvent));
					break;
				case StoreFileUpdatedEvent updatedEvent:
					WrappedInvokeObservable.Send(
						new StoreFileEvent(updatedEvent));
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}
}