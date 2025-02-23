// Module name: InternalTools
// File name: CollectionExtensions.cs
// Last edit: 2025-02-23 11:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;
using System.Collections.Generic;

namespace Internal;

public static class EnumerableExtensions
{
	public static List<Exception>? ForEachNotThrowing<T>(this IEnumerable<T> collection, Action<T> action, List<Exception>? exceptions = null)
	{
		foreach (var element in collection)
		{
			try
			{
				action(element);
			}
			catch (Exception exception)
			{
				(exceptions ??= new List<Exception>()).Add(exception);
			}
		}

		return exceptions;
	}
}