//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Api.Interfaces;
using PrivMX.Endpoint.Extra.Events;
using PrivMX.Endpoint.Extra.Events.Internal;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Thread;
using PrivMX.Endpoint.Thread.Models;
using System.ComponentModel;

namespace PrivMX.Endpoint.Extra.Api;

/// <summary>
///     Asynchronous wrapper over thread API.
/// </summary>
public sealed class AsyncThreadApi : IAsyncDisposable, IDisposable, IAsyncThreadApi
{
	private readonly long _connectionId;
	private readonly IEventDispatcher _eventDispatcher;

	private readonly IThreadApi _threadApi;
	private readonly ThreadChannelEventDispatcher _threadChannelEventDispatcher;
	private readonly Dictionary<string, ThreadMessageChannelEventDispatcher> _threadMessageDispatchers;
	private DisposeBool _disposed;

	/// <summary>
	///     Creates async thread API over real PrivMX connection.
	/// </summary>
	/// <param name="connection">Connection used.</param>
	public AsyncThreadApi(Connection connection) : this(ThreadApi.Create(connection), connection.GetConnectionId(),
		PrivMXEventDispatcher.Instance)
	{
	}

	/// <summary>
	///     Wraps existing thread api into async thread api.
	///     This constructor is meant to be used in advanced scenarios like object mocking and testing.
	/// </summary>
	/// <param name="threadApi">Existing thread API.</param>
	/// <param name="connectionId">ID of user connection.</param>
	/// <param name="eventDispatcher">Event dispatcher used as event source.</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncThreadApi(IThreadApi threadApi, long connectionId, IEventDispatcher eventDispatcher)
	{
		_threadApi = threadApi;
		_eventDispatcher = eventDispatcher;
		_connectionId = connectionId;
		_threadChannelEventDispatcher = new ThreadChannelEventDispatcher(_threadApi, connectionId, eventDispatcher);
		_threadMessageDispatchers = new Dictionary<string, ThreadMessageChannelEventDispatcher>();
	}

	/// <summary>
	///     Disposes async thread API with all related resources.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

