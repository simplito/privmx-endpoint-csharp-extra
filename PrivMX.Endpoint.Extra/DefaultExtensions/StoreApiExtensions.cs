// Module name: PrivmxEndpointCsharpExtra
// File name: StoreApiExtensions.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Store;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivMX.Endpoint.Extra;

/// <summary>
///     Asynchronous extensions for dtore API.
/// </summary>
public static class StoreApiExtensions
{
	/// <summary>
	///     Creates a new Store in given Context.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="contextId">ID of the Context to create the Store in.</param>
	/// <param name="users">Array of UserWithPubKey structs which indicates who will have access to the created Store.</param>
	/// <param name="managers">
	///     Array of UserWithPubKey structs which indicates who will have access (and management rights) to
	///     the created Store.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) metadata.</param>
	/// <param name="privateMeta">Private (encrypted) metadata.</param>
	/// <param name="policies">(optional) Store policy.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Created Store ID.</returns>
	public static ValueTask<string> CreateStoreAsync(this IStoreApi api, string contextId,
		List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
		ContainerPolicy policies,
		CancellationToken token = default)
	{
		if (api == null)
			throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(
			() => api.CreateStore(contextId, users, managers, publicMeta, privateMeta, policies), token);
	}

	/// <summary>
	///     Updates an existing Store.
	/// </summary>
	/// <param name="api">Extended object.</param>
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
	/// <param name="forceGenerateNewKey">Force to renenerate a key for the Store.</param>
	/// <param name="policies">(optional) Store policy.</param>
	/// <param name="token">Cancellation token.</param>
	public static ValueTask UpdateStoreAsync(this IStoreApi api, string storeId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, long version, bool force,
		bool forceGenerateNewKey, ContainerPolicy? policies = null, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(
			() => api.UpdateStore(storeId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, policies), token);
	}

	/// <summary>
	///     Deletes a Store by given Store ID.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="storeId">ID of the Store to delete.</param>
	/// <param name="token">Cancellation token.</param>
	public static ValueTask DeleteStoreAsync(this IStoreApi api, string storeId, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.DeleteStore(storeId), token);
	}

	/// <summary>
	///     Gets a single Store by given Store ID.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="storeId">ID of the Store to get.</param>
	/// <param name="token">Cancellation token</param>
	/// <returns>Information about about the Store.</returns>
	public static ValueTask<PrivMX.Endpoint.Store.Models.Store> GetStoreAsync(this IStoreApi api, string storeId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.GetStore(storeId), token);
	}

	/// <summary>
	///     Gets a list of Stores in given Context.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="contextId">ID of the Context to get the Stores from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of Stores.</returns>
	public static ValueTask<PagingList<PrivMX.Endpoint.Store.Models.Store>> ListStoresAsync(this IStoreApi api,
		string contextId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ListStores(contextId, pagingQuery), token);
	}

	/// <summary>
	///     Creates a new file in a Store.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="storeId">ID of the Store to create the file in.</param>
	/// <param name="publicMeta">Public file meta_data.</param>
	/// <param name="privateMeta">Private file meta_data.</param>
	/// <param name="size">Size of the file.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Handle to write data.</returns>
	public static ValueTask<long> CreateFileAsync(this IStoreApi api, string storeId, byte[] publicMeta,
		byte[] privateMeta, long size, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.CreateFile(storeId, publicMeta, privateMeta, size), token);
	}

	/// <summary>
	///     Updates an existing file in a Store.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileId">ID of the file to update.</param>
	/// <param name="publicMeta">Public file meta_data.</param>
	/// <param name="privateMeta">Private file meta_data.</param>
	/// <param name="size">Size of the file.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Handle to write file data.</returns>
	public static ValueTask<long> UpdateFileAsync(this IStoreApi api, string fileId, byte[] publicMeta,
		byte[] privateMeta, long size, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UpdateFile(fileId, publicMeta, privateMeta, size), token);
	}

	/// <summary>
	///     Updates meta data of an existing file in a Store.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileId">ID of the file to update.</param>
	/// <param name="publicMeta">Public file meta_data.</param>
	/// <param name="privateMeta">Private file meta_data.</param>
	/// <param name="token">Cancellation token</param>
	public static ValueTask UpdateFileMetaAsync(this IStoreApi api, string fileId, byte[] publicMeta,
		byte[] privateMeta, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UpdateFileMeta(fileId, publicMeta, privateMeta), token);
	}

	/// <summary>
	///     Writes a file data.
	/// </summary>
	/// <param name="api"></param>
	/// <param name="fileHandle">Handle to write file data.</param>
	/// <param name="dataChunk">File data chunk.</param>
	/// <param name="token"></param>
	public static ValueTask WriteToFileAsync(this IStoreApi api, long fileHandle, byte[] dataChunk,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.WriteToFile(fileHandle, dataChunk), token);
	}

	/// <summary>
	///     Deletes a file by given ID.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileId">ID of the file to delete.</param>
	/// <param name="token">Cancellation token.</param>
	public static ValueTask DeleteFileAsync(this IStoreApi api, string fileId, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.DeleteFile(fileId), token);
	}

	/// <summary>
	///     Deletes a file by given ID.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileId">ID of the file to delete.</param>
	/// <param name="token">Cancellation token.</param>
	public static ValueTask<File> GetFileAsync(this IStoreApi api, string fileId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.GetFile(fileId), token);
	}

	/// <summary>
	///     Gets a list of files in given Store.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="storeId">ID of the Store to get files from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of files.</returns>
	public static ValueTask<PagingList<File>> ListFilesAsync(this IStoreApi api, string storeId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ListFiles(storeId, pagingQuery), token);
	}

	/// <summary>
	///     Opens a file to read.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileId">ID of the file to read.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Handle to read file data.</returns>
	public static ValueTask<long> OpenFileAsync(this IStoreApi api, string fileId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.OpenFile(fileId), token);
	}

	/// <summary>
	///     Reads file data.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileHandle">Handle to write file data.</param>
	/// <param name="length">Size of data to read.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>File data chunk.</returns>
	public static ValueTask<byte[]> ReadFromFileAsync(this IStoreApi api, long fileHandle, long length,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ReadFromFile(fileHandle, length), token);
	}

	/// <summary>
	///     Moves read cursor.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileHandle">Handle to write file data.</param>
	/// <param name="position">New cursor position.</param>
	/// <param name="token">Cancellation token</param>
	public static ValueTask SeekInFileAsync(this IStoreApi api, long fileHandle, long position,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SeekInFile(fileHandle, position), token);
	}

	/// <summary>
	///     Closes the file handle.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="fileHandle">Handle to read/write file data.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>ID of closed file.</returns>
	public static ValueTask<string> CloseFileAsync(this IStoreApi api, long fileHandle,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.CloseFile(fileHandle), token);
	}

	/// <summary>
	///     Subscribes for the Store module main events.
	/// </summary>
	public static ValueTask SubscribeForStoreEventsAsync(this IStoreApi api, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(api.SubscribeForStoreEvents, token);
	}

	/// <summary>
	///     Unsubscribes from the Store module main events.
	/// </summary>
	public static ValueTask UnsubscribeFromStoreEventsAsync(this IStoreApi api, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(api.UnsubscribeFromStoreEvents, token);
	}

	/// <summary>
	///     Subscribes for the events in given Store.
	/// </summary>
	/// <param name="api">Extended object.</param>
	/// <param name="storeId">ID of the store to subscribe to.</param>
	/// <param name="token">Cancellation token.</param>
	public static ValueTask SubscribeForFileEventsAsync(this IStoreApi api, string storeId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SubscribeForFileEvents(storeId), token);
	}

	/// <summary>
	///     Unsubscribes from the events in given Store.
	/// </summary>
	/// <param name="api">Extended object</param>
	/// <param name="storeId">ID of the store to unsubscribe from.</param>
	/// <param name="token">Cancellation token.</param>
	public static ValueTask UnsubscribeFromFileEventsAsync(this IStoreApi api, string storeId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UnsubscribeFromFileEvents(storeId), token);
	}
}