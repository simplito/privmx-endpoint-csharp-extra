// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncStoreApi.cs
// Last edit: 2025-02-23 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra.Abstractions;
using PrivmxEndpointCsharpExtra.Events;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivmxEndpointCsharpExtra.Api.Interfaces;

public interface IAsyncStoreApi
{
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
	ValueTask<string> CreateStore(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, ContainerPolicy containerPolicy, CancellationToken token = default);

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
	ValueTask UpdateStore(string storeId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? containerPolicy = null, CancellationToken token = default);

	/// <summary>
	///     Deletes a Store by given Store ID.
	/// </summary>
	/// <param name="storeId">ID of the Store to delete.</param>
	/// <param name="token">Cancellation token.</param>
	/// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
	ValueTask DeleteStore(string storeId, CancellationToken token = default);

	/// <summary>
	/// Gets a single Store by given Store ID.
	/// </summary>
	/// <param name="storeId">ID of the store to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Information about about the Store.</returns>
	ValueTask<PrivMX.Endpoint.Store.Models.Store> GetStore(string storeId, CancellationToken token = default);

	/// <summary>
	/// Gets information about existing file.
	/// </summary>
	/// <param name="fileId">ID of the file to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Store file metadata.</returns>
	ValueTask<File> GetFile(string fileId, CancellationToken token = default);

	/// <summary>
	/// Gets a list of Stores in given Context.
	/// </summary>
	/// <param name="contextId">ID of the Context to get the Stores from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of Stores.</returns>
	ValueTask<PagingList<PrivMX.Endpoint.Store.Models.Store>> ListStores(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default);

	ValueTask<PagingList<File>> ListFiles(string storeId, PagingQuery pagingQuery,
		CancellationToken token = default);

	/// <summary>
	/// Creates a new file in a Store.
	/// </summary>
	/// <param name="storeId">ID of the Store to create the file in.</param>
	/// <param name="publicMeta">Public file meta_data.</param>
	/// <param name="privateMeta">Private file meta_data.</param>
	/// <param name="size">Size of the file.</param>
	/// <param name="fillValue">Optional value to fill empty space in file stream on close.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Fixed size file stream that supports write operations.</returns>
	ValueTask<PrivmxFileStream> CreateFile(string storeId, long size, byte[] publicMeta, byte[] privateMeta, byte? fillValue = null, CancellationToken token = default);

	/// <summary>
	/// Opens a file for write.
	/// </summary>
	/// <param name="fileId">ID of the file to update.</param>
	/// <param name="size">New file size.</param>
	/// <param name="publicMeta">Public file meta data.></param>
	/// <param name="privateMeta">Private file meta data.</param>
	/// <param name="fillValue">Optional value to fill empty space in file stream on close.</param>
	/// <param name="token">Cancellation token</param>
	/// <returns>Fixed size file stream that supports write operations.</returns>
	ValueTask<PrivmxFileStream> OpenFileForWrite(string fileId, long size, byte[] publicMeta, byte[] privateMeta, byte? fillValue = null,
		CancellationToken token = default);

	/// <summary>
	/// Opens a file for read.
	/// </summary>
	/// <param name="fileId">ID of the file to read.</param>
	/// <param name="token">Cancellation token</param>
	/// <returns>Fixed size readable stream that supports seek operation.</returns>
	ValueTask<PrivmxFileStream> OpenFileForRead(string fileId, CancellationToken token = default);

	/// <summary>
	/// Updates meta data of an existing file in a Store.
	/// </summary>
	/// <param name="fileId">ID of the file to update.</param>
	/// <param name="publicMeta">Public file meta_data.</param>
	/// <param name="privateMeta">Private file meta_data.</param>
	/// <param name="token">Cancellation token.</param>
	ValueTask UpdateFileMetaAsync(string fileId, byte[] publicMeta, byte[] privateMeta,
		CancellationToken token = default);

	/// <summary>
	/// Gets store events.
	/// </summary>
	/// <returns>Stream of store events.</returns>
	IObservable<StoreEvent> GetStoreEvents();

	/// <summary>
	/// Gets store file events.
	/// </summary>
	/// <param name="storeId">ID of the tracked store.</param>
	/// <returns>Stream of store file events.</returns>
	IObservable<StoreFileEvent> GetFileEvents(string storeId);
}