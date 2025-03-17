// Module name: PrivmxEndpointCsharpExtra
// File name: IWrapperCallsExecutor.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
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
	/// <summary>
	///     Executes native function and returns result.
	/// </summary>
	/// <param name="func">Function to execute.</param>
	/// <param name="cancellationToken">Operation cancellation token.</param>
	/// <typeparam name="T">Returned object type.</typeparam>
	/// <returns>Value task that represents asynchronous operation.</returns>
	ValueTask<T> Execute<T>(Func<T> func, CancellationToken cancellationToken);

	/// <summary>
	///     Executes native function and returns result.
	/// </summary>
	/// <param name="action">Function to execute.</param>
	/// <param name="cancellationToken">Operation cancellation token.</param>
	/// <returns>Value task that represents asynchronous operation.</returns>
	ValueTask Execute(Action action, CancellationToken cancellationToken);
}