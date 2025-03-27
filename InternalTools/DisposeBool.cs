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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Internal;

public struct DisposeBool : IEquatable<bool>
{
	private const byte NotDisposed = 0;
	private const byte IsDisposed = 1;
	private volatile int _backingField;

	private bool Disposed => _backingField == IsDisposed;

	/// <summary>
	///     Checks dispose flag and sets it if not set.
	/// </summary>
	/// <returns>true if dispose operation should be performed immediately</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool PerformDispose()
	{
		return Interlocked.Exchange(ref _backingField, IsDisposed) == NotDisposed;
	}

	public bool Equals(bool other)
	{
		return other == Disposed;
	}
#if NET6_0_OR_GREATER
	[StackTraceHidden]
#endif
	public void ThrowIfDisposed(string objectName)
	{
		if (Disposed)
			throw new ObjectDisposedException(objectName);
	}

	public static implicit operator bool(DisposeBool dis)
	{
		return dis.Disposed;
	}

	public static implicit operator DisposeBool(bool val)
	{
		return new DisposeBool
		{
			_backingField = val ? 1 : 0
		};
	}
}