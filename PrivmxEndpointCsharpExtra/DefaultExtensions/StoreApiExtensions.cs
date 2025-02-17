// Module name: PrivmxEndpointCsharpExtra
// File name: StoreApiExtensions.cs
// Last edit: 2025-02-17 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Store;
using PrivMX.Endpoint.Store.Models;
using PrivmxEndpointCsharpExtra.Internals;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivmxEndpointCsharpExtra;

public static class StoreApiExtensions
{
	/// <inheritdoc cref="StoreApi.CreateStore" />
	/// <param name="token">cancellation token</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">Thrown when api is null.</exception>
	public static ValueTask<string> CreateStoreAsync(this IStoreApi api, string contextId,
		List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
		ContainerPolicy containerPolicy,
		CancellationToken token = default)
	{
		if (api == null)
			throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(
			() => api.CreateStore(contextId, users, managers, publicMeta, privateMeta, containerPolicy), token);
	}

	/// <inheritdoc cref="StoreApi.UpdateStore" />
	/// <param name="token">cancellation token</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">Thrown when api is null.</exception>
	public static ValueTask UpdateStoreAsync(this IStoreApi api, string storeId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, long version, bool force,
		bool forceGenerateNewKey, ContainerPolicy containerPolicy = null, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(
			() => api.UpdateStore(storeId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, containerPolicy), token);
	}

	public static ValueTask DeleteStoreAsync(this IStoreApi api, string storeId, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.DeleteStore(storeId), token);
	}

	public static ValueTask<Store> GetStoreAsync(this IStoreApi api, string storeId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.GetStore(storeId), token);
	}

	public static ValueTask<PagingList<Store>> ListStoresAsync(this IStoreApi api, string contextId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ListStores(contextId, pagingQuery), token);
	}

	public static ValueTask<long> CreateFileAsync(this IStoreApi api, string storeId, byte[] publicMeta,
		byte[] privateMeta, long size, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.CreateFile(storeId, publicMeta, privateMeta, size), token);
	}

	public static ValueTask<long> UpdateFileAsync(this IStoreApi api, string fileId, byte[] publicMeta,
		byte[] privateMeta, long size, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UpdateFile(fileId, publicMeta, privateMeta, size), token);
	}

	public static ValueTask UpdateFileMetaAsync(this IStoreApi api, string fileId, byte[] publicMeta,
		byte[] privateMeta, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UpdateFileMeta(fileId, publicMeta, privateMeta), token);
	}

	public static ValueTask WriteToFileAsync(this IStoreApi api, long fileHandle, byte[] dataChunk,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.WriteToFile(fileHandle, dataChunk), token);
	}

	public static ValueTask DeleteFileAsync(this IStoreApi api, string storeId, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.DeleteFile(storeId), token);
	}

	public static ValueTask<File> GetFileAsync(this IStoreApi api, string fileId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.GetFile(fileId), token);
	}

	public static ValueTask<PagingList<File>> ListFilesAsync(this IStoreApi api, string storeId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ListFiles(storeId, pagingQuery), token);
	}

	public static ValueTask<long> OpenFileAsync(this IStoreApi api, string fileId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.OpenFile(fileId), token);
	}

	public static ValueTask<byte[]> ReadFromFileAsync(this IStoreApi api, long fileHandle, long length,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ReadFromFile(fileHandle, length), token);
	}

	public static ValueTask SeekInFileAsync(this IStoreApi api, long fileHandle, long position,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SeekInFile(fileHandle, position), token);
	}

	public static ValueTask<string> CloseFileAsync(this IStoreApi api, long fileHandle,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.CloseFile(fileHandle), token);
	}

	public static ValueTask SubscribeForStoreEventsAsync(this IStoreApi api, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(api.SubscribeForStoreEvents, token);
	}

	public static ValueTask UnsubscribeFromStoreEventsAsync(this IStoreApi api, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(api.UnsubscribeFromStoreEvents, token);
	}

	public static ValueTask SubscribeForFileEventsAsync(this IStoreApi api, string storeId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SubscribeForFileEvents(storeId), token);
	}

	public static ValueTask UnsubscribeFromFileEventsAsync(this IStoreApi api, string storeId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UnsubscribeFromFileEvents(storeId), token);
	}
}