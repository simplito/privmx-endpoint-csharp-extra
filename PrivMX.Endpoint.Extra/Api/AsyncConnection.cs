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

using Internal;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Api.Interfaces;
using PrivMX.Endpoint.Extra.Internals;
using System.ComponentModel;

namespace PrivMX.Endpoint.Extra.Api;

/// <summary>
///     Connection container that manages connection and exposes asynchronous API.
/// </summary>
public sealed class AsyncConnection : IAsyncDisposable, IAsyncConnection
{
	private static readonly Logger.SourcedLogger<AsyncConnection> Logger = default;
	private DisposeBool _disposeBool;

	/// <summary>
	///     Wraps existing connection into async connection.
	///     It's user responsibility to provide valid (connected) connection.
	/// </summary>
	/// <param name="connection">Connection to wrap</param>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public AsyncConnection(IConnection connection)
	{
		Connection = connection;
	}

	private IConnection Connection { get; }

	/// <inheritdoc />
	public long GetConnectionId()
	{
		_disposeBool.ThrowIfDisposed(nameof(AsyncConnection));
		return Connection.GetConnectionId();
	}

	/// <inheritdoc />
	public ValueTask<PagingList<Context>> ListContexts(
		PagingQuery pagingQuery, CancellationToken token = default)
	{
		_disposeBool.ThrowIfDisposed(nameof(Connection));
		return Connection.ListContextsAsync(pagingQuery, token);
	}

	/// <summary>
	///     Disposes async connection with all related resources.
	/// </summary>
	public ValueTask DisposeAsync()
	{
		if (_disposeBool.PerformDispose())
			return Connection.DisconnectAsync();
		return default;
	}

	/// <summary>
	///     Connects to the PrivMX Bridge server.
	/// </summary>
	/// <param name="userPrivateKey">User's private key.</param>
	/// <param name="solutionId">ID of the Solution.</param>
	/// <param name="platformUrl">PrivMX Bridge URL.</param>
	/// <param name="token">Cancelation token.</param>
	/// <returns>Created and connected instance of the <see cref="Connection" />.</returns>
	public static async Task<AsyncConnection> Connect(string userPrivateKey, string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		Logger.Log(LogLevel.Trace, "Connecting to {0}, solution {1}, with userKey {2}", platformUrl, solutionId,
			userPrivateKey);
		var connection = await ConnectionAsyncExtensions.ConnectAsync(userPrivateKey, solutionId, platformUrl, token);
		return new AsyncConnection(connection);
	}

	/// <summary>
	///     Connects to the PrivMX Bridge server as a guest user.
	/// </summary>
	/// <param name="solutionId">ID of the Solution.</param>
	/// <param name="platformUrl">PrivMX Bridge URL.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Created and connected instance of the <see cref="Connection" />.</returns>
	public static async Task<AsyncConnection> ConnectPublic(string solutionId, string platformUrl,
		CancellationToken token = default)
	{
		Logger.Log(LogLevel.Trace, "Connecting to {0}, solution {1} as public", platformUrl, solutionId);
		var connection = await ConnectionAsyncExtensions.ConnectPublicAsync(solutionId, platformUrl, token);
		return new AsyncConnection(connection);
	}
}