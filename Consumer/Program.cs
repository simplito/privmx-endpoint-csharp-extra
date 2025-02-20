// Module name: Consumer
// File name: Program.cs
// Last edit: 2025-02-17 08:48 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Crypto;
using PrivMX.Endpoint.Thread;
using PrivmxEndpointCsharpExtra;
using PrivmxEndpointCsharpExtra.Api;
using PrivmxEndpointCsharpExtra.Events;
using PrivmxEndpointCsharpExtra.Internals;
using PrivmxEndpointCsharpExtra.Logging;

namespace Consumer;

internal class Program
{
	const string password = "password1";
	const string salt = "test";
	const string solutionId = "3b54def9-dca1-434f-9231-f3852a83d878";
	const string contextId = "8a1bcb91-e34c-4192-a0b7-2bd5975d014c";

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
					       $"Observer - Channel: {ev.Channel}, Type: {ev.Type}, Instance id: {ev.ConnectionId}")))
		{
			await TestStore.Test();
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