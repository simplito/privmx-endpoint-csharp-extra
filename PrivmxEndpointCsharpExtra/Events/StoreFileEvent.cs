// Module name: PrivmxEndpointCsharpExtra
// File name: StoreFileEvent.cs
// Last edit: 2025-02-17 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal.Unions;
using PrivMX.Endpoint.Store.Models;
using PrivMX.Endpoint.Thread.Models;

namespace PrivmxEndpointCsharpExtra.Events;
/// <summary>
/// Union of multiple store file events.
/// </summary>
public partial struct StoreFileEvent : IUnion<StoreFileCreatedEvent, StoreFileUpdatedEvent, StoreFileDeletedEvent>;