	/// <summary>
	///     Creates new Thread in given Context.
	/// </summary>
	/// <param name="contextId">ID of the Context to create the Thread in.</param>
	/// <param name="users">Array of <see cref="UserWithPubKey" /> which indicates who will have access to the created Thread.</param>
	/// <param name="managers">
	///     Array of <see cref="UserWithPubKey" /> which indicates who will have access (and management
	///     rights) to the created Thread.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) meta data.</param>
	/// <param name="privateMeta">Private (encrypted) meta data.</param>
	/// <param name="policies">(optional) Thread policy.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>ID of the created Thread.</returns>
	public ValueTask<string> CreateThreadAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy? policies = null,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(
			() => _threadApi.CreateThread(contextId, users, managers, publicMeta, privateMeta, policies), token);
	}

	/// <summary>
	///     Updates an existing Thread.
	/// </summary>
	/// <param name="threadId">ID of the Thread to update.</param>
	/// <param name="users">
	///     Array of <see cref="UserWithPubKey" /> structs which indicates who will have access to the created
	///     Thread.
	/// </param>
	/// <param name="managers">
	///     Array of <see cref="UserWithPubKey" /> structs which indicates who will have access (and
	///     management rights) to the created Thread.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) meta data.</param>
	/// <param name="privateMeta">Private (encrypted) meta data.</param>
	/// <param name="version">Current version of the updated Thread.</param>
	/// <param name="force">Force update (without checking version).</param>
	/// <param name="forceGenerateNewKey">Force to regenerate a key for the Thread.</param>
	/// <param name="policies">(optional) Thread policy.</param>
	/// <param name="token">Cancellation token.</param>
	public ValueTask UpdateThreadAsync(string threadId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(
			() => _threadApi.UpdateThread(threadId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, policies), token);
	}

	/// <summary>
	///     Deletes a Thread by given Thread ID.
	/// </summary>
	/// <param name="threadId">ID of the Thread to delete.</param>
	/// <param name="token">Cancellation token.</param>
	public ValueTask DeleteThreadAsync(string threadId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.DeleteThread(threadId), token);
	}

	/// <summary>
	///     Gets a Thread by given Thread ID.
	/// </summary>
	/// <param name="threadId">ID of Thread to get.</param>
	/// <param name="token">Cancellation token</param>
	/// <returns>Information about the Thread.</returns>
	public ValueTask<Thread.Models.Thread> GetThreadAsync(string threadId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.GetThread(threadId), token);
	}

	/// <summary>
	///     Gets a list of Threads in given Context.
	/// </summary>
	/// <param name="contextId">ID of the Context to get the Threads from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of Threads.</returns>
	public ValueTask<PagingList<Thread.Models.Thread>> ListThreadsAsync(string contextId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.ListThreads(contextId, pagingQuery), token);
	}

	/// <summary>
	///     Gets a message by given message ID.
	/// </summary>
	/// <param name="messageId">ID of the message to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Message.</returns>
	public ValueTask<Message> GetMessageAsync(string messageId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.GetMessage(messageId), token);
	}

	/// <summary>
	///     Gets a list of messages from a Thread.
	/// </summary>
	/// <param name="threadId">ID of the Thread to list messages from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of messages.</returns>
	public ValueTask<PagingList<Message>> ListMessagesAsync(string threadId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.ListMessages(threadId, pagingQuery), token);
	}

	/// <summary>
	///     Sends a message in a Thread.
	/// </summary>
	/// <param name="threadId">ID of the Thread to send message to.</param>
	/// <param name="publicMeta">Public message metadata.</param>
	/// <param name="privateMeta">Private message metadata.</param>
	/// <param name="data">Content of the message.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>ID of the new message.</returns>
	public ValueTask<string> SendMessageAsync(string threadId, byte[] publicMeta, byte[] privateMeta, byte[] data,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.SendMessage(threadId, publicMeta, privateMeta, data),
			token);
	}

	/// <summary>
	///     Updates a message in a Thread.
	/// </summary>
	/// <param name="messageId">ID of the message to update.</param>
	/// <param name="publicMeta">Public message metadata.</param>
	/// <param name="privateMeta">Private message metadata.</param>
	/// <param name="data">Content of the message.</param>
	/// <param name="token">Cancellation token.</param>
	public ValueTask UpdateMessageAsync(string messageId, byte[] publicMeta, byte[] privateMeta, byte[] data,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.UpdateMessage(messageId, publicMeta, privateMeta, data),
			token);
	}

	/// <summary>
	///     Deletes a message by given message ID.
	/// </summary>
	/// <param name="messageId">ID of the message to delete.</param>
	/// <param name="token">Cancellation token.</param>
	public ValueTask DeleteMessageAsync(string messageId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return WrapperCallsExecutor.Execute(() => _threadApi.DeleteMessage(messageId), token);
	}

	/// <summary>
	///     Stream of threads related events.
	/// </summary>
	/// <returns>Observable stream of events.</returns>
	public IObservable<ThreadEvent> GetThreadEvents()
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		return _threadChannelEventDispatcher;
	}

	/// <summary>
	///     Stream of events related to a particular thread.
	/// </summary>
	/// <param name="threadId">ID of the thread.</param>
	/// <returns>Observable stream of events.</returns>
	public IObservable<ThreadMessageEvent> GetThreadMessageEvents(string threadId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
		lock (_threadMessageDispatchers)
		{
			if (!_threadMessageDispatchers.TryGetValue(threadId, out var dispatcher))
			{
				dispatcher =
					new ThreadMessageChannelEventDispatcher(threadId, _threadApi, _connectionId,
						_eventDispatcher);
				_threadMessageDispatchers.Add(threadId, dispatcher);
			}

			return dispatcher;
		}
	}

	/// <summary>
	///     Dispose async thread API with all related resources.
	/// </summary>
	public void Dispose()
	{
		if (_disposed.PerformDispose())
		{
			_threadChannelEventDispatcher.Dispose();
			var exceptions = _threadMessageDispatchers.Values.ForEachNotThrowing(obj => obj.Dispose());
			if (exceptions is not null)
				throw new AggregateException(exceptions);
		}
	}

	private class ThreadChannelEventDispatcher(
		IThreadApi connection,
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<ThreadEvent>("thread", connectionId, eventDispatcher)
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
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<ThreadMessageEvent>($"thread/{threadId}/messages", connectionId, eventDispatcher)
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