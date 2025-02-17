// Module name: PrivmxEndpointCsharpExtra
// File name: Logger.cs
// Last edit: 2025-02-17 08:48 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivmxEndpointCsharpExtra.Logging;

namespace PrivmxEndpointCsharpExtra.Internals;

public static class Logger
{
	private static ILibraryLogger _logger = NullLogger.Instance;

	public static void SetLogger(ILibraryLogger? logger)
	{
		if (logger == null)
			_logger = NullLogger.Instance;
		else
			_logger = logger;
	}

	/// <summary>
	///     Event where all unobserved exceptions are published.
	///     Such events may come from async methods that are not awaited or dispose calls of the library objects.
	/// </summary>
	public static event Action<Exception>? UnobservedExceptions;

	internal static void PublishUnobservedException(Exception exception)
	{
		UnobservedExceptions?.Invoke(exception);
	}

	internal static void Log(LogLevel type, string source, string format, Exception? exception = null)
	{
		_logger.Log(type, source, format, exception);
	}

	internal static void Log<T1>(LogLevel type, string source, string format, T1 arg1, Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, exception);
	}

	internal static void Log<T1, T2>(LogLevel type, string source, string format, T1 arg1, T2 arg2,
		Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, arg2, exception);
	}

	internal static void Log<T1, T2, T3>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
		Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, arg2, arg3, exception);
	}

	/// <summary>
	///     Helper struct that keeps type of the source class for logging.
	/// </summary>
	/// <typeparam name="T">Source type.</typeparam>
	internal readonly struct SourcedLogger<T>
	{
		internal void Log(LogLevel type, string format, Exception? exception = null)
		{
			Logger.Log(type, typeof(T).FullName!, format, exception);
		}

		internal void Log<T1>(LogLevel type, string format, T1 arg1, Exception? exception = null)
		{
			Logger.Log(type, typeof(T).FullName!, format, arg1, exception);
		}

		internal void Log<T1, T2>(LogLevel type, string format, T1 arg1, T2 arg2, Exception? exception = null)
		{
			Logger.Log(type, typeof(T).FullName!, format, arg1, arg2, exception);
		}

		internal void Log<T1, T2, T3>(LogLevel type, string format, T1 arg1, T2 arg2, T3 arg3,
			Exception? exception = null)
		{
			Logger.Log(type, typeof(T).FullName!, format, arg1, arg2, arg3, exception);
		}
	}
}