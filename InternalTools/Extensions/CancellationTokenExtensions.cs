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
using System.Threading;

namespace Internal.Extensions;

public static class CancellationTokenExtensions
{
	public static IDisposable LinkIfNeeded(this CancellationToken token1, CancellationToken token2,
		out CancellationToken linkedToken)
	{
		if (!token1.CanBeCanceled)
		{
			linkedToken = token2;
			return NullDisposable.Instance;
		}

		if (!token2.CanBeCanceled)
		{
			linkedToken = token1;
			return NullDisposable.Instance;
		}

		var cts = CancellationTokenSource.CreateLinkedTokenSource(token1, token2);
		linkedToken = cts.Token;
		return cts;
	}
}