// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncStoreApi.cs
// Last edit: 2025-02-19 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Store;
using PrivmxEndpointCsharpExtra.Api.Interfaces;
using PrivmxEndpointCsharpExtra.Store;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivmxEndpointCsharpExtra.Api;

public class AsyncStoreApi : IAsyncDisposable, IDisposable, IAsyncStoreApi
{
	private long _connectionId;
	private DisposeBool _disposed;
	private IStoreApi _storeApi;

	/// <summary>
	///     Creates a new instance of the store api over real privmx connection.
	/// </summary>
	/// <param name="connection"></param>
	public AsyncStoreApi(Connection connection)
	{
		_storeApi = StoreApi.Create(connection);
		_connectionId = connection.GetConnectionId();
	}

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

	public ValueTask<PrivMX.Endpoint.Store.Models.Store> GetStore(string storeId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.GetStoreAsync(storeId, token);
	}

	public ValueTask<File> GetFile(string fileId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.GetFileAsync(fileId, token);
	}

	public ValueTask<PagingList<PrivMX.Endpoint.Store.Models.Store>> ListStores(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.ListStoresAsync(contextId, pagingQuery, token);
	}

	public ValueTask<PagingList<File>> ListFiles(string storeId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.ListFilesAsync(storeId, pagingQuery, token);
	}

	public StoreWriteFileStream CreateFile(string storeId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return new StoreWriteFileStream(storeId, null, [], [], [], _storeApi);
	}

	public async ValueTask<StoreWriteFileStream> OpenFileForWrite(string fileId, IAsyncStoreApi.WriteOpen mode,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		token.ThrowIfCancellationRequested();
		if (mode == IAsyncStoreApi.WriteOpen.Append)
		{
			byte[] buffer;
			await using var source = await OpenFileForRead(fileId, token);
			token.ThrowIfCancellationRequested();
			buffer = new byte[source.Length];
			var bytesRead = await source.ReadAsync(buffer, token);
			if (bytesRead != source.Length) throw new InvalidOperationException("Failed to read the whole file.");
			return new StoreWriteFileStream(null, fileId, buffer, source._publicMeta, source._privateMeta, _storeApi);
		}

		var meta = await _storeApi.GetFileAsync(fileId, token);
		return new StoreWriteFileStream(null, fileId, [], meta.PublicMeta, meta.PrivateMeta, _storeApi);
	}

	public async ValueTask<StoreReadonlyFileStream> OpenFileForRead(string fileId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		var meta = _storeApi.GetFileAsync(fileId, token).AsTask();
		var handle = _storeApi.OpenFileAsync(fileId, token).AsTask();
		await Task.WhenAll(meta, handle);
		return new StoreReadonlyFileStream(meta.Result.Size, handle.Result, _storeApi,
			meta.Result.PublicMeta, meta.Result.PrivateMeta);
	}

	public ValueTask UpdateFileMetaAsync(string fileId, byte[] publicMeta, byte[] privateMeta,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
		return _storeApi.UpdateFileMetaAsync(fileId, publicMeta, privateMeta, token);
	}

	public void Dispose()
	{
		_disposed.PerformDispose();
		_storeApi = null!;
	}
}