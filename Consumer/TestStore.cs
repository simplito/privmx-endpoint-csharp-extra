// Module name: Consumer
// File name: TestStore.cs
// Last edit: 2025-02-19 20:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivmxEndpointCsharpExtra;

namespace Consumer;

public class TestStore
{      
	public static async Task Test()
	{
		await using var connection = await Connection.CreateSession();
		var result = await connection.Connection.ListContexts(PagingQueryExtension.DefaultQuery);
		var context = result.ReadItems.First();
		var user = new UserWithPubKey
		{
			PubKey = connection.PublicKey,
			UserId = context.UserId
		};
		using (connection.StoreApi.GetStoreEvents().Subscribe(ev =>
		       {
			       ev.Match(_ => Console.WriteLine("Observer - StoreCreated"),
				       _ => Console.WriteLine("Observer - StoreUpdated"),
				       _ => Console.WriteLine("Observer - StoreDeleted"),
				       _ => Console.WriteLine("Observer - StoreStatsChanged"));
		       }))
		{
			var storeId = await connection.StoreApi.CreateStore(context.ContextId, [user], [user], [], [], null);
			Console.WriteLine($"Created store: {storeId}");
			using (connection.StoreApi.GetFileEvents(storeId).Subscribe(ev =>
			       {
				       ev.Match(
					       _ => Console.WriteLine("Observer - FileCreated"),
					       _ => Console.WriteLine("Observer - FileUpdated"),
					       _ => Console.WriteLine("Observer - FileDeleted")
				       );
			       }))
			{
				// the writing is not pretty - can't file have dynamic size?
				var message = "Hello, World!";
				await using (var writeStream =
				             new StreamWriter(await connection.StoreApi.CreateFile(storeId, 1024, [], [], 0x0)))
				{
					await writeStream.WriteAsync(message);
					// await writeStream.WriteAsync(inputAsBytes[3..]);
					// await writeStream.WriteAsync(inputAsBytes);
					Console.WriteLine($"Wrote message: {message} to file.");
				}

				var file =
					(await connection.StoreApi.ListFiles(storeId, PagingQueryExtension.DefaultQuery)).ReadItems.First();
				Console.WriteLine($"Created file id: {file.Info.FileId}");
				try
				{
					using var readStream =
						new StreamReader(await connection.StoreApi.OpenFileForRead(file.Info.FileId));
					var content = await readStream.ReadToEndAsync();
					Console.WriteLine($"Read message: {content} from file.");
				}
				finally
				{
					await connection.StoreApi.DeleteStore(storeId);
					Console.WriteLine($"Deleted store: {storeId}");
				}

				await Task.Delay(1000);
			}
		}
	}
}