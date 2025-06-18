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
using PrivMX.Endpoint.Stream;
using PrivMX.Endpoint.Stream.Models;

namespace PrivMX.Endpoint.Extra;

public static class StreamApiAsyncExtensions
{
	public static ValueTask<string> CreateStreamRoomAsync(this IStreamApi streamApi, string contextId,
		List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
		ContainerPolicy? containerPolicy = null,
		CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentNullException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(
			() => streamApi.CreateStreamRoom(contextId, users, managers, publicMeta, privateMeta, containerPolicy), token);
	}

	public static ValueTask UpdateStreamRoomAsync(this IStreamApi streamApi, string streamRoomId,
		List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
		long version, bool force, bool forceGenerateNewKey, ContainerPolicy? containerPolicy = null,
		CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentNullException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(
			() => streamApi.UpdateStreamRoom(streamRoomId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, containerPolicy), token);
	}

	public static ValueTask DeleteStreamRoomAsync(this IStreamApi streamApi, string streamRoomId,
		CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.DeleteStreamRoom(streamRoomId), token);
	}

	public static ValueTask<Stream.Models.StreamRoom> GetStreamRoomAsync(this IStreamApi streamApi,
		string streamRoomId, CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.GetStreamRoom(streamRoomId), token);
	}

	public static ValueTask<PagingList<Stream.Models.StreamRoom>> ListStreamRoomsAsync(this IStreamApi streamApi,
		string contextId, PagingQuery pagingQuery, CancellationToken token = default)
	{
		if (streamApi == null) throw new ArgumentNullException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.ListStreamRooms(contextId, pagingQuery), token);
	}

    public static ValueTask<long> CreateStreamAsync(this IStreamApi streamApi,
		string streamRoomId, CancellationToken token = default)
	{
		if (streamApi == null) throw new ArgumentNullException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.CreateStream(streamRoomId), token);
	}

    public static ValueTask AddTrackAsync(this IStreamApi streamApi, long streamId,
		TrackType type, string? parameters = null, CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.AddTrack(streamId, type, parameters), token);
	}

    public static ValueTask PublishStreamAsync(this IStreamApi streamApi, long streamId,
		CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.PublishStream(streamId), token);
	}

    public static ValueTask StreamTrackSendDataAsync(this IStreamApi streamApi, long streamId,
		byte[] data, CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.StreamTrackSendData(streamId, data), token);
	}

    public static ValueTask<long> JoinStreamAsync(this IStreamApi streamApi,
		string streamRoomId, string? settings = null, CancellationToken token = default)
	{
		if (streamApi == null) throw new ArgumentNullException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.JoinStream(streamRoomId, settings), token);
	}

    public static ValueTask StreamTrackRecvDataAsync(this IStreamApi streamApi, long streamId,
		IObserver<StreamData> observer, CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.StreamTrackRecvData(streamId, observer), token);
	}

    public static ValueTask UnpublishStreamAsync(this IStreamApi streamApi, long streamId,
		CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.UnpublishStream(streamId), token);
	}

    public static ValueTask LeaveStreamAsync(this IStreamApi streamApi, long streamId,
		CancellationToken token = default)
	{
		if (streamApi is null) throw new ArgumentException(nameof(streamApi));
		return WrapperCallsExecutor.Execute(() => streamApi.LeaveStream(streamId), token);
	}
}