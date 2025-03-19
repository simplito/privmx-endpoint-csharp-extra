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
using PrivMX.Endpoint.Extra;
using PrivMX.Endpoint.Inbox.Models;

namespace Consumer;

public static class TestInbox
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
		using (connection.InboxApi.GetInboxEvents().Subscribe(ev => ev.Match(
			       _ => Console.WriteLine("Observer - InboxCreated"),
			       _ => Console.WriteLine("Observer - InboxDeleted"))))
		{
			var inboxId = await connection.InboxApi.CreateInboxAsync(context.ContextId,
				new List<UserWithPubKey> { user },
				new List<UserWithPubKey> { user }, [], [], new FilesConfig
				{
					MaxCount = 10,
					MaxFileSize = 1024,
					MaxWholeUploadSize = 3 * 1024,
					MinCount = 0
				});
			Console.WriteLine("Created inbox, id: " + inboxId);
			using (connection.InboxApi.GetEntryEvents(inboxId).Subscribe(ev =>
					   ev.Match(_ => Console.WriteLine("Observer - EntryCreated"),
						   _ => Console.WriteLine("Observer - EntryDeleted")
				       )))
			{
				await using (var writer = await connection.InboxApi.GetEntryBuilder(inboxId)
					             .AddFile("myFile", [], [], 100, 0x0)
					             .AddFile("secondFile", [], [], 100, 0x0)
					             .BuildAsync())
				{
					await using (var textWriter = new StreamWriter(writer.FileStreams["myFile"]))
					{
						textWriter.Write("Hello,");
					}

					await using (var textWriter = new StreamWriter(writer.FileStreams["secondFile"]))
					{
						textWriter.Write(" world!");
					}
				}
			}
			await connection.InboxApi.DeleteInboxAsync(inboxId);
			await Task.Delay(1000);
		}
	}
}