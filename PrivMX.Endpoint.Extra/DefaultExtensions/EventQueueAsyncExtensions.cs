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
using PrivMX.Endpoint.Extra.Internals;
using Event = PrivMX.Endpoint.Core.Models.Event;

namespace PrivMX.Endpoint.Extra;

/// <summary>
///     Asynchronous extensions for IEventQueue.
/// </summary>
public static class EventQueueAsyncExtensions
{
	/// <summary>
	///     Gets or waits for a new event from the queue.
	///     Waiting can be canceled by <see cref="EmitBreakEventAsync" />.
	/// </summary>
	/// <returns>A new event.</returns>
	public static ValueTask<Event> WaitEventAsync(this IEventQueue eventQueue,
		CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.WaitEvent, token);
	}

	/// <summary>
	///     Gets a new event from the queue.
	/// </summary>
	/// <returns>A new event, or <see langword="null" /> if no events in the queue.</returns>
	public static ValueTask<Event?> GetEvent(this IEventQueue eventQueue, CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.GetEvent, token);
	}

	/// <summary>
	///     Puts the LibBreakEvent event into the event queue.
	///     This method is useful for interrupting a blocking <see cref="WaitEventAsync" /> call and breaking an event
	///     processing loop.
	/// </summary>
	public static ValueTask EmitBreakEventAsync(this IEventQueue eventQueue, CancellationToken token = default)
	{
		if (eventQueue == null)
			throw new ArgumentNullException(nameof(eventQueue));
		return WrapperCallsExecutor.Execute(eventQueue.EmitBreakEvent, token);
	}
}