// Module name: PrivmxEndpointCsharpExtra
// File name: InboxApiExtensions.cs
// Last edit: 2025-02-22 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Inbox;
using PrivMX.Endpoint.Inbox.Models;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra;

public static class InboxApiExtensions
{
	public static ValueTask<PrivMX.Endpoint.Inbox.Models.Inbox> GetInboxAsync(this IInboxApi inboxApi, string inboxId,
		CancellationToken token = default)
	{
		if (inboxApi is null)
			throw new ArgumentNullException(nameof(inboxApi));
		return WrapperCallsExecutor.Execute(
			() => inboxApi.GetInbox(inboxId), token);
	}

     public static ValueTask<string> CreateInboxAsync(this IInboxApi inboxApi, string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig, ContainerPolicyWithoutItem? policies = null, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.CreateInbox(contextId, users, managers, publicMeta, privateMeta, filesConfig, policies), token);
        }

        public static ValueTask UpdateInboxAsync(this IInboxApi inboxApi, string inboxId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig, long version, bool force, bool forceGenerateNewKey, ContainerPolicyWithoutItem? policies = null, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.UpdateInbox(inboxId, users, managers, publicMeta, privateMeta, filesConfig, version, force, forceGenerateNewKey, policies), token);
        }

        public static ValueTask<PagingList<PrivMX.Endpoint.Inbox.Models.Inbox>> ListInboxesAsync(this IInboxApi inboxApi, string contextId, PagingQuery pagingQuery, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.ListInboxes(contextId, pagingQuery), token);
        }

        public static ValueTask<InboxPublicView> GetInboxPublicViewAsync(this IInboxApi inboxApi, string inboxId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.GetInboxPublicView(inboxId), token);
        }

        public static ValueTask DeleteInboxAsync(this IInboxApi inboxApi, string inboxId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.DeleteInbox(inboxId), token);
        }

        public static ValueTask<long> PrepareEntryAsync(this IInboxApi inboxApi, string inboxId, byte[] data, List<long> inboxFileHandles, string userPrivKey, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.PrepareEntry(inboxId, data, inboxFileHandles, userPrivKey), token);
        }

        public static ValueTask SendEntryAsync(this IInboxApi inboxApi, long inboxHandle, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.SendEntry(inboxHandle), token);
        }

        public static ValueTask<InboxEntry> ReadEntryAsync(this IInboxApi inboxApi, string inboxEntryId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.ReadEntry(inboxEntryId), token);
        }

        public static ValueTask<PagingList<InboxEntry>> ListEntriesAsync(this IInboxApi inboxApi, string inboxId, PagingQuery pagingQuery, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.ListEntries(inboxId, pagingQuery), token);
        }

        public static ValueTask DeleteEntryAsync(this IInboxApi inboxApi, string inboxEntryId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.DeleteEntry(inboxEntryId), token);
        }

        public static ValueTask<long> CreateFileHandleAsync(this IInboxApi inboxApi, byte[] publicMeta, byte[] privateMeta, long fileSize, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.CreateFileHandle(publicMeta, privateMeta, fileSize), token);
        }

        public static ValueTask WriteToFileAsync(this IInboxApi inboxApi, long inboxHandle, long inboxFileHandle, byte[] dataChunk, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.WriteToFile(inboxHandle, inboxFileHandle, dataChunk), token);
        }

        public static ValueTask<long> OpenFileAsync(this IInboxApi inboxApi, string fileId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.OpenFile(fileId), token);
        }

        public static ValueTask<byte[]> ReadFromFileAsync(this IInboxApi inboxApi, long fileHandle, long length, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.ReadFromFile(fileHandle, length), token);
        }

        public static ValueTask SeekInFileAsync(this IInboxApi inboxApi, long fileHandle, long position, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.SeekInFile(fileHandle, position), token);
        }

        public static ValueTask<string> CloseFileAsync(this IInboxApi inboxApi, long fileHandle, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.CloseFile(fileHandle), token);
        }

        public static ValueTask SubscribeForInboxEventsAsync(this IInboxApi inboxApi, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(inboxApi.SubscribeForInboxEvents, token);
        }

        public static ValueTask UnsubscribeFromInboxEventsAsync(this IInboxApi inboxApi, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(inboxApi.UnsubscribeFromInboxEvents, token);
        }

        public static ValueTask SubscribeForEntryEventsAsync(this IInboxApi inboxApi, string inboxId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.SubscribeForEntryEvents(inboxId), token);
        }

        public static ValueTask UnsubscribeFromEntryEventsAsync(this IInboxApi inboxApi, string inboxId, CancellationToken token = default)
        {
            if (inboxApi is null)
                throw new ArgumentNullException(nameof(inboxApi));
            return WrapperCallsExecutor.Execute(() => inboxApi.UnsubscribeFromEntryEvents(inboxId), token);
        }
}