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
using PrivMX.Endpoint.Event.Models;
using System.ComponentModel;

namespace PrivMX.Endpoint.Extra.Api;

/// <summary>
///     Asynchronous wrapper over Event API.
/// </summary>
public sealed class AsyncEventApi : IAsyncDisposable, IDisposable, IAsyncEventApi
{
	private readonly long _connectionId;
	private readonly IEventDispatcher _eventDispatcher;

	private readonly IEventApi _eventApi;
	private readonly Dictionary<string, ContextCustomChannelEventDispatcher> _customEventDispatchers;
	private DisposeBool _disposed;

	/// <summary>
	///     Creates async Event API over real PrivMX connection.
	/// </summary>
	/// <param name="connection">Connection used.</param>
	public AsyncEventApi(Connection connection) : this(EventApi.Create(connection), connection.GetConnectionId(),
		PrivMXEventDispatcher.Instance)
	{
	}

	/// <summary>
	///     Wraps existing event api into async event api.
	///     This constructor is meant to be used in advanced scenarios like object mocking and testing.
	/// </summary>
	/// <param name="eventApi">Existing Event API.</param>
	/// <param name="connectionId">ID of user connection.</param>
	/// <param name="eventDispatcher">Event dispatcher used as event source.</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncEventApi(IEventApi eventApi, long connectionId, IEventDispatcher eventDispatcher)
	{
		_eventApi = eventApi;
		_eventDispatcher = eventDispatcher;
		_connectionId = connectionId;
		_customEventDispatchers = new Dictionary<string, ContextCustomChannelEventDispatcher>();
	}

	/// <summary>
	///     Disposes async Event API with all related resources.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		Dispose();
		return default;
	}

	/// <summary>
	///     Emits the custom event on the given Context and channel.
	/// </summary>
	/// <param name="contextId">ID of the Context.</param>
	/// <param name="users">List of <see cref="UserWithPubKey" /> which defines the recipients of the event.</param>
	/// <param name="channelName">Name of the Channel.</param>
	/// <param name="eventData">Event's data.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>ID of the created Thread.</returns>
	public ValueTask EmitEvent(string contextId, List<UserWithPubKey> users, string channelName, byte[] eventData,
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncEventApi));
		return WrapperCallsExecutor.Execute(
			() => _eventApi.EmitEvent(contextId, users, channelName, eventData), token);
	}

	/// <summary>
	///     Stream of events related to a particular custom events.
	/// </summary>
	/// <param name="contextId">ID of the Context.</param>
	/// <param name="channelName">Name of the Channel.</param>
	/// <returns>Observable stream of events.</returns>
	public IObservable<ContextCustomEvent> GetCustomEvents(string contextId, string channelName)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncEventApi));
		lock (_customEventDispatchers)
		{
			string id = $"context/{contextId}/{channelName}";
			if (!_customEventDispatchers.TryGetValue(id, out var dispatcher))
			{
				dispatcher =
					new ContextCustomChannelEventDispatcher(contextId, channelName, _eventApi, _connectionId,
						_eventDispatcher);
				_customEventDispatchers.Add(id, dispatcher);
			}

			return dispatcher;
		}
	}

	/// <summary>
	///     Dispose async Event API with all related resources.
	/// </summary>
	public void Dispose()
	{
		if (_disposed.PerformDispose())
		{
			var exceptions = _customEventDispatchers.Values.ForEachNotThrowing(obj => obj.Dispose());
			if (exceptions is not null)
				throw new AggregateException(exceptions);
		}
	}

	private class ContextCustomChannelEventDispatcher(
		string contextId,
		string channelName,
		IEventApi connection,
		long connectionId,
		IEventDispatcher eventDispatcher)
		: ChannelEventDispatcher<ContextCustomEvent>($"context/{contextId}/{channelName}", connectionId, eventDispatcher)
	{
		private IEventApi Connection { get; } = connection;
		private string ContextId { get; } = contextId;
		private string ChannelName { get; } = channelName;

		protected override void OpenChanel()
		{
			Connection.SubscribeForCustomEvents(ContextId, ChannelName);
		}

		protected override void CloseChanel()
		{
			Connection.UnsubscribeFromCustomEvents(ContextId, ChannelName);
		}

		public override void HandleEvent(Core.Models.Event @event)
		{
			switch (@event)
			{
				case ContextCustomEvent customEvent:
					WrappedInvokeObservable.Send(
						customEvent);
					break;
				default:
					Logger.Log(LogLevel.Warning, "Invalid event was passed to channel dispatcher: {0}.", @event);
					break;
			}
		}
	}
}