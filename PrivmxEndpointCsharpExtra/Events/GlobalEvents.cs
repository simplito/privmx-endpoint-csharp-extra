// Module name: PrivmxEndpointCsharpExtra
// File name: GlobalEvents.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.ComponentModel;
using Internal;
using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra.Events.Internal;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Events;

public class GlobalEvents : IDisposable
{
	private readonly NonExistingChannelDispatcher _channelDispatcher;
	private readonly IEventDispatcher _eventDispatcher;
	private DisposeBool _disposed;

	public GlobalEvents()
	{
		_eventDispatcher = PrivMXEventDispatcher.GetDispatcher();
		_channelDispatcher = new NonExistingChannelDispatcher(_eventDispatcher);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public GlobalEvents(IEventDispatcher eventDispatcher)
	{
		_eventDispatcher = eventDispatcher;
		_channelDispatcher = new NonExistingChannelDispatcher(_eventDispatcher);
	}

	public void Dispose()
	{
		if (!_disposed.PerformDispose())
			return;
		_channelDispatcher.Dispose();
	}

	public IObservable<Event> AllEvents()
	{
		_disposed.ThrowIfDisposed(nameof(GlobalEvents));
		return _channelDispatcher;
	}

	private class NonExistingChannelDispatcher : ChannelEventDispatcher<Event>
	{
		private static readonly Logger.SourcedLogger<NonExistingChannelDispatcher> Logger = default;

		public NonExistingChannelDispatcher(IEventDispatcher eventDispatcher) : base(
			PrivMXEventDispatcher.WildcardChannel, eventDispatcher)
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