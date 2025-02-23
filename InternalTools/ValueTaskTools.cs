// Module name: InternalTools
// File name: ValueTaskTools.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Internal;

public class ValueTaskTools
{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public static async ValueTask WhenAll(params ValueTask[] tasks)
	{
		if (tasks is null)
			throw new ArgumentNullException(nameof(tasks));

		// We don't allocate the list if no task throws
		List<Exception>? exceptions = null;

		for (var i = 0; i < tasks.Length; i++)
			try
			{
				await tasks[i].ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				exceptions ??= new List<Exception>(tasks.Length);
				exceptions.Add(ex);
			}

		if (exceptions is not null)
			throw new AggregateException(exceptions);
	}

	public static async ValueTask<T[]> WhenAll<T>(params ValueTask<T>[] tasks)
	{
		if (tasks is null)
			throw new ArgumentNullException(nameof(tasks));
		if (tasks.Length == 0)
			return Array.Empty<T>();

		// We don't allocate the list if no task throws
		List<Exception>? exceptions = null;

		var results = new T[tasks.Length];
		for (var i = 0; i < tasks.Length; i++)
			try
			{
				results[i] = await tasks[i].ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				exceptions ??= new List<Exception>(tasks.Length);
				exceptions.Add(ex);
			}

		return exceptions is null
			? results
			: throw new AggregateException(exceptions);
	}
#endif
}