// Module name: PrivmxEndpointCsharpExtra
// File name: IEventHandler.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;

namespace PrivmxEndpointCsharpExtra.Events;

public interface IEventHandler : IDisposable
{
	public void HandleEvent(Event @event);
}