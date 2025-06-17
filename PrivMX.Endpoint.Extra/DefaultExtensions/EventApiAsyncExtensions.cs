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
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Event;
using PrivMX.Endpoint.Event.Models;

namespace PrivMX.Endpoint.Extra;

public static class EventApiAsyncExtensions
{
	public static ValueTask EmitEventAsync(this IEventApi eventApi, string contextId,
		List<UserWithPubKey> users, string channelName, byte[] eventData,
		CancellationToken token = default)
	{
		if (eventApi is null) throw new ArgumentNullException(nameof(eventApi));
		return WrapperCallsExecutor.Execute(
			() => eventApi.EmitEvent(contextId, users, channelName, eventData), token);
	}

	public static ValueTask SubscribeForCustomEventsAsync(this IEventApi api, string contextId,
		string channelName,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.SubscribeForCustomEvents(contextId, channelName), token);
	}

	public static ValueTask UnsubscribeFromCustomEventsAsync(this IEventApi api, string contextId,
		string channelName,
		CancellationToken token = default)
	{
		if (api == null) throw new ArgumentNullException(nameof(api));
		return WrapperCallsExecutor.Execute(() => api.UnsubscribeFromCustomEvents(contextId, channelName), token);
	}
}