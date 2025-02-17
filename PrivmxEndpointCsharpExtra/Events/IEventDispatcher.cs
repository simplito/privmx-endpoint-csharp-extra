// Module name: PrivmxEndpointCsharpExtra
// File name: IEventDispatcher.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

namespace PrivmxEndpointCsharpExtra.Events;

/// <summary>
///     Dispatcher that routes events from a single connection.
/// </summary>
public interface IEventDispatcher
{
	/// <summary>
	///     Adds handler that listens for events related to its channel id.
	/// </summary>
	/// <param name="channelId">Channel related to events consumed by this handler.</param>
	/// <param name="handler">Incoming events handler.</param>
	void AddHandler(string channelId, IEventHandler handler);

	/// <summary>
	///     Removes handler that listens for events related to its channel id.
	/// </summary>
	/// <param name="channelId">Channel related to events consumed by this handler.</param>
	/// <param name="handler">Incoming events handler.</param>
	void RemoveHandler(string channelId, IEventHandler handler);
}