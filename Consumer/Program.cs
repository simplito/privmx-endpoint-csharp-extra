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
using PrivMX.Endpoint.Crypto;
using PrivMX.Endpoint.Extra;
using PrivMX.Endpoint.Extra.Api;
using PrivMX.Endpoint.Extra.Events;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Extra.Logging;
using PrivMX.Endpoint.Thread;

namespace Consumer;

internal class Program
{
	const string password = "";
	const string salt = "";
	const string solutionId = "";
	const string contextId = "";

	// private static async Task Clean(Connection connection)
	// {
	// 	await using var api = new AsyncThreadApi(connection);
	// 	var threads = await api.ListThreadsAsync(contextId, PagingQueryExtension.DefaultQuery);
	// 	foreach (var thread in threads.ReadItems)
	// 	{
	// 		await api.DeleteThreadAsync(thread.ThreadId);
	// 	}
	// }

    private static async Task Main(string[] args)
	{
		Logger.SetupLogger();
		using var globalEvents = new GlobalEvents();
		using (globalEvents.AllEvents()
			       .Subscribe(ev =>
				       Console.WriteLine(
					       $"GlobalObserver - Channel: {ev.Channel}, Type: {ev.Type}, Instance id: {ev.ConnectionId}")))
		{
			// await TestStore.Test();
			await TestInbox.Test();
			// var origConnection =
			// 	await ConnectionAsyncExtensions.ConnectAsync(privateKey, solutionId, "http://localhost:9111");
			// await using var connection = new AsyncConnection(origConnection);
			// Console.WriteLine($"Connection id: {origConnection.GetConnectionId()}");
			// var contexts = await connection.ListContexts(PagingQueryExtension.DefaultQuery);
			// var context = contexts.ReadItems.First();
			// var threadApi = new AsyncThreadApi(origConnection);
			// var users = new List<UserWithPubKey>();
			// users.Add(new UserWithPubKey
			// {
			// 	PubKey = publicKey,
			// 	UserId = context.UserId
			// });
			// using (threadApi.GetThreadEvents().Subscribe(ev =>
			//        {
			// 	       ev.Match(_ => Console.WriteLine("Observer - ThreadCreated"),
			// 		       _ => Console.WriteLine("Observer - ThreadUpdated"),
			// 		       _ => Console.WriteLine("Observer - ThreadDeleted"),
			// 		       _ => Console.WriteLine("Observer - ThreadStatsChanged"));
			//        }))
			// {
			// 	var threadId = await threadApi.CreateThreadAsync(contextId, users, users, [], []);
			// 	Console.WriteLine($"Created thread: {threadId}");
			// 	using (threadApi.GetThreadMessageEvents(threadId).Subscribe(ev =>
			// 	       {
			// 		       ev.Match(_ => Console.WriteLine("Observer - MessageCreated"),
			// 			       _ => Console.WriteLine("Observer - MessageDeleted"));
			// 	       }))
			// 	{
			// 		var messageId = await threadApi.SendMessageAsync(threadId, [], [],
			// 			"Hello world!"u8.ToArray());
			// 		Console.WriteLine($"Created message: {messageId}");
			await Task.Delay(1000);
			//}

		}

		// {
		// 	var origConnection =
		// 		await ConnectionAsyncExtensions.ConnectAsync(privateKey, solutionId, "http://localhost:9111");
		// 	await using var connection =
		// 		new AsyncConnection(origConnection);
		// 	Console.WriteLine($"Connection id: {origConnection.GetConnectionId()}");
		// }
	}
}