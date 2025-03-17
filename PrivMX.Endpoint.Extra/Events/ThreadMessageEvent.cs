// Module name: PrivmxEndpointCsharpExtra
// File name: ThreadMessageEvent.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal.Unions;
using PrivMX.Endpoint.Thread.Models;

namespace PrivmxEndpointCsharpExtra.Events;

/// <summary>
///     Union of multiple thread message events.
/// </summary>
public readonly partial struct ThreadMessageEvent : IUnion<ThreadNewMessageEvent, ThreadMessageDeletedEvent>;