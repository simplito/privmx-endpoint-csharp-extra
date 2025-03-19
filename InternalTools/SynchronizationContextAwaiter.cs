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
using System.Runtime.CompilerServices;
using System.Threading;

namespace Internal;

public readonly struct SynchronizationContextAwaiter : INotifyCompletion
{
	private static readonly SendOrPostCallback PostCallback = state => ((Action)state!)();

	private readonly SynchronizationContext _context;

	public SynchronizationContextAwaiter(SynchronizationContext context)
	{
		_context = context;
	}

	public bool IsCompleted => _context == SynchronizationContext.Current;

	public void OnCompleted(Action continuation)
	{
		_context.Post(PostCallback, continuation);
	}

	public void GetResult()
	{
	}
}