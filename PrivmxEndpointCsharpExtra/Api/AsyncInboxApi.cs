// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncInboxApi.cs
// Last edit: 2025-02-23 12:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

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

public class AsyncInboxApi : IAsyncInboxApi, IAsyncDisposable, IDisposable
{
	private static readonly Logger.SourcedLogger<AsyncInboxApi> Logger = default;
	private readonly long _connectionId;
	private DisposeBool _disposed;
	private Dictionary<string, InboxEntryEventDispatcher> _entryEventDispatchers;
	private readonly IEventDispatcher _eventDispatcher;
	private IInboxApi _inboxApi;
	private InboxChannelEnventDispatcher _inboxChannelEventDispatcher;
	internal IInboxApi InboxApi => _disposed ? throw new ObjectDisposedException(nameof(AsyncInboxApi)) : _inboxApi;

	public AsyncInboxApi(Connection connection) : this(PrivMX.Endpoint.Inbox.InboxApi.Create(connection, ThreadApi.Create(connection), StoreApi.Create(connection)), connection.GetConnectionId(), PrivMXEventDispatcher.Instance)
	{
		
	}

	public AsyncInboxApi(IInboxApi inboxApi, long connectionId, IEventDispatcher eventDispatcher)
	{
		_inboxApi = inboxApi;
		_connectionId = connectionId;
		_eventDispatcher = eventDispatcher;
		_entryEventDispatchers = new Dictionary<string, InboxEntryEventDispatcher>();
		_inboxChannelEventDispatcher = new InboxChannelEnventDispatcher(_inboxApi, _connectionId, eventDispatcher);
	}

	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

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
		if (exceptions is not null)
		{
			throw new AggregateException(exceptions);
		}
	}


	public ValueTask<string> CreateInboxAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig,
		ContainerPolicyWithoutItem? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(
			() => _inboxApi.CreateInbox(contextId, users, managers, publicMeta, privateMeta, filesConfig, policies),
			token);
	}

	public ValueTask UpdateInboxAsync(string inboxId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig, long version, bool force,
		bool forceGenerateNewKey, ContainerPolicyWithoutItem? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(
			() => _inboxApi.UpdateInbox(inboxId, users, managers, publicMeta, privateMeta, filesConfig, version, force,
				forceGenerateNewKey, policies), token);
	}

	public ValueTask<PrivMX.Endpoint.Inbox.Models.Inbox> GetInboxAsync(string inboxId,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.GetInbox(inboxId), token);
	}

	public ValueTask<PagingList<PrivMX.Endpoint.Inbox.Models.Inbox>> ListInboxesAsync(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.ListInboxes(contextId, pagingQuery), token);
	}

	public ValueTask<InboxPublicView> GetInboxPublicViewAsync(string inboxId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.GetInboxPublicView(inboxId), token);
	}

	public ValueTask DeleteInboxAsync(string inboxId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.DeleteInbox(inboxId), token);
	}

	public InboxEntryWriterBuilder GetEntryBuilder(string inboxId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return new InboxEntryWriterBuilder(inboxId, InboxApi);
	}

	public ValueTask<InboxEntry> ReadEntryAsync(string inboxEntryId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.ReadEntry(inboxEntryId), token);
	}

	public ValueTask<PagingList<InboxEntry>> ListEntriesAsync(string inboxId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.ListEntries(inboxId, pagingQuery), token);
	}

	public ValueTask DeleteEntryAsync(string inboxEntryId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return WrapperCallsExecutor.Execute(() => _inboxApi.DeleteEntry(inboxEntryId), token);
	}

	public async ValueTask<PrivmxFileStream> OpenFileForWrite(InboxEntry entry, string fileId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		var file = entry.Files.FirstOrDefault(f => f.Info.FileId == fileId);
		if (file is null)
		{
			throw new ArgumentException($"File with ID {fileId} not found in entry.");
		}
		var handle = await WrapperCallsExecutor.Execute(() => _inboxApi.OpenFile(fileId), token);
		return new InboxReadFileStream(file.Size, handle, InboxApi, file.PublicMeta, file.PrivateMeta);
	}

	public IObservable<InboxEvent> GetInboxEvents()
	{
		_disposed.ThrowIfDisposed(nameof(AsyncInboxApi));
		return _inboxChannelEventDispatcher;
	}

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