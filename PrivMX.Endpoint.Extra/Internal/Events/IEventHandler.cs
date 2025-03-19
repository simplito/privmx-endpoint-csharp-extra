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

namespace PrivMX.Endpoint.Extra.Events;

/// <summary>
///     Interface representing an event handler that will be called when event arrive.
/// </summary>
public interface IEventHandler : IDisposable
{
	/// <summary>
	///     Handles incoming event.
	/// </summary>
	/// <param name="event">Event to handle.</param>
	public void HandleEvent(Event @event);
}