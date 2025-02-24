// Module name: PrivmxEndpointCsharpExtra
// File name: EventQueueAsyncExtensions.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core;
using PrivmxEndpointCsharpExtra.Internals;
using Event = PrivMX.Endpoint.Core.Models.Event;

namespace PrivmxEndpointCsharpExtra;

/// <summary>
///     Asynchronous extensions for IEventQueue.
/// </summary>
public static class EventQueueAsyncExtensions
{
	/// <summary>
	///     Gets or waits for a new event from the queue.
	///     Waiting can be canceled by <see cref="EmitBreakEventAsync" />.
	/// </summary>
	/// <returns>A new event.</returns>
	public static ValueTask<Event> WaitEventAsync(this IEventQueue eventQueue,
		CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.WaitEvent, token);
	}

	/// <summary>
	///     Gets a new event from the queue.
	/// </summary>
	/// <returns>A new event, or <see langword="null" /> if no events in the queue.</returns>
	public static ValueTask<Event?> GetEvent(this IEventQueue eventQueue, CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.GetEvent, token);
	}

	/// <summary>
	///     Puts the LibBreakEvent event into the event queue.
	///     This method is useful for interrupting a blocking <see cref="WaitEventAsync" /> call and breaking an event
	///     processing loop.
	/// </summary>
	public static ValueTask EmitBreakEventAsync(this IEventQueue eventQueue, CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.EmitBreakEvent, token);
	}
}