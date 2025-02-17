// Module name: PrivmxEndpointCsharpExtra
// File name: NullLogger.cs
// Last edit: 2025-02-17 08:48 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivmxEndpointCsharpExtra.Logging;

namespace PrivmxEndpointCsharpExtra.Internals;

internal sealed class NullLogger : ILibraryLogger
{
	private NullLogger()
	{
	}

	public static NullLogger Instance { get; } = new();

	public void Log(LogLevel type, string source, string format, Exception? exception = null)
	{
	}

	public void Log<T1>(LogLevel type, string source, string format, T1 arg1, Exception? exception = null)
	{
	}

	public void Log<T1, T2>(LogLevel type, string source, string format, T1 arg1, T2 arg2, Exception? exception = null)
	{
	}

	public void Log<T1, T2, T3>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
		Exception? exception = null)
	{
	}
}