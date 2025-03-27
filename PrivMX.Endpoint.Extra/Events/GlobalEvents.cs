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
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Events.Internal;
using System.ComponentModel;

namespace PrivMX.Endpoint.Extra.Events;

/// <summary>
///     Represents global events in the PrivMX platform.
/// </summary>
public class GlobalEvents : IDisposable
{
	private readonly NonExistingChannelDispatcher _channelDispatcher;
	private DisposeBool _disposed;

	/// <summary>
	///     Initializes a new instance of the <see cref="GlobalEvents" /> class.
	/// </summary>
	public GlobalEvents()

	{
		IEventDispatcher eventDispatcher = PrivMXEventDispatcher.Instance;
		_channelDispatcher = new NonExistingChannelDispatcher(eventDispatcher);
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="GlobalEvents" /> class with a specified event dispatcher.
	/// </summary>
	/// <param name="eventDispatcher">The event dispatcher to use.</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public GlobalEvents(IEventDispatcher eventDispatcher)
	{
		_channelDispatcher = new NonExistingChannelDispatcher(eventDispatcher);
	}

	/// <summary>
	///     Disposes the resources used by the <see cref="GlobalEvents" /> class.
	/// </summary>
	public void Dispose()
	{
		if (!_disposed.PerformDispose())
			return;
		_channelDispatcher.Dispose();
	}

	/// <summary>
	///     Gets an observable stream of all events.
	/// </summary>
	/// <returns>An observable stream of <see cref="Event" />.</returns>
	public IObservable<Event> AllEvents()
	{
		_disposed.ThrowIfDisposed(nameof(GlobalEvents));
		return _channelDispatcher;
	}

	private class NonExistingChannelDispatcher : ChannelEventDispatcher<Event>
	{
		public NonExistingChannelDispatcher(IEventDispatcher eventDispatcher) : base(
			PrivMXEventDispatcher.WildcardChannel, 0, eventDispatcher)
		{
		}

		public override void HandleEvent(Event @event)
		{
			WrappedInvokeObservable.Send(@event);
		}


		protected override void OpenChanel()
		{
		}


		protected override void CloseChanel()
		{
		}
	}
}