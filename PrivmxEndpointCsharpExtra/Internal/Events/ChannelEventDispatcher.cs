// Module name: PrivmxEndpointCsharpExtra
// File name: ChannelEventDispatcher.cs
// Last edit: 2025-02-17 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using Internal.Observables;
using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Events;

internal abstract class ChannelEventDispatcher<T>(
	string channelName,
	long connectioId,
	IEventDispatcher eventDispatcher)
	: IObservable<T>, IEventHandler
{
	protected static readonly Logger.SourcedLogger<ChannelEventDispatcher<T>> Logger = default;
	private DisposeBool _disposed;
	private int _subscribersCount;

	protected InvokeObservable<T> WrappedInvokeObservable { get; set; } = new();

	public abstract void HandleEvent(Event @event);


	public void Dispose()
	{
		if (!_disposed.PerformDispose())
			return;
		Logger.Log(LogLevel.Trace, "Disposing, channel: {0}, connectionId: {1}", channelName, connectioId);
		WrappedInvokeObservable?.Dispose();
		WrappedInvokeObservable = null!;
		eventDispatcher.RemoveHandler(channelName, connectioId, this);
		WrapperCallsExecutor.Execute(() =>
		{
			Logger.Log(LogLevel.Debug,
				"Unsubscribing from {0} in response to ChannelEventDispatcher dispose", typeof(T).Name);
			try
			{
				CloseChanel();
			}
			catch (Exception exception)
			{
				Logger.Log(LogLevel.Error, "Failed to properly dispose",
					exception);
			}

			Logger.Log(LogLevel.Trace, "Disposed properly, channel {0}, connectionId: {1}", channelName, connectioId);
		}, CancellationToken.None);
	}

	IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
	{
		_disposed.ThrowIfDisposed(nameof(InvokeObservable<T>));
		if (Interlocked.Increment(ref _subscribersCount) == 1)
		{
			eventDispatcher.AddHandler(channelName, connectioId, this);
			WrapperCallsExecutor.Execute(() =>
			{
				Logger.Log(LogLevel.Debug, "Opening chanel for {0}", typeof(T).Name);
				try
				{
					OpenChanel();
				}
				catch (Exception exception)
				{
					Logger.Log(LogLevel.Error, "Failed to open chanel", exception);
					Internals.Logger.PublishUnobservedException(exception);
				}
			}, CancellationToken.None);
		}

		return new WrappingDisposable(this, WrappedInvokeObservable.Subscribe(observer));
	}

	protected abstract void OpenChanel();
	protected abstract void CloseChanel();

	private void DecrementSubscribersCount()
	{
		if (_disposed)
			return;
		Logger.Log(LogLevel.Trace, "Decrementing subscribers count, current count: {0}", _subscribersCount);
		if (Interlocked.Decrement(ref _subscribersCount) == 0)
		{
			eventDispatcher.RemoveHandler(channelName, connectioId, this);
			WrapperCallsExecutor.Execute(() =>
			{
				Logger.Log(LogLevel.Debug, "Closing chanel for {0}", typeof(T).Name);
				try
				{
					CloseChanel();
					Logger.Log(LogLevel.Trace, "Chanel closed for {0}", typeof(T).Name);
				}
				catch (Exception exception)
				{
					Logger.Log(LogLevel.Error, "Failed to close chanel", exception);
					Internals.Logger.PublishUnobservedException(exception);
				}
			}, CancellationToken.None);
		}
	}

	private class WrappingDisposable : IDisposable
	{
		private DisposeBool _disposed;

		public WrappingDisposable(ChannelEventDispatcher<T> channelEventDispatcher,
			InvokeObservable<T>.InvokeObservableSubscriptionDisposer wrappedDisposable)
		{
			ChannelEventDispatcher = channelEventDispatcher;
			WrappedDisposable = wrappedDisposable;
		}

		private ChannelEventDispatcher<T> ChannelEventDispatcher { get; }
		private InvokeObservable<T>.InvokeObservableSubscriptionDisposer WrappedDisposable { get; }

		public void Dispose()
		{
			if (!_disposed.PerformDispose())
				return;
			ChannelEventDispatcher.DecrementSubscribersCount();
			WrappedDisposable.Dispose();
		}
	}
}