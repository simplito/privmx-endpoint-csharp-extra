﻿//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Thread;
using PrivMX.Endpoint.Thread.Models;

namespace PrivMX.Endpoint.Extra;

public static class ThreadApiAsyncExtensions
{
	public static ValueTask<string> CreateThreadAsync(this IThreadApi threadApi, string contextId,
		List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
		ContainerPolicy containerPolicy,
		CancellationToken token = default)
	{
		if (threadApi is null) throw new ArgumentNullException(nameof(threadApi));
		return WrapperCallsExecutor.Execute(
			() => threadApi.CreateThread(contextId, users, managers, publicMeta, privateMeta, containerPolicy), token);
	}

	public static ValueTask UpdateThreadAsync(this IThreadApi threadApi, string threadId,
		List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
		long version, bool force, bool forceGenerateNewKey, ContainerPolicy? containerPolicy = null,
		CancellationToken token = default)
	{
		if (threadApi is null) throw new ArgumentNullException(nameof(threadApi));
		return WrapperCallsExecutor.Execute(
			() => threadApi.UpdateThread(threadId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, containerPolicy), token);
	}

	public static ValueTask DeleteThreadAsync(this IThreadApi threadApi, string threadId,
		CancellationToken token = default)
	{
		if (threadApi is null) throw new ArgumentException(nameof(threadApi));
		return WrapperCallsExecutor.Execute(() => threadApi.DeleteThread(threadId), token);
	}

	public static ValueTask<Thread.Models.Thread> GetThreadAsync(this IThreadApi threadApi,
		string threadId, CancellationToken token = default)
	{
		if (threadApi is null) throw new ArgumentException(nameof(threadApi));
		return WrapperCallsExecutor.Execute(() => threadApi.GetThread(threadId), token);
	}

	public static ValueTask<PagingList<Thread.Models.Thread>> ListThreadsAsync(this IThreadApi api,
		string contextId, PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ListThreads(contextId, pagingQuery), token);
	}

	public static ValueTask<Message> GetMessageAsync(this IThreadApi api, string messageId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.GetMessage(messageId), token);
	}

	public static ValueTask<PagingList<Message>> ListMessagesAsync(this IThreadApi api, string threadId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.ListMessages(threadId, pagingQuery), token);
	}

	public static ValueTask<string> SendMessageAsync(this IThreadApi api, string threadId, byte[] publicMeta,
		byte[] privateMeta, byte[] data, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SendMessage(threadId, publicMeta, privateMeta, data), token);
	}

	public static ValueTask UpdateMessageAsync(this IThreadApi api, string messageId, byte[] publicMeta,
		byte[] privateMeta, byte[] data, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UpdateMessage(messageId, publicMeta, privateMeta, data), token);
	}

	public static ValueTask DeleteMessageAsync(this IThreadApi api, string messageId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.DeleteMessage(messageId), token);
	}

	public static ValueTask SubscribeForThreadEventsAsync(this IThreadApi api, CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(api.SubscribeForThreadEvents, token);
	}

	public static ValueTask UnsubscribeFromThreadEventsAsync(this IThreadApi api,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(api.UnsubscribeFromThreadEvents, token);
	}

	public static ValueTask SubscribeForMessageEventsAsync(this IThreadApi api, string threadId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SubscribeForMessageEvents(threadId), token);
	}

	public static ValueTask UnsubscribeFromMessageEventsAsync(this IThreadApi api, string threadId,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UnsubscribeFromMessageEvents(threadId), token);
	}
}