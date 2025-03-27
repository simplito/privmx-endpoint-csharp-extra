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

namespace Internal;

public static class EnumerableExtensions
{
	public static List<Exception>? ForEachNotThrowing<T>(this IEnumerable<T> collection, Action<T> action,
		List<Exception>? exceptions = null)
	{
		foreach (var element in collection)
			try
			{
				action(element);
			}
			catch (Exception exception)
			{
				(exceptions ??= new List<Exception>()).Add(exception);
			}

		return exceptions;
	}
}