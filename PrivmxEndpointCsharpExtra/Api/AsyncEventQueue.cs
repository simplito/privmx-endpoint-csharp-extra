// Module name: PrivmxEndpointCsharpExtra
// File name: AsyncEventQueue.cs
// Last edit: 2025-02-17 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.Runtime.CompilerServices;
using Internal;
using Internal.Extensions;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Api;

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

	public AsyncEventQueue(IEventQueue eventQueue)
	{
		EventQueue = eventQueue;
	}

	private IEventQueue EventQueue { get; set; }

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