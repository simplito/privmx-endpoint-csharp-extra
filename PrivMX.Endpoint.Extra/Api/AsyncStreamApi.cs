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
using PrivMX.Endpoint.Event.Models;
using PrivMX.Endpoint.Store.Models;
using System.ComponentModel;

using System.Text;
using Newtonsoft.Json;

namespace PrivMX.Endpoint.Extra.Api;

public sealed class AsyncStreamApi : IAsyncDisposable, IDisposable, IAsyncStreamApi
{
	private readonly long _connectionId;
	private readonly IEventDispatcher _eventDispatcher;

	private readonly IStreamApi _streamApi;
	private readonly IAsyncStoreApi _storeApi;
	private readonly IAsyncEventApi _eventApi;
	private readonly StreamRoomUsersProvider _usersProvider;
	private readonly StreamIdGenerator _idGenerator = new StreamIdGenerator();
	private IDisposable? _subscription = null;
	// private readonly ThreadChannelEventDispatcher _threadChannelEventDispatcher;
	// private readonly Dictionary<string, ThreadMessageChannelEventDispatcher> _threadMessageDispatchers;

	private DisposeBool _disposed;

	// public AsyncStreamApi(Connection connection, StoreApi storeApi, EventApi eventApi, IAsyncStoreApi asyncStoreApi, IAsyncEventApi asyncEventApi) : this(StreamApi.Create(connection, storeApi, eventApi), asyncStoreApi, asyncEventApi, connection.GetConnectionId(),
	// 	PrivMXEventDispatcher.Instance)
	// {
	// }

	class StreamIdGenerator
	{
		private long _id = 1;

		public long Generate()
		{
			return _id++;
		}
	}

	class StreamRoomUsersProvider
	{
		public class UsersInfo
		{
			public string StreamRoomId { get; set; }
			public string ContextId { get; set; }
			public List<UserWithPubKey> Users { get; set; }
		}

		public class RoomInfo
		{
			public string StreamRoomId { get; set; }
			public string ContextId { get; set; }
		}

		public delegate ValueTask<UsersInfo> GetUsersFromBridge(string streamRoomId, CancellationToken token = default);

		private readonly Dictionary<long, string> _map = new Dictionary<long, string>();
		private readonly Dictionary<string, List<UserWithPubKey>> _map2 = new Dictionary<string, List<UserWithPubKey>>();
		private readonly Dictionary<string, string> _map3 = new Dictionary<string, string>();
		private readonly GetUsersFromBridge _getter;
		
		public StreamRoomUsersProvider(GetUsersFromBridge getter) {
			_getter = getter;
		}

		public void SetStreamId(long streamId, string streamRoomId)
		{
			lock(_map)
			{
				_map[streamId] = streamRoomId;
			}
		}

		public void SetUsers(string streamRoomId, List<UserWithPubKey> users)
		{
			lock(_map)
			{
				_map2[streamRoomId] = users;
			}
		}

		public void SetContextId(string streamRoomId, string contextId)
		{
			lock(_map)
			{
				_map3[streamRoomId] = contextId;
			}
		}

		public async ValueTask<UsersInfo> GetUsers(long streamId, CancellationToken token = default)
		{
			bool needFetch = false;
			string streamRoomId;
			lock(_map)
			{
				streamRoomId = _map[streamId];
				if (!_map2.ContainsKey(streamRoomId) || !_map3.ContainsKey(streamRoomId))
				{
					needFetch = true;
				}
			}
			if(needFetch)
			{
				var users = await _getter(streamRoomId, token);
				lock(_map)
				{
					_map2[streamRoomId] = users.Users;
					_map3[streamRoomId] = users.ContextId;
				}
			}
			lock(_map)
			{
				return new UsersInfo() {
					StreamRoomId = streamRoomId,
					ContextId = _map3[streamRoomId],
					Users = _map2[streamRoomId]
				};
			}
		}

		public RoomInfo GetRoomInfo(long streamId)
		{
			lock(_map)
			{
				return new RoomInfo() {
					StreamRoomId = _map[streamId],
					ContextId = _map3[_map[streamId]]
				};
			}
		}
	}

	class StreamDataAdapter : IObservable<StreamData>
	{
		private readonly IAsyncEventApi _eventApi;
		private readonly string _contextId;
		private readonly string _streamRoomId;

		class CustomObserverAdapter : IObserver<ContextCustomEvent>
		{
			private readonly IObserver<StreamData> _observer;
			private readonly string _channel;

			public CustomObserverAdapter(IObserver<StreamData> observer, string channel)
			{
				_observer = observer;
				_channel = channel;
			}

			public void OnCompleted()
			{
				_observer.OnCompleted();
			}

