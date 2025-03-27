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

namespace PrivMX.Endpoint.Extra.Internals;

/// <summary>
///     Interface for object used to execute calls to native library trough privmx-endpoint-csharp
///     https://github.com/simplito/privmx-endpoint-csharp.
/// </summary>
public interface IWrapperCallsExecutor
{
	/// <summary>
	///     Executes native function and returns result.
	/// </summary>
	/// <param name="func">Function to execute.</param>
	/// <param name="cancellationToken">Operation cancellation token.</param>
	/// <typeparam name="T">Returned object type.</typeparam>
	/// <returns>Value task that represents asynchronous operation.</returns>
	ValueTask<T> Execute<T>(Func<T> func, CancellationToken cancellationToken);

	/// <summary>
	///     Executes native function and returns result.
	/// </summary>
	/// <param name="action">Function to execute.</param>
	/// <param name="cancellationToken">Operation cancellation token.</param>
	/// <returns>Value task that represents asynchronous operation.</returns>
	ValueTask Execute(Action action, CancellationToken cancellationToken);
}