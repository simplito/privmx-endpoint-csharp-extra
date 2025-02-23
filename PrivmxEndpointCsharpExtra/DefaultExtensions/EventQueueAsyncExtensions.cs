// Module name: PrivmxEndpointCsharpExtra
// File name: EventQueueAsyncExtensions.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core;
using PrivmxEndpointCsharpExtra.Internals;
using Event = PrivMX.Endpoint.Core.Models.Event;

namespace PrivmxEndpointCsharpExtra;

public static class EventQueueAsyncExtensions
{
	public static ValueTask<Event> WaitEventAsync(this IEventQueue eventQueue,
		CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.WaitEvent, token);
	}

	public static ValueTask<Event?> GetEvent(this IEventQueue eventQueue, CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.GetEvent, token);
	}

	public static ValueTask EmitBreakEventAsync(this IEventQueue eventQueue, CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.EmitBreakEvent, token);
	}
}