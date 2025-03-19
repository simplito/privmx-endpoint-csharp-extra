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

using PrivMX.Endpoint.Extra.Logging;

namespace PrivMX.Endpoint.Extra.Internals;

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

	public void Log<T1, T2, T3, T4>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
		Exception? exception = null)
	{
	}

	public void Log<T1, T2, T3, T4, T5>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
		T5 arg5,
		Exception? exception = null)
	{
	}
}