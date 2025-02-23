// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncStoreApi.cs
// Last edit: 2025-02-19 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Store.Models;
using PrivmxEndpointCsharpExtra.Abstractions;
using PrivmxEndpointCsharpExtra.Events;
using PrivmxEndpointCsharpExtra.Store;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivmxEndpointCsharpExtra.Api.Interfaces;

public interface IAsyncStoreApi
{
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
	ValueTask<string> CreateStore(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy containerPolicy, CancellationToken token = default);

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
	ValueTask UpdateStore(string storeId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey, ContainerPolicy? containerPolicy = null, CancellationToken token = default);

	/// <summary>
	/// Deletes a Store by given Store ID.
	/// </summary>
	/// <param name="storeId">ID of the Store to delete.</param>
	/// <param name="token">Cancellation token.</param>
	/// <exception cref="ObjectDisposedException">Throw when attempt to use object after disposal is made.</exception>
	ValueTask DeleteStore(string storeId, CancellationToken token = default);

	ValueTask<PrivMX.Endpoint.Store.Models.Store> GetStore(string storeId, CancellationToken token = default);
	ValueTask<File> GetFile(string fileId, CancellationToken token = default);
	ValueTask<PagingList<PrivMX.Endpoint.Store.Models.Store>> ListStores(string contextId, PagingQuery pagingQuery, CancellationToken token = default);

	public ValueTask<PagingList<File>> ListFiles(string storeId, PagingQuery pagingQuery,
		CancellationToken token = default);
	ValueTask<PrivmxFileStream> CreateFile(string storeId, long size, byte[] publicMeta, byte[] privateMeta, byte? fillValue = null, CancellationToken token = default);
	ValueTask<PrivmxFileStream> OpenFileForWrite(string fileId, long size, byte[] publicMeta, byte[] privateMeta, byte? fillValue = null, CancellationToken token = default);
	ValueTask<PrivmxFileStream> OpenFileForRead(string fileId, CancellationToken token = default);
	ValueTask UpdateFileMetaAsync(string fileId, byte[] publicMeta, byte[] privateMeta, CancellationToken token = default);
	public IObservable<StoreEvent> GetStoreEvents();
	public IObservable<StoreFileEvent> GetFileEvents(string storeId);
}