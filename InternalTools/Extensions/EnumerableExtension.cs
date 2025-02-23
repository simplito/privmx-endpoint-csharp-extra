// Module name: InternalTools
// File name: EnumerableExtension.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;
using System.Collections.Generic;

namespace Internal.Extensions;

internal static class EnumerableExtensions
{
	public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		List<Exception>? exceptions = null;
		foreach (var element in enumerable)
		{
			try
			{
				action(element);
			}
			catch (Exception exception)
			{
				if (exceptions == null)
					exceptions = new List<Exception> { exception };
				else
					exceptions.Add(exception);
			}

			if (exceptions != null)
				throw new AggregateException(exceptions);
		}
	}

	public static void ForEach<T, TArg>(this IEnumerable<T> enumerable, Action<T, TArg> action, TArg arg)
	{
		List<Exception>? exceptions = null;
		foreach (var element in enumerable)
		{
			try
			{
				action(element, arg);
			}
			catch (Exception exception)
			{
				if (exceptions == null)
					exceptions = new List<Exception> { exception };
				else
					exceptions.Add(exception);
			}

			if (exceptions != null)
				throw new AggregateException(exceptions);
		}
	}
}