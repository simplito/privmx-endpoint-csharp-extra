// Module name: PrivmxEndpointCsharpExtra
// File name: StoreEvent.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal.Unions;
using PrivMX.Endpoint.Store.Models;
using PrivMX.Endpoint.Thread.Models;

namespace PrivmxEndpointCsharpExtra.Events;

public partial struct StoreEvent : IUnion<StoreCreatedEvent, StoreUpdatedEvent, StoreFileDeletedEvent>
{
}