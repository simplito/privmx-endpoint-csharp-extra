﻿//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Internal;
using Internal.Extensions;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Internals;
using System.Runtime.CompilerServices;

namespace PrivMX.Endpoint.Extra.Api;

/// <summary>
///     Wrapper for event queue that allows to iterate events asynchronously.
/// </summary>
public sealed class AsyncEventQueue : IAsyncDisposable
{
	private const int IsRunning = 1;
	private const int IsNotRunning = 0;
	private static readonly Logger.SourcedLogger<AsyncEventQueue> Logger = default;
	private DisposeBool _disposed;
	private volatile int _isRunningFlag;

	/// <summary>
	///     Wraps exising event queue into async event queue.
	///     It's user responsibility to provide valid event queue.
	/// </summary>
	/// <param name="eventQueue">Event queue to wrap</param>
	public AsyncEventQueue(IEventQueue eventQueue)
	{
		EventQueue = eventQueue;
	}

	private IEventQueue EventQueue { get; set; }

	/// <summary>
	///     Stops and disposes event queue.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (!_disposed.PerformDispose())
			return;
		await SynchronizationContextHelper.ThreadPoolSynchronizationContext;
		try
		{
			if (Interlocked.Exchange(ref _isRunningFlag, IsNotRunning) == IsRunning)
			{
				Logger.Log(LogLevel.Trace, "Breaking event loop.");
				EventQueue.EmitBreakEvent();
				Logger.Log(LogLevel.Trace, "Event loop interrupted without errors.");
			}
		}
		catch (Exception ex)
		{
			Logger.Log(LogLevel.Error, "Failed to break event queue.", ex);
			Internals.Logger.PublishUnobservedException(ex);
		}
		finally
		{
			EventQueue = null!;
		}
	}

	/// <summary>
	///     Iterates events until eiter event queue breaks or cancellation token is cancelled.
	/// </summary>
	/// <param name="token"></param>
	/// <exception cref="InvalidOperationException">Throw when this method is called when another enumeration is in progress.</exception>
	/// <returns>Stream of incoming events.</returns>
	public async IAsyncEnumerable<Event> WaitEventsAsync(
		[EnumeratorCancellation]
		CancellationToken token = default)
	{
		_disposed.ThrowIfDisposed(nameof(AsyncEventQueue));
		if (Interlocked.Exchange(ref _isRunningFlag, IsRunning) == IsRunning)
			throw new InvalidOperationException("Event loop is already running.");
		await SynchronizationContextHelper.ThreadPoolSynchronizationContext;
		token.ThrowIfCancellationRequested();
		CancellationTokenRegistration registration = default;
		if (token.CanBeCanceled)
			registration = token.Register(EventQueue.EmitBreakEvent);
		try
		{
			while (!token.IsCancellationRequested && !_disposed) yield return EventQueue.WaitEvent();
		}
		finally
		{
			_isRunningFlag = IsNotRunning;
			await registration.DisposeAsync();
		}
	}
}