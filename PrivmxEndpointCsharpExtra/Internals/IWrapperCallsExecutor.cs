// Module name: PrivmxEndpointCsharpExtra
// File name: IWrapperCallsExecutor.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

namespace PrivmxEndpointCsharpExtra.Internals;

/// <summary>
///     Interface for object used to execute calls to native library trough privmx-endpoint-csharp
///     https://github.com/simplito/privmx-endpoint-csharp.
/// </summary>
public interface IWrapperCallsExecutor
{
	ValueTask<T> Execute<T>(Func<T> func, CancellationToken cancellationToken);
	ValueTask Execute(Action action, CancellationToken cancellationToken);
}