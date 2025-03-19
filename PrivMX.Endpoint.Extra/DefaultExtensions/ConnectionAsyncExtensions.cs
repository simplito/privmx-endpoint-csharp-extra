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

using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Internals;

namespace PrivMX.Endpoint.Extra;

/// <summary>
///     Extension methods that provide asynchronous method execution for objects implementing <see cref="IConnection" />
///     interface.
///     Internally operations are executed using default <see cref="ThreadPool" />.
/// </summary>
public static class ConnectionAsyncExtensions
{
	/// <summary>
	///     Connects to the PrivMX Bridge server.
	/// </summary>
	/// <param name="userPrivKey">User's private key.</param>
	/// <param name="solutionId">ID of the Solution.</param>
	/// <param name="platformUrl">PrivMX Bridge URL.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Created and connected instance of the Connection</returns>
	public static ValueTask<Connection> ConnectAsync(string userPrivKey, string solutionId,
		string platformUrl, CancellationToken token = default)
	{
		return WrapperCallsExecutor.Execute(
			() => Connection.Connect(userPrivKey, solutionId, platformUrl), token);
	}

	/// <summary>
	///     Connects to the PrivMX Bridge server as a guest user.
	/// </summary>
	/// <param name="solutionId">ID of the Solution.</param>
	/// <param name="platformUrl">PrivMX Bridge URL.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Created and connected instance of the Connection.</returns>
	public static ValueTask<Connection> ConnectPublicAsync(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		return WrapperCallsExecutor.Execute(() => Connection.ConnectPublic(solutionId, platformUrl), token);
	}

	/// <summary>
	///     Gets a list of Contexts available for the user.
	/// </summary>
	/// <param name="connection">Extended object.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of contexts.</returns>
	public static ValueTask<PagingList<Context>> ListContextsAsync(this IConnection connection,
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (connection is null)
			throw new ArgumentNullException(nameof(connection));
		return WrapperCallsExecutor.Execute(() => connection.ListContexts(pagingQuery), token);
	}

	/// <summary>
	///     Disconnects from the PrivMX Bridge.
	/// </summary>
	public static ValueTask DisconnectAsync(this IConnection connection, CancellationToken token = default)
	{
		if (connection is null)
			throw new ArgumentNullException(nameof(connection));
		return WrapperCallsExecutor.Execute(connection.Disconnect, token);
	}

	/// <summary>
	///     Gets the ID of the current connection.
	/// </summary>
	/// <returns>ID of the connection.</returns>
	public static ValueTask<long> GetConnectionIdAsync(this IConnection connection, CancellationToken token = default)
	{
		if (connection is null)
			throw new ArgumentNullException(nameof(connection));
		return WrapperCallsExecutor.Execute(connection.GetConnectionId, token);
	}
}