// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncThreadApi.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.ComponentModel;
using Internal;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Thread;
using PrivMX.Endpoint.Thread.Models;
using PrivmxEndpointCsharpExtra.Events;
using PrivmxEndpointCsharpExtra.Events.Internal;
using PrivmxEndpointCsharpExtra.Internals;
using Thread = PrivMX.Endpoint.Thread.Models.Thread;

namespace PrivmxEndpointCsharpExtra.Api;

public sealed class AsyncThreadApi : IAsyncDisposable
{
	private readonly IThreadApi _threadApi;
	private readonly ThreadChannelEventDispatcher _threadChannelEventDispatcher;
	private readonly Dictionary<string, ThreadMessageChannelEventDispatcher> _threadMessageDispatchers;
	private DisposeBool _disposed;


	public AsyncThreadApi(IThreadApi threadApi)
	{
		_threadApi = threadApi;
		_threadChannelEventDispatcher =
			new ThreadChannelEventDispatcher(_threadApi, PrivMXEventDispatcher.GetDispatcher());
		_threadMessageDispatchers = new Dictionary<string, ThreadMessageChannelEventDispatcher>();
	}

	/// <summary>
	///     Wraps existing thread api into async thread api.
	/// </summary>
	/// <param name="threadApi">Existing thread API.</param>
	/// <param name="eventDispatcher"></param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncThreadApi(IThreadApi threadApi, IEventDispatcher eventDispatcher)
	{
		_threadApi = threadApi;
		_threadChannelEventDispatcher = new ThreadChannelEventDispatcher(_threadApi, eventDispatcher);
		_threadMessageDispatchers = new Dictionary<string, ThreadMessageChannelEventDispatcher>();
	}

	public ValueTask DisposeAsync()
	{
		if (_disposed.PerformDispose())
		{
			_threadChannelEventDispatcher.Dispose();
			foreach (var disp in _threadMessageDispatchers.Values) disp.Dispose();
		}

		return default;
	}


	public ValueTask<string> CreateThreadAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy? containerPolicy = null,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(
			() => _threadApi.CreateThread(contextId, users, managers, publicMeta, privateMeta, containerPolicy), token);
	}

	public ValueTask UpdateThreadAsync(string threadId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? containerPolicy = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(
			() => _threadApi.UpdateThread(threadId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, containerPolicy), token);
	}

	public ValueTask DeleteThreadAsync(string threadId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.DeleteThread(threadId), token);
	}

	public ValueTask<Thread> GetThreadAsync(string threadId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.GetThread(threadId), token);
	}

	public ValueTask<PagingList<Thread>> ListThreadsAsync(string contextId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.ListThreads(contextId, pagingQuery), token);
	}

	public ValueTask<Message> GetMessageAsync(string messageId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.GetMessage(messageId), token);
	}

	public ValueTask<PagingList<Message>> ListMessagesAsync(string threadId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.ListMessages(threadId, pagingQuery), token);
	}

	public ValueTask<string> SendMessageAsync(string threadId, byte[] publicMeta, byte[] privateMeta, byte[] data,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.SendMessage(threadId, publicMeta, privateMeta, data),
			token);
	}

	public ValueTask UpdateMessageAsync(string messageId, byte[] publicMeta, byte[] privateMeta, byte[] data,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.UpdateMessage(messageId, publicMeta, privateMeta, data),
			token);
	}

	public ValueTask DeleteMessageAsync(string messageId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.DeleteMessage(messageId), token);
	}

	public IObservable<ThreadEvent> GetThreadEvents()
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return _threadChannelEventDispatcher;
	}

	public IObservable<ThreadMessageEvent> GetThreadMessageEvents(string threadId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		lock (_threadChannelEventDispatcher)
		{
			if (!_threadMessageDispatchers.TryGetValue(threadId, out var dispatcher))
			{
				dispatcher =
					new ThreadMessageChannelEventDispatcher(threadId, _threadApi,
						PrivMXEventDispatcher.GetDispatcher());
				_threadMessageDispatchers.Add(threadId, dispatcher);
			}

			return dispatcher;
		}
	}

	private class ThreadChannelEventDispatcher(IThreadApi connection, IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<ThreadEvent>("thread", eventDispatcher)
	{
		private IThreadApi Connection { get; } = connection;

		protected override void OpenChanel()
		{
			Connection.SubscribeForThreadEvents();
		}

		protected override void CloseChanel()
		{
			Connection.UnsubscribeFromThreadEvents();
		}

		public override void HandleEvent(Event @event)
		{
			switch (@event)
			{
				case ThreadCreatedEvent createdEvent:
					WrappedInvokeObservable.Send(
						new ThreadEvent(createdEvent));
					break;
				case ThreadDeletedEvent deletedEvent:
					WrappedInvokeObservable.Send(
						new ThreadEvent(deletedEvent));
					break;
				case ThreadUpdatedEvent updatedEvent:
					WrappedInvokeObservable.Send(
						new ThreadEvent(updatedEvent));
					break;
				case ThreadStatsChangedEvent statsChangedEvent:
					WrappedInvokeObservable.Send(
						new ThreadEvent(statsChangedEvent));
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}

	private class ThreadMessageChannelEventDispatcher(
		string threadId,
		IThreadApi connection,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<ThreadMessageEvent>($"thread/{threadId}/messages", eventDispatcher)
	{
		private IThreadApi Connection { get; } = connection;
		private string ThreadId { get; } = threadId;

		protected override void OpenChanel()
		{
			Connection.SubscribeForMessageEvents(ThreadId);
		}

		protected override void CloseChanel()
		{
			Connection.UnsubscribeFromMessageEvents(ThreadId);
		}

		public override void HandleEvent(Event @event)
		{
			switch (@event)
			{
				case ThreadNewMessageEvent createdEvent:
					WrappedInvokeObservable.Send(
						new ThreadMessageEvent(createdEvent));
					break;
				case ThreadMessageDeletedEvent deletedEvent:
					WrappedInvokeObservable.Send(
						new ThreadMessageEvent(deletedEvent));
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}
}