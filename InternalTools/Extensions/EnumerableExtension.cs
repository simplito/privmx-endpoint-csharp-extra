//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

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