			public void OnError(Exception e)
			{
				_observer.OnError(e);
			}

			public void OnNext(ContextCustomEvent ev)
			{
				if (ev.Channel == _channel) {
					var data = new StreamData() { UserId = ev.Data.UserId, Data = ev.Data.Payload };
					_observer.OnNext(data);
				}
			}
		}

		public StreamDataAdapter(IAsyncEventApi eventApi, string contextId, string streamRoomId)
		{
			_eventApi = eventApi;
			_contextId = contextId;
			_streamRoomId = streamRoomId;
		}

		public IDisposable Subscribe(IObserver<StreamData> observer)
		{
			return _eventApi.GetCustomEvents(_contextId, $"stream/{_streamRoomId}").Subscribe(
				new CustomObserverAdapter(observer, $"context/{_contextId}/stream/{_streamRoomId}"));
		}
	}

	class StoreEventsAdapter : IObserver<StoreEvent>
	{
		private readonly StreamRoomUsersProvider _usersProvider;
		
		public StoreEventsAdapter(StreamRoomUsersProvider usersProvider)
		{
			_usersProvider = usersProvider;
		}

		public void OnCompleted()
		{
		}

		public void OnError(Exception e)
		{
		}

		public void OnNext(StoreEvent ev)
		{
			if (ev.Is<StoreCreatedEvent>())
			{
				var store = ((StoreCreatedEvent)ev).Data;
				_usersProvider.SetContextId(store.StoreId, store.ContextId);
				_usersProvider.SetUsers(store.StoreId, AsyncStreamApi.convertFromPrivateMeta(store.PrivateMeta));
			}
			else if (ev.Is<StoreUpdatedEvent>())
			{
				var store = ((StoreUpdatedEvent)ev).Data;
				_usersProvider.SetContextId(store.StoreId, store.ContextId);
				_usersProvider.SetUsers(store.StoreId, AsyncStreamApi.convertFromPrivateMeta(store.PrivateMeta));
			}
		}

	}

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
		_usersProvider = new StreamRoomUsersProvider(async (streamRoomId, token) => {
			var room = await GetStreamRoomAsync(streamRoomId, token);
			return new StreamRoomUsersProvider.UsersInfo() {
				StreamRoomId = streamRoomId,
				ContextId = room.ContextId,
				Users = convertFromPrivateMeta(room.PrivateMeta)
			};
		});
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
		string streamRoomId = await _streamApi.CreateStreamRoomAsync(contextId, users, managers, publicMeta, convertToPrivateData(users), policies, token);
		_usersProvider.SetUsers(streamRoomId, users);
		_usersProvider.SetContextId(streamRoomId, contextId);
		return streamRoomId;
	}

	public async ValueTask UpdateStreamRoomAsync(string streamRoomId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? policies = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		await _streamApi.UpdateStreamRoomAsync(streamRoomId, users, managers, publicMeta, convertToPrivateData(users), version, force,
				forceGenerateNewKey, policies, token);
		_usersProvider.SetUsers(streamRoomId, users);
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
		long streamId = _idGenerator.Generate();
		_usersProvider.SetStreamId(streamId, streamRoomId);
		return new ValueTask<long>(streamId);
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
		var users = await _usersProvider.GetUsers(streamId);
		await _eventApi.EmitEvent(users.ContextId, users.Users, $"stream/{users.StreamRoomId}", data);
	}

    public async ValueTask<long> JoinStreamAsync(string streamRoomId, string? settings = null, CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		long streamId = _idGenerator.Generate();
		_usersProvider.SetStreamId(streamId, streamRoomId);
		await _usersProvider.GetUsers(streamId);
		return streamId;
	}

    public IObservable<StreamData> StreamTrackRecvData(long streamId)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncStreamApi));
		var room = _usersProvider.GetRoomInfo(streamId);
		return new StreamDataAdapter(_eventApi, room.ContextId, room.StreamRoomId);
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
		lock(_subscription) 
		{
			if (_subscription != null)
			{
				_subscription.Dispose();
			}
		}
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

	public static byte[] convertToPrivateData(List<UserWithPubKey> users)
	{
		string json = JsonConvert.SerializeObject(users);
		return Encoding.UTF8.GetBytes(json);
	}

	public static List<UserWithPubKey> convertFromPrivateMeta(byte[] data)
	{
		string json = Encoding.UTF8.GetString(data);
		return JsonConvert.DeserializeObject<List<UserWithPubKey>>(json);
	}

	private void ensureSubscribed()
	{
		lock(_subscription)
		{
			if (_subscription == null)
			{
				_subscription = _storeApi.GetStoreEvents().Subscribe(new StoreEventsAdapter(_usersProvider));
			}
		}
	}
}
