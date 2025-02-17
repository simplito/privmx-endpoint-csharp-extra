// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncThreadApi.cs
// Last edit: 2025-02-17 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Thread.Models;
using PrivmxEndpointCsharpExtra.Events;
using Thread = PrivMX.Endpoint.Thread.Models.Thread;

namespace PrivmxEndpointCsharpExtra.Api;

public interface IAsyncThreadApi
{
	ValueTask<string> CreateThreadAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy? containerPolicy = null,
		CancellationToken token = default);

	ValueTask UpdateThreadAsync(string threadId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? containerPolicy = null, CancellationToken token = default);

	ValueTask DeleteThreadAsync(string threadId, CancellationToken token = default);
	ValueTask<Thread> GetThreadAsync(string threadId, CancellationToken token = default);

	ValueTask<PagingList<Thread>> ListThreadsAsync(string contextId, PagingQuery pagingQuery,
		CancellationToken token = default);

	ValueTask<Message> GetMessageAsync(string messageId, CancellationToken token = default);

	ValueTask<PagingList<Message>> ListMessagesAsync(string threadId, PagingQuery pagingQuery,
		CancellationToken token = default);

	ValueTask<string> SendMessageAsync(string threadId, byte[] publicMeta, byte[] privateMeta, byte[] data,
		CancellationToken token = default);

	ValueTask UpdateMessageAsync(string messageId, byte[] publicMeta, byte[] privateMeta, byte[] data,
		CancellationToken token = default);

	ValueTask DeleteMessageAsync(string messageId, CancellationToken token = default);
	IObservable<ThreadEvent> GetThreadEvents();
	IObservable<ThreadMessageEvent> GetThreadMessageEvents(string threadId);
}