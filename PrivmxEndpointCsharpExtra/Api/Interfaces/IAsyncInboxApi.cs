// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncInboxApi.cs
// Last edit: 2025-02-23 19:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Inbox.Models;
using PrivmxEndpointCsharpExtra.Abstractions;
using PrivmxEndpointCsharpExtra.Events;
using PrivmxEndpointCsharpExtra.Inbox;

namespace PrivmxEndpointCsharpExtra.Api.Interfaces;

public interface IAsyncInboxApi
{
	ValueTask<string> CreateInboxAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig,
		ContainerPolicyWithoutItem? policies = null, CancellationToken token = default);

	ValueTask UpdateInboxAsync(string inboxId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig, long version, bool force,
		bool forceGenerateNewKey, ContainerPolicyWithoutItem? policies = null, CancellationToken token = default);

	ValueTask<PrivMX.Endpoint.Inbox.Models.Inbox> GetInboxAsync(string inboxId,
		CancellationToken token = default);

	ValueTask<PagingList<PrivMX.Endpoint.Inbox.Models.Inbox>> ListInboxesAsync(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default);

	ValueTask<InboxPublicView> GetInboxPublicViewAsync(string inboxId, CancellationToken token = default);
	ValueTask DeleteInboxAsync(string inboxId, CancellationToken token = default);
	InboxEntryWriterBuilder GetEntryBuilder(string inboxId);
	ValueTask<InboxEntry> ReadEntryAsync(string inboxEntryId, CancellationToken token = default);

	ValueTask<PagingList<InboxEntry>> ListEntriesAsync(string inboxId, PagingQuery pagingQuery,
		CancellationToken token = default);

	ValueTask DeleteEntryAsync(string inboxEntryId, CancellationToken token = default);
	ValueTask<PrivmxFileStream> OpenFileForWrite(InboxEntry entry, string fileId, CancellationToken token = default);
	IObservable<InboxEvent> GetInboxEvents();
	IObservable<InboxEntryEvent> GetEntryEvents(string inboxId);
}