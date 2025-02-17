// Module name: PrivmxEndpointCsharpExtra
// File name: IEventSource.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivmxEndpointCsharpExtra.Events;

namespace PrivmxEndpointCsharpExtra.Interfaces;

/// <summary>
///     Interface class for object that exposes PrivMX events in structured way.
/// </summary>
public interface IEventSource
{
	/// <summary>
	///     Returns observable that sends events about threads updates (creation of new thread, change in existing thread,
	///     deletion of existing thread)
	/// </summary>
	/// <returns>Observable with thread events.</returns>
	IObservable<ThreadEvent> GetThreadsUpdates();

	/// <summary>
	///     Observable that sends events related to messages in single thread.
	/// </summary>
	/// <param name="threadId">Id of thead for which updates should be send</param>
	/// <returns>Observable that sends events for about messages in the specified thread.</returns>
	IObservable<ThreadMessageEvent> GetThreadMessageUpdates(
		string threadId);
}