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

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Events;

namespace PrivMX.Endpoint.Extra.Api.Interfaces;

/// <summary>
///     Interface representing an asynchronous connection.
/// </summary>
public interface IAsyncConnection
{
	/// <summary>
	///     Gets the ID of the current connection.
	/// </summary>
	/// <returns>ID of the connection.</returns>
	public long GetConnectionId();

	/// <summary>
	///     Gets a list of Contexts available for the user.
	/// </summary>
	/// <param name="pagingQuery">List query parameters</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a paging list of contexts.</returns>
	ValueTask<PagingList<Context>> ListContexts(
		PagingQuery pagingQuery, CancellationToken token = default);

	/// <summary>
	///     Gets a list of users of given context.
	/// </summary>
	/// <param name="contextId">ID of the context</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a list of users Info.</returns>
	ValueTask<List<UserInfo>> GetContextUsers(
		string contextId, CancellationToken token = default);

	IObservable<ConnectionEvent> GetConnectionEvents();
}