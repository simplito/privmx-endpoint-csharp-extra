// Module name: PrivmxEndpointCsharpExtra
// File name: ILibraryLogger.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Extra.Internals;

namespace PrivMX.Endpoint.Extra.Logging;

/// <summary>
///     Interface for all every logger used in the library.
/// </summary>
public interface ILibraryLogger
{
	/// <summary>
	///     Logs single event.
	/// </summary>
	/// <param name="type">Log type</param>
	/// <param name="source">Log message source.</param>
	/// <param name="format">Format string.</param>
	/// <param name="exception">Exception associated with this log.</param>
	public void Log(LogLevel type, string source, string format, Exception? exception = null);

	/// <summary>
	///     Logs single event.
	/// </summary>
	/// <param name="type">Log type</param>
	/// <param name="source">Log message source.</param>
	/// <param name="format">Format string.</param>
	/// <param name="exception">Exception associated with this log.</param>
	/// <param name="arg1">Argument passed to string format.</param>
	/// <typeparam name="T1">Format string argument type</typeparam>
	public void Log<T1>(LogLevel type, string source, string format, T1 arg1, Exception? exception = null);

	/// <summary>
	///     Logs single event.
	/// </summary>
	/// <param name="type">Log type</param>
	/// <param name="source">Log message source.</param>
	/// <param name="format">Format string.</param>
	/// <param name="exception">Exception associated with this log.</param>
	/// <param name="arg1">Argument passed to string format.</param>
	/// <param name="arg2">Second argument passed to string format.</param>
	/// <typeparam name="T1">Format string argument type</typeparam>
	/// ///
	/// <typeparam name="T2">Format string second argument type</typeparam>
	public void Log<T1, T2>(LogLevel type, string source, string format, T1 arg1, T2 arg2, Exception? exception = null);

	/// <summary>
	///     Logs single event.
	/// </summary>
	/// <param name="type">Log type</param>
	/// <param name="source">Log message source.</param>
	/// <param name="format">Format string.</param>
	/// <param name="exception">Exception associated with this log.</param>
	/// <param name="arg1">Argument passed to string format.</param>
	/// <param name="arg2">Second argument passed to string format.</param>
	/// <param name="arg3">Third argument passed to string format.</param>
	/// <typeparam name="T1">Format string argument type</typeparam>
	/// <typeparam name="T2">Format string second argument type</typeparam>
	/// <typeparam name="T3">Format string third argument type.</typeparam>
	public void Log<T1, T2, T3>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
		Exception? exception = null);

	/// <summary>
	///     Logs single event.
	/// </summary>
	/// <param name="type">Log type</param>
	/// <param name="source">Log message source.</param>
	/// <param name="format">Format string.</param>
	/// <param name="exception">Exception associated with this log.</param>
	/// <param name="arg1">Argument passed to string format.</param>
	/// <param name="arg2">Second argument passed to string format.</param>
	/// <param name="arg3">Third argument passed to string format.</param>
	/// <param name="arg4">Fourth argument passed to string format.</param>
	/// <typeparam name="T1">Format string argument type</typeparam>
	/// <typeparam name="T2">Format string second argument type</typeparam>
	/// <typeparam name="T3">Format string third argument type.</typeparam>
	/// <typeparam name="T4">Format string fourth argument type.</typeparam>
	public void Log<T1, T2, T3, T4>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
		Exception? exception = null);

	/// <summary>
	///     Logs single event.
	/// </summary>
	/// <param name="type">Log type</param>
	/// <param name="source">Log message source.</param>
	/// <param name="format">Format string.</param>
	/// <param name="exception">Exception associated with this log.</param>
	/// <param name="arg1">Argument passed to string format.</param>
	/// <param name="arg2">Second argument passed to string format.</param>
	/// <param name="arg3">Third argument passed to string format.</param>
	/// <param name="arg4">Fourth argument passed to string format.</param>
	/// <param name="arg5">Fifth argument passed to string format.</param>
	/// <typeparam name="T1">Format string argument type</typeparam>
	/// <typeparam name="T2">Format string second argument type</typeparam>
	/// <typeparam name="T3">Format string third argument type.</typeparam>
	/// <typeparam name="T4">Format string fourth argument type.</typeparam>
	/// <typeparam name="T5">Format string fifth argument type.</typeparam>
	public void Log<T1, T2, T3, T4, T5>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
		T5 arg5,
		Exception? exception = null);
}