// Module name: PrivmxEndpointCsharpExtra
// File name: IEventHandler.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;

namespace PrivmxEndpointCsharpExtra.Events;

/// <summary>
///     Interface representing an event handler that will be called when event arrive.
/// </summary>
public interface IEventHandler : IDisposable
{
	/// <summary>
	///     Handles incoming event.
	/// </summary>
	/// <param name="event">Event to handle.</param>
	public void HandleEvent(Event @event);
}