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
using PrivMX.Endpoint.Stream.Models;

namespace PrivMX.Endpoint.Extra.Api.Interfaces;

public interface IAsyncStreamApi
{
	ValueTask<string> CreateStreamRoomAsync(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, ContainerPolicy? policies = null,
		CancellationToken token = default);

	ValueTask UpdateStreamRoomAsync(string streamRoomId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey,
		ContainerPolicy? policies = null, CancellationToken token = default);

	ValueTask DeleteStreamRoomAsync(string streamRoomId, CancellationToken token = default);

	ValueTask<Stream.Models.StreamRoom> GetStreamRoomAsync(string streamRoomId, CancellationToken token = default);

	ValueTask<PagingList<Stream.Models.StreamRoom>> ListStreamRoomsAsync(string contextId, PagingQuery pagingQuery,
		CancellationToken token = default);

    ValueTask<long> CreateStreamAsync(string streamRoomId, CancellationToken token = default);

    ValueTask AddTrackAsync(long streamId,
		TrackType type, string? parameters = null, CancellationToken token = default);

    ValueTask PublishStreamAsync(long streamId,
		CancellationToken token = default);

    ValueTask StreamTrackSendDataAsync(long streamId,
		byte[] data, CancellationToken token = default);

    ValueTask<long> JoinStreamAsync(string streamRoomId, string? settings = null, CancellationToken token = default);

    ValueTask StreamTrackRecvDataAsync(long streamId,
		IObserver<StreamData> observer, CancellationToken token = default);

    ValueTask UnpublishStreamAsync(long streamId,
		CancellationToken token = default);

    ValueTask LeaveStreamAsync(long streamId,
		CancellationToken token = default);
}