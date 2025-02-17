// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncStoreApi.cs
// Last edit: 2025-02-17 19:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Store;
using PrivMX.Endpoint.Store.Models;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivmxEndpointCsharpExtra.Api;

public class AsyncStoreApi : IAsyncDisposable, IDisposable
{
    private DisposeBool _disposed;
	private IStoreApi _storeApi;
	private long _connectionId;

	/// <summary>
	/// Creates a new instance of the store api over real privmx connection.
	/// </summary>
	/// <param name="connection"></param>
	public AsyncStoreApi(Connection connection)
	{
		_storeApi = StoreApi.Create(connection);
		_connectionId = connection.GetConnectionId();
	}

    /// <summary>
    /// Creates a new Store in given Context.
    /// </summary>
    /// <param name="contextId">ID of the Context to create the Store in.</param>
    /// <param name="users">Array of UserWithPubKey structs which indicates who will have access to the created Store.</param>
    /// <param name="managers">Array of UserWithPubKey structs which indicates who will have access (and management rights) to the created Store.</param>
    /// <param name="publicMeta">Public (unencrypted) metadata.</param>
    /// <param name="privateMeta">Private (encrypted) metadata.</param>
    /// <param name="containerPolicy">Store policy.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Created Store ID.</returns>
    /// /// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
    public ValueTask<string> CreateStoreAsync(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy containerPolicy, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.CreateStoreAsync(contextId, users, managers, publicMeta, privateMeta, containerPolicy, token);
    }

    /// <summary>
    /// Updates an existing Store.
    /// </summary>
    /// <param name="storeId">ID of the Store to update.</param>
    /// <param name="users">Array of UserWithPubKey structs which indicates who will have access to the created Store.</param>
    /// <param name="managers">Array of UserWithPubKey structs which indicates who will have access (and management rights) to the created Store.</param>
    /// <param name="publicMeta">Public (unencrypted) metadata.</param>
    /// <param name="privateMeta">Private (encrypted) metadata.</param>
    /// <param name="version">Current version of the updated Store.</param>
    /// <param name="force">Force update (without checking version).</param>
    /// <param name="forceGenerateNewKey">Force to regenerate a key for the Store.</param>
    /// <param name="containerPolicy">(optional) Store policy.</param>
    /// <param name="token">Cancellation token.</param>
    /// /// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
    public ValueTask UpdateStoreAsync(string storeId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey, ContainerPolicy? containerPolicy = null, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.UpdateStoreAsync(storeId, users, managers, publicMeta, privateMeta, version, force, forceGenerateNewKey, containerPolicy, token);
    }

    /// <summary>
    /// Deletes a Store by given Store ID.
    /// </summary>
    /// <param name="storeId">ID of the Store to delete.</param>
    /// <param name="token">Cancellation token.</param>
    /// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
    public ValueTask DeleteStoreAsync(string storeId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.DeleteStoreAsync(storeId, token);
    }

    public ValueTask<Store> GetStoreAsync(string storeId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.GetStoreAsync(storeId, token);
    }

    public ValueTask<PagingList<Store>> ListStoresAsync(string contextId, PagingQuery pagingQuery, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.ListStoresAsync(contextId, pagingQuery, token);
    }

    public ValueTask<long> CreateFileAsync(string storeId, byte[] publicMeta, byte[] privateMeta, long size, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.CreateFileAsync(storeId, publicMeta, privateMeta, size, token);
    }

    public ValueTask<long> UpdateFileAsync(string fileId, byte[] publicMeta, byte[] privateMeta, long size, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.UpdateFileAsync(fileId, publicMeta, privateMeta, size, token);
    }

    public ValueTask UpdateFileMetaAsync(string fileId, byte[] publicMeta, byte[] privateMeta, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.UpdateFileMetaAsync(fileId, publicMeta, privateMeta, token);
    }

    public ValueTask WriteToFileAsync(long fileHandle, byte[] dataChunk, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.WriteToFileAsync(fileHandle, dataChunk, token);
    }

    public ValueTask DeleteFileAsync(string storeId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.DeleteFileAsync(storeId, token);
    }

    public ValueTask<File> GetFileAsync(string fileId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.GetFileAsync(fileId, token);
    }

    public ValueTask<PagingList<File>> ListFilesAsync(string storeId, PagingQuery pagingQuery, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.ListFilesAsync(storeId, pagingQuery, token);
    }

    public ValueTask<long> OpenFileAsync(string fileId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.OpenFileAsync(fileId, token);
    }

    public ValueTask<byte[]> ReadFromFileAsync(long fileHandle, long length, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.ReadFromFileAsync(fileHandle, length, token);
    }

    public ValueTask SeekInFileAsync(long fileHandle, long position, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.SeekInFileAsync(fileHandle, position, token);
    }

    public ValueTask<string> CloseFileAsync(long fileHandle, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.CloseFileAsync(fileHandle, token);
    }

    public ValueTask SubscribeForStoreEventsAsync(CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.SubscribeForStoreEventsAsync(token);
    }

    public ValueTask UnsubscribeFromStoreEventsAsync(CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.UnsubscribeFromStoreEventsAsync(token);
    }

    public ValueTask SubscribeForFileEventsAsync(string storeId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.SubscribeForFileEventsAsync(storeId, token);
    }

    public ValueTask UnsubscribeFromFileEventsAsync(string storeId, CancellationToken token = default)
    {
        _disposed.ThrowIfDisposed(nameof(AsyncStoreApi));
        return _storeApi.UnsubscribeFromFileEventsAsync(storeId, token);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }

    public void Dispose()
    {
        _disposed.PerformDispose();
        _storeApi = null!;
    }
}