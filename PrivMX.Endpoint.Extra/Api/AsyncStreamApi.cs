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
using PrivMX.Endpoint.Event;
using PrivMX.Endpoint.Stream;
using PrivMX.Endpoint.Stream.Models;
using PrivMX.Endpoint.Store;
using System.ComponentModel;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace PrivMX.Endpoint.Extra.Api;

public sealed class AsyncStreamApi : IAsyncDisposable, IDisposable, IAsyncStreamApi
{
	private readonly long _connectionId;
	private readonly IEventDispatcher _eventDispatcher;

	private readonly IStreamApi _streamApi;
	private readonly IAsyncStoreApi _storeApi;
	private readonly IAsyncEventApi _eventApi;
	// private readonly ThreadChannelEventDispatcher _threadChannelEventDispatcher;
	// private readonly Dictionary<string, ThreadMessageChannelEventDispatcher> _threadMessageDispatchers;
	private readonly Dictionary<long, string> _map = new Dictionary<long, string>();
	private readonly Dictionary<string, List<UserWithPubKey>> _map2 = new Dictionary<string, List<UserWithPubKey>>();
	private DisposeBool _disposed;

	// public AsyncStreamApi(Connection connection, StoreApi storeApi, EventApi eventApi, IAsyncStoreApi asyncStoreApi, IAsyncEventApi asyncEventApi) : this(StreamApi.Create(connection, storeApi, eventApi), asyncStoreApi, asyncEventApi, connection.GetConnectionId(),
	// 	PrivMXEventDispatcher.Instance)
	// {
	// }

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncStreamApi(IStreamApi streamApi, IAsyncStoreApi asyncStoreApi, IAsyncEventApi asyncEventApi, long connectionId, IEventDispatcher eventDispatcher)
	{
		_streamApi = streamApi;
		_storeApi = asyncStoreApi;
		_eventApi = asyncEventApi;
		_eventDispatcher = eventDispatcher;
		_connectionId = connectionId;
		// _threadChannelEventDispatcher = new ThreadChannelEventDispatcher(_threadApi, connectionId, eventDispatcher);
		// _threadMessageDispatchers = new Dictionary<string, ThreadMessageChannelEventDispatcher>();
	}

	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

	public async ValueTask<string> CreateStreamRoomAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy? policies = null,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		string id = await _streamApi.CreateStreamRoomAsync(contextId, users, managers, publicMeta, convertToPrivateData(users), policies, token);
		lock (_map)
		{
			_map2[id] = users;
		}
		return id;
	}

	public async ValueTask UpdateStreamRoomAsync(string streamRoomId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		await _streamApi.UpdateStreamRoomAsync(streamRoomId, users, managers, publicMeta, convertToPrivateData(users), version, force,
				forceGenerateNewKey, policies, token);
		lock (_map)
		{
			_map2[streamRoomId] = users;
		}
		return;
	}

	public ValueTask DeleteStreamRoomAsync(string streamRoomId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.DeleteStreamRoomAsync(streamRoomId, token);
	}

	public ValueTask<Stream.Models.StreamRoom> GetStreamRoomAsync(string streamRoomId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.GetStreamRoomAsync(streamRoomId, token);
	}

	public ValueTask<PagingList<Stream.Models.StreamRoom>> ListStreamRoomsAsync(string contextId, PagingQuery pagingQuery,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.ListStreamRoomsAsync(contextId, pagingQuery, token);
	}

	public ValueTask<long> CreateStreamAsync(string streamRoomId, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.CreateStreamAsync(streamRoomId, token);
	}

    public ValueTask AddTrackAsync(long streamId,
		TrackType type, string? parameters = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.AddTrackAsync(streamId, type, parameters, token);
	}

    public ValueTask PublishStreamAsync(long streamId,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.PublishStreamAsync(streamId, token);
	}

    public async ValueTask StreamTrackSendDataAsync(long streamId,
		byte[] data, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		string id;
		lock (_map)
		{
			id = _map[streamId];
		}
		var room = await GetStreamRoomAsync(id, token);
		var users = convertFromPrivateMeta(room.PrivateMeta);
		await _eventApi.EmitEvent(room.ContextId, users, $"stream/${id}", data);
	}

    public async ValueTask<long> JoinStreamAsync(string streamRoomId, string? settings = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		long id = await _streamApi.JoinStreamAsync(streamRoomId, settings, token);
		// _storeApi.GetStoreEvents().Subscribe(ev =>
		//        {
		// 	       ev.Match(_ => {},
		// 		       evt => {
		// 					lock (_map)
		// 					{
		// 						_map2[evt.StoreId] = convertFromPrivateMeta(evt.PrivateMeta);
		// 					}
		// 			   },
		// 		       _ => {},
		// 		       _ => {});
		//        });
		return id;
	}

    public async ValueTask StreamTrackRecvDataAsync(long streamId,
		IObserver<StreamData> observer, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		// return _streamApi.StreamTrackRecvDataAsync(streamId, observer, token);
		string id;
		lock (_map)
		{
			id = _map[streamId];
		}
		var room = await GetStreamRoomAsync(id, token);

		// _eventApi.GetCustomEvents(room.ContextId, $"stream/${id}").Subscribe(e => {
			// if (e.Channel == $"context/${room.ContextId}/stream/${id}") {
			// 	var data = new StreamData() { UserId = e.Data.UserId, Data = e.Data.Payload };
			// 	observer.OnNext(data);
			// }
		// });
	}

    public ValueTask UnpublishStreamAsync(long streamId,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.UnpublishStreamAsync(streamId, token);
	}

    public ValueTask LeaveStreamAsync(long streamId,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		return _streamApi.LeaveStreamAsync(streamId, token);
	}

	// public IObservable<ThreadEvent> GetThreadEvents()
	// {
	// 	_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
	// 	return _threadChannelEventDispatcher;
	// }

	// public IObservable<ThreadMessageEvent> GetThreadMessageEvents(string threadId)
	// {
	// 	_disposed.ThrowIfDisposed(nameof(AsyncThreadApi));
	// 	lock (_threadMessageDispatchers)
	// 	{
	// 		if (!_threadMessageDispatchers.TryGetValue(threadId, out var dispatcher))
	// 		{
	// 			dispatcher =
	// 				new ThreadMessageChannelEventDispatcher(threadId, _threadApi, _connectionId,
	// 					_eventDispatcher);
	// 			_threadMessageDispatchers.Add(threadId, dispatcher);
	// 		}

	// 		return dispatcher;
	// 	}
	// }

	public void Dispose()
	{
		// if (_disposed.PerformDispose())
		// {
		// 	_threadChannelEventDispatcher.Dispose();
		// 	var exceptions = _threadMessageDispatchers.Values.ForEachNotThrowing(obj => obj.Dispose());
		// 	if (exceptions is not null)
		// 		throw new AggregateException(exceptions);
		// }
	}

	// private class ThreadChannelEventDispatcher(
	// 	IThreadApi connection,
	// 	long connectionId,
	// 	IEventDispatcher eventDispatcher)
	// 	: ChannelEventDispatcher<ThreadEvent>("thread", connectionId, eventDispatcher)
	// {
	// 	private IThreadApi Connection { get; } = connection;

	// 	protected override void OpenChanel()
	// 	{
	// 		Connection.SubscribeForThreadEvents();
	// 	}

	// 	protected override void CloseChanel()
	// 	{
	// 		Connection.UnsubscribeFromThreadEvents();
	// 	}

	// 	public override void HandleEvent(Core.Models.Event @event)
	// 	{
	// 		switch (@event)
	// 		{
	// 			case ThreadCreatedEvent createdEvent:
	// 				WrappedInvokeObservable.Send(
	// 					new ThreadEvent(createdEvent));
	// 				break;
	// 			case ThreadDeletedEvent deletedEvent:
	// 				WrappedInvokeObservable.Send(
	// 					new ThreadEvent(deletedEvent));
	// 				break;
	// 			case ThreadUpdatedEvent updatedEvent:
	// 				WrappedInvokeObservable.Send(
	// 					new ThreadEvent(updatedEvent));
	// 				break;
	// 			case ThreadStatsChangedEvent statsChangedEvent:
	// 				WrappedInvokeObservable.Send(
	// 					new ThreadEvent(statsChangedEvent));
	// 				break;
	// 			default:
	// 				Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
	// 				break;
	// 		}
	// 	}
	// }

	// private class ThreadMessageChannelEventDispatcher(
	// 	string threadId,
	// 	IThreadApi connection,
	// 	long connectionId,
	// 	IEventDispatcher eventDispatcher)
	// 	: ChannelEventDispatcher<ThreadMessageEvent>($"thread/{threadId}/messages", connectionId, eventDispatcher)
	// {
	// 	private IThreadApi Connection { get; } = connection;
	// 	private string ThreadId { get; } = threadId;

	// 	protected override void OpenChanel()
	// 	{
	// 		Connection.SubscribeForMessageEvents(ThreadId);
	// 	}

	// 	protected override void CloseChanel()
	// 	{
	// 		Connection.UnsubscribeFromMessageEvents(ThreadId);
	// 	}

	// 	public override void HandleEvent(Core.Models.Event @event)
	// 	{
	// 		switch (@event)
	// 		{
	// 			case ThreadNewMessageEvent createdEvent:
	// 				WrappedInvokeObservable.Send(
	// 					new ThreadMessageEvent(createdEvent));
	// 				break;
	// 			case ThreadMessageDeletedEvent deletedEvent:
	// 				WrappedInvokeObservable.Send(
	// 					new ThreadMessageEvent(deletedEvent));
	// 				break;
	// 			default:
	// 				Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
	// 				break;
	// 		}
	// 	}
	// }

	private byte[] convertToPrivateData(List<UserWithPubKey> users)
	{
		byte[] bytes;
		IFormatter formatter = new BinaryFormatter();
		using (MemoryStream stream = new MemoryStream())
		{
			formatter.Serialize(stream, users);
			bytes = stream.ToArray();
		}
		return bytes;
	}

	private List<UserWithPubKey> convertFromPrivateMeta(byte[] data)
	{
		IFormatter formatter = new BinaryFormatter();
		using (MemoryStream stream = new MemoryStream())
		{
			stream.Write(data);
			return (List<UserWithPubKey>)formatter.Deserialize(stream);
		}
	}
}