// Module name: PrivmxEndpointCsharpExtra
// File name: Logger.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Extra.Logging;

namespace PrivMX.Endpoint.Extra.Internals;

/// <summary>
///     Manages logging in the library.
/// </summary>
public static class Logger
{
	private static ILibraryLogger _logger = NullLogger.Instance;

	/// <summary>
	///     Sets logger in the library.
	/// </summary>
	/// <param name="logger">Logger that should replace current logger. If null is provided then empty logger is put in place.</param>
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

	private static void Log(LogLevel type, string source, string format, Exception? exception = null)
	{
		_logger.Log(type, source, format, exception);
	}

	private static void Log<T1>(LogLevel type, string source, string format, T1 arg1, Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, exception);
	}

	private static void Log<T1, T2>(LogLevel type, string source, string format, T1 arg1, T2 arg2,
		Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, arg2, exception);
	}

	private static void Log<T1, T2, T3>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
		Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, arg2, arg3, exception);
	}

	private static void Log<T1, T2, T3, T4>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
		T4 arg4,
		Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, arg2, arg3, arg4, exception);
	}

	private static void Log<T1, T2, T3, T4, T5>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
		T4 arg4, T5 arg5,
		Exception? exception = null)
	{
		_logger.Log(type, source, format, arg1, arg2, arg3, arg4, arg5, exception);
	}

	/// <summary>
	///     Helper struct that keeps type of the source class for logging.
	/// </summary>
	/// <typeparam name="T">Source type.</typeparam>
	internal readonly struct SourcedLogger<T>
	{
		private static readonly string TypeName = GetPrettyTypeName(typeof(T));

		private static string GetPrettyTypeName(Type type)
		{
			if (!type.IsGenericType)
				return type.FullName!;

			var genericTypeName = type.GetGenericTypeDefinition().FullName!;
			var genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetPrettyTypeName));
			return $"{genericTypeName.Substring(0, genericTypeName.IndexOf('`'))}<{genericArguments}>";
		}

		internal void Log(LogLevel type, string format, Exception? exception = null)
		{
			Logger.Log(type, TypeName, format, exception);
		}

		internal void Log<T1>(LogLevel type, string format, T1 arg1, Exception? exception = null)
		{
			Logger.Log(type, TypeName, format, arg1, exception);
		}

		internal void Log<T1, T2>(LogLevel type, string format, T1 arg1, T2 arg2, Exception? exception = null)
		{
			Logger.Log(type, TypeName, format, arg1, arg2, exception);
		}

		internal void Log<T1, T2, T3>(LogLevel type, string format, T1 arg1, T2 arg2, T3 arg3,
			Exception? exception = null)
		{
			Logger.Log(type, TypeName, format, arg1, arg2, arg3, exception);
		}

		internal void Log<T1, T2, T3, T4>(LogLevel type, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
			Exception? exception = null)
		{
			Logger.Log(type, TypeName, format, arg1, arg2, arg3, arg4, exception);
		}

		internal void Log<T1, T2, T3, T4, T5>(LogLevel type, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
			Exception? exception = null)
		{
			Logger.Log(type, TypeName, format, arg1, arg2, arg3, arg4, arg5, exception);
		}
	}
}