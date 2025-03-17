// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncInboxApi.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.ComponentModel;
using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Inbox;
using PrivMX.Endpoint.Inbox.Models;
using PrivMX.Endpoint.Store;
using PrivMX.Endpoint.Thread;
using PrivmxEndpointCsharpExtra.Abstractions;
using PrivmxEndpointCsharpExtra.Api.Interfaces;
using PrivmxEndpointCsharpExtra.Events;
using PrivmxEndpointCsharpExtra.Events.Internal;
using PrivmxEndpointCsharpExtra.Inbox;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Api;

/// <summary>
///     Inbox API wrapper that provides asynchronous methods and manages resources.
/// </summary>
public class AsyncInboxApi : IAsyncInboxApi, IAsyncDisposable, IDisposable
{
	private readonly long _connectionId;
	private readonly IEventDispatcher _eventDispatcher;
	private DisposeBool _disposed;
	private Dictionary<string, InboxEntryEventDispatcher> _entryEventDispatchers;
	private IInboxApi _inboxApi;
	private InboxChannelEnventDispatcher _inboxChannelEventDispatcher;

	/// <summary>
	///     Wraps existing connection into async connection.
	///     It's user responsibility to provide valid (connected) connection.
	/// </summary>
	/// <param name="connection">Connection to wrap</param>
	public AsyncInboxApi(Connection connection) : this(
		PrivMX.Endpoint.Inbox.InboxApi.Create(connection, ThreadApi.Create(connection), StoreApi.Create(connection)),
		connection.GetConnectionId(), PrivMXEventDispatcher.Instance)
	{
	}

	/// <summary>
	///     Wraps existing inbox api.
	///     It's recommended to use this constructor in tests or custom scenarios.
	/// </summary>
	/// <param name="inboxApi">Wrapped inbox api</param>
	/// <param name="connectionId">Connection ID</param>
	/// <param name="eventDispatcher">Event dispatcher used in event streams</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncInboxApi(IInboxApi inboxApi, long connectionId, IEventDispatcher eventDispatcher)
	{
		_inboxApi = inboxApi;
		_connectionId = connectionId;
		_eventDispatcher = eventDispatcher;
		_entryEventDispatchers = new Dictionary<string, InboxEntryEventDispatcher>();
		_inboxChannelEventDispatcher = new InboxChannelEnventDispatcher(_inboxApi, _connectionId, eventDispatcher);
	}

	internal IInboxApi InboxApi => _disposed ? throw new ObjectDisposedException(nameof(AsyncInboxApi)) : _inboxApi;

	/// <summary>
	///     Disposes async inbox api with all related resources.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

	/// <inheritdoc />
	public ValueTask<string> CreateInboxAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig,
		ContainerPolicyWithoutItem? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(
			() => _inboxApi.CreateInbox(contextId, users, managers, publicMeta, privateMeta, filesConfig, policies),
			token);
	}

	/// <inheritdoc />
	public ValueTask UpdateInboxAsync(string inboxId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig, long version, bool force,
		bool forceGenerateNewKey, ContainerPolicyWithoutItem? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(
			() => _inboxApi.UpdateInbox(inboxId, users, managers, publicMeta, privateMeta, filesConfig, version, force,
				forceGenerateNewKey, policies), token);
	}

	/// <inheritdoc />
	public ValueTask<PrivMX.Endpoint.Inbox.Models.Inbox> GetInboxAsync(string inboxId,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.GetInbox(inboxId), token);
	}

	/// <inheritdoc />
	public ValueTask<PagingList<PrivMX.Endpoint.Inbox.Models.Inbox>> ListInboxesAsync(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.ListInboxes(contextId, pagingQuery), token);
	}

	/// <inheritdoc />
	public ValueTask<InboxPublicView> GetInboxPublicViewAsync(string inboxId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.GetInboxPublicView(inboxId), token);
	}

	/// <inheritdoc />
	public ValueTask DeleteInboxAsync(string inboxId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.DeleteInbox(inboxId), token);
	}

	/// <inheritdoc />
	public InboxEntryWriterBuilder GetEntryBuilder(string inboxId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return new InboxEntryWriterBuilder(inboxId, InboxApi);
	}

	/// <inheritdoc />
	public ValueTask<InboxEntry> ReadEntryAsync(string inboxEntryId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.ReadEntry(inboxEntryId), token);
	}

	/// <inheritdoc />
	public ValueTask<PagingList<InboxEntry>> ListEntriesAsync(string inboxId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.ListEntries(inboxId, pagingQuery), token);
	}

	/// <inheritdoc />
	public ValueTask DeleteEntryAsync(string inboxEntryId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.DeleteEntry(inboxEntryId), token);
	}

	/// <inheritdoc />
	public async ValueTask<PrivmxFileStream> OpenFileForRead(InboxEntry entry, string fileId,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		var file = entry.Files.FirstOrDefault(f => f.Info.FileId == fileId);
		if (file is null) throw new ArgumentException($"File with ID {fileId} not found in entry.");
		var handle = await WrapperCallsExecutor.Execute(() => _inboxApi.OpenFile(fileId), token);
		return new InboxReadFileStream(file.Size, handle, InboxApi, file.PublicMeta, file.PrivateMeta);
	}

	/// <inheritdoc />
	public IObservable<InboxEvent> GetInboxEvents()
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return _inboxChannelEventDispatcher;
	}

	/// <inheritdoc />
	public IObservable<InboxEntryEvent> GetEntryEvents(string inboxId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		lock (_entryEventDispatchers)
		{
			if (!_entryEventDispatchers.TryGetValue(inboxId, out var dispatcher))
			{
				dispatcher =
					new InboxEntryEventDispatcher(inboxId, _inboxApi, _connectionId,
						_eventDispatcher);
				_entryEventDispatchers.Add(inboxId, dispatcher);
			}

			return dispatcher;
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (!_disposed.PerformDispose())
			return;
		var exceptions = _entryEventDispatchers.Values.ForEachNotThrowing(dispatcher => dispatcher.Dispose());
		_entryEventDispatchers = null!;
		try
		{
			_inboxChannelEventDispatcher.Dispose();
		}
		catch (Exception exception)
		{
			(exceptions ??= new List<Exception>()).Add(exception);
		}

		_inboxChannelEventDispatcher = null!;
		_inboxApi = null!;
		if (exceptions is not null) throw new AggregateException(exceptions);
	}

	private class InboxChannelEnventDispatcher(
		IInboxApi connection,
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<InboxEvent>("inbox", connectionId, eventDispatcher)
	{
		protected override void OpenChanel()
		{
			connection.SubscribeForInboxEvents();
		}

		protected override void CloseChanel()
		{
			connection.UnsubscribeFromInboxEvents();
		}

		public override void HandleEvent(Event @event)
		{
			switch (@event)
			{
				case InboxCreatedEvent createdEvent:
					WrappedInvokeObservable.Send(
						new InboxEvent(createdEvent));
					break;
				case InboxDeletedEvent deletedEvent:
					WrappedInvokeObservable.Send(
						new InboxEvent(deletedEvent));
					break;
				case InboxUpdatedEvent updatedEvent:
					WrappedInvokeObservable.Send(
						new InboxEvent(updatedEvent));
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}

	private class InboxEntryEventDispatcher(
		string inboxId,
		IInboxApi connection,
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<InboxEntryEvent>($"inbox/{inboxId}/entries", connectionId, eventDispatcher)
	{
		protected override void OpenChanel()
		{
			connection.SubscribeForEntryEvents(inboxId);
		}

		protected override void CloseChanel()
		{
			connection.UnsubscribeFromEntryEvents(inboxId);
		}

		public override void HandleEvent(Event @event)
		{
			switch (@event)
			{
				case InboxEntryCreatedEvent createdEvent:
					WrappedInvokeObservable.Send(
						new InboxEntryEvent(createdEvent));
					break;
				case InboxEntryDeletedEvent deletedEvent:
					WrappedInvokeObservable.Send(
						new InboxEntryEvent(deletedEvent));
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}
}