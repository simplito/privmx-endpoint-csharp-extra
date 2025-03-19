// Module name: Consumer
// File name: Logger.cs
// Last edit: 2025-02-19 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Extra.Logging;

namespace Consumer;

public static class Logger
{
	public static void SetupLogger()
	{
		PrivMX.Endpoint.Extra.Internals.Logger.SetLogger(new ConsoleLogger());
		PrivMX.Endpoint.Extra.Internals.Logger.UnobservedExceptions +=
			exception => Console.WriteLine($"Unobserved exception: {exception}");
	}

	private class ConsoleLogger : ILibraryLogger
	{
		public void Log(LogLevel type, string source, string format, Exception? exception = null)
		{
			if (exception != null)
				Console.WriteLine($"[{type}] <{source}> {format} \n{exception}");
			else
				Console.WriteLine($"[{type}] <{source}> {format}");
		}

		public void Log<T1>(LogLevel type, string source, string format, T1 arg1, Exception? exception = null)
		{
			if (exception != null)
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1)} \n{exception}");
			else
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1)}");
		}

		public void Log<T1, T2>(LogLevel type, string source, string format, T1 arg1, T2 arg2,
			Exception? exception = null)
		{
			if (exception != null)
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2)} \n{exception}");
			else
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2)}");
		}

		public void Log<T1, T2, T3>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
			Exception? exception = null)
		{
			if (exception != null)
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3)} \n{exception}");
			else
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3)}");
		}

		public void Log<T1, T2, T3, T4>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
			Exception? exception = null)
		{
			if (exception != null)
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4)} \n{exception}");
			else
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4)}");
		}

		public void Log<T1, T2, T3, T4, T5>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
			T4 arg4, T5 arg5,
			Exception? exception = null)
		{
			if (exception != null)
				Console.WriteLine(
					$"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4, arg5)} \n{exception}");
			else
				Console.WriteLine($"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4, arg5)}");
		}
	}
}