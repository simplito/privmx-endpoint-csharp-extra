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
using PrivMX.Endpoint.Event.Models;

namespace PrivMX.Endpoint.Extra.Api.Interfaces;

/// <summary>
///     Interface for asynchronous operations related to Custom Events in the PrivMX platform.
/// </summary>
public interface IAsyncEventApi
{
	/// <summary>
	///     Emits the custom event on the given Context and channel.
	/// </summary>
	/// <param name="contextId">ID of the Context.</param>
	/// <param name="users">List of <see cref="UserWithPubKey" /> which defines the recipients of the event.</param>
	/// <param name="channelName">Name of the Channel.</param>
	/// <param name="eventData">Event's data.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>ID of the created Thread.</returns>
	ValueTask EmitEvent(string contextId, List<UserWithPubKey> users, string channelName, byte[] eventData,
		CancellationToken token = default);

	/// <summary>
	///     Gets the events in a given Thread and channel.
	/// </summary>
	/// <param name="threadId">ID of the Thread to get events from.</param>
	/// <returns>Stream of Thread message events.</returns>
	IObservable<ContextCustomEvent> GetCustomEvents(string contextId, string channelName);
}