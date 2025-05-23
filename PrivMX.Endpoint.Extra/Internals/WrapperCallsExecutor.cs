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

using Internal.Extensions;

namespace PrivMX.Endpoint.Extra.Internals;

/// <summary>
///     Class responsible dispatching calls to native library.
/// </summary>
public static class WrapperCallsExecutor
{
	private static IWrapperCallsExecutor _executor = default!;

	static WrapperCallsExecutor()
	{
		_executor = new NoCancellationThreadPoolExecutor();
	}

	/// <summary>
	///     Sets executor that will be used to dispatch calls to native library.
	/// </summary>
	/// <param name="executor">Executor implementation</param>
	public static void SetExecutor(IWrapperCallsExecutor? executor)
	{
		if (executor is null)
			_executor = new NoCancellationThreadPoolExecutor();
		else
			_executor = executor;
	}

	internal static ValueTask Execute(Action action, CancellationToken cancellationToken = default)
	{
		return _executor.Execute(action, cancellationToken);
	}

	internal static ValueTask<T> Execute<T>(Func<T> action, CancellationToken cancellationToken = default)
	{
		return _executor.Execute(action, cancellationToken);
	}

	/// <summary>
	///     Executor that schedules calls to wrapper library onto thread pool threads.
	///     Cancellation tokens are ignored.
	/// </summary>
	private class NoCancellationThreadPoolExecutor : IWrapperCallsExecutor
	{
		private static readonly SynchronizationContext _threadPoolSynchronizationContext = new();

		async ValueTask<T> IWrapperCallsExecutor.Execute<T>(Func<T> func, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await _threadPoolSynchronizationContext;
			try
			{
				var result = func();
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException(cancellationToken);
				return result;
			}
			catch (Exception ex)
			{
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException(null, ex, cancellationToken);
				throw;
			}
		}

		async ValueTask IWrapperCallsExecutor.Execute(Action action, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await _threadPoolSynchronizationContext;
			try
			{
				action();
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException(cancellationToken);
			}
			catch (Exception ex)
			{
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException(null, ex, cancellationToken);
				throw;
			}
		}
	}
}