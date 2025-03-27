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

namespace PrivMX.Endpoint.Extra.Events;

/// <summary>
///     Dispatcher that routes events from a single connection.
/// </summary>
public interface IEventDispatcher
{
	/// <summary>
	///     Adds handler that listens for events related to its channel id.
	/// </summary>
	/// <param name="channelId">Channel related to events consumed by this handler.</param>
	/// <param name="connectionId">Handler connection ID.</param>
	/// <param name="handler">Incoming events handler.</param>
	void AddHandler(string channelId, long connectionId, IEventHandler handler);

	/// <summary>
	///     Removes handler that listens for events related to its channel id.
	/// </summary>
	/// <param name="channelId">Channel related to events consumed by this handler.</param>
	/// <param name="connectionId">Handler connection ID.</param>
	/// <param name="handler">Incoming events handler.</param>
	void RemoveHandler(string channelId, long connectionId, IEventHandler handler);
}