// Module name: PrivmxEndpointCsharpExtra
// File name: PrivMXEventDispatcher.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using Internal.Extensions;
using PrivMX.Endpoint.Extra.Api;
using PrivMX.Endpoint.Extra.Internals;
using Exception = System.Exception;

namespace PrivMX.Endpoint.Extra.Events.Internal;

/// <summary>
///     Utility class that listens for incoming events and dispatches them to appropriate event handlers.
/// </summary>
internal sealed class PrivMXEventDispatcher : IEventDispatcher
{
	internal const string WildcardChannel = "*";
	internal const long WildcardConnectionId = 0;
	private const int IsRunning = 1;
	private const int IsNotRunning = 0;
	public static readonly PrivMXEventDispatcher Instance = new();
	private readonly Dictionary<string, IEventHandler> _chanelNameToObservables = new();
	private readonly Logger.SourcedLogger<PrivMXEventDispatcher> Logger = default;
	private int _isRunning;

	private PrivMXEventDispatcher()
	{
		EventQueue =
			new AsyncEventQueue(PrivMX.Endpoint.Core.EventQueue
				.GetInstance()); // TODO: Refactor this when event queue migrates from global singleton to per connection.
		CancellationTokenSource = new CancellationTokenSource();
	}

	private AsyncEventQueue EventQueue { get; }
	private CancellationTokenSource CancellationTokenSource { get; set; }

	public void AddHandler(string channel, long connectionId, IEventHandler handler)
	{
		// ReSharper disable once InconsistentlySynchronizedField
		if (_chanelNameToObservables.ContainsKey(channel))
			throw new InvalidOperationException(
				"Only single handler per channel is supported. Remove previous handler first.");

		// lock for write 
		lock (_chanelNameToObservables)
		{
			if (_chanelNameToObservables.TryGetValue(channel, out var currentHandlers))
				throw new InvalidOperationException(
					"Only single handler per channel is supported. Remove previous handler first.");
			_chanelNameToObservables[channel] = handler;
			if (Interlocked.Exchange(ref _isRunning, IsRunning) == IsNotRunning)
				EventPump();
		}
	}

	public void RemoveHandler(string channel, long connectionId, IEventHandler handler)
	{
		// ReSharper disable once InconsistentlySynchronizedField
		if (!_chanelNameToObservables.TryGetValue(channel, out var originalHandlers))
			return;

		// lock for write 
		lock (_chanelNameToObservables)
		{
			if (!_chanelNameToObservables.Remove(channel, out var removed))
				return;
			removed.Dispose();
			if (_chanelNameToObservables.Count == 0 && Interlocked.Exchange(ref _isRunning, IsNotRunning) == IsRunning)
			{
				CancellationTokenSource.Cancel();
				CancellationTokenSource = new CancellationTokenSource();
			}
		}
	}

	~PrivMXEventDispatcher()
	{
		CancellationTokenSource.Cancel();
		CancellationTokenSource.Dispose();
	}

	private async void EventPump()
	{
		await SynchronizationContextHelper.ThreadPoolSynchronizationContext;
		Logger.Log(LogLevel.Trace, "Starting EventPump");
		try
		{
			await foreach (var serializedEvent in EventQueue.WaitEventsAsync(CancellationTokenSource.Token))
			{
				Logger.Log(LogLevel.Debug, "Dispatching event {0}", serializedEvent);
				IEventHandler? handler = null;
				try
				{
					// ReSharper disable once InconsistentlySynchronizedField
					if (_chanelNameToObservables.TryGetValue(serializedEvent.Channel, out handler))
						handler.HandleEvent(serializedEvent);
					if (_chanelNameToObservables.TryGetValue(WildcardChannel, out handler))
						handler.HandleEvent(serializedEvent);
				}
				catch (Exception exception)
				{
					Logger.Log(LogLevel.Error, "Failed to dispatch event {0} in event dispatcher {1}",
						serializedEvent, handler,
						exception);
					Internals.Logger.PublishUnobservedException(exception);
				}
			}
		}
		catch (Exception exception)
		{
			Logger.Log(LogLevel.Error, "Event pump finished exceptionally", exception);
		}

		Logger.Log(LogLevel.Trace, "Event pump finished");
	}
}