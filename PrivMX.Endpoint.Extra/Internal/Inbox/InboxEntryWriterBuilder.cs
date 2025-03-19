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

using PrivMX.Endpoint.Inbox;

namespace PrivMX.Endpoint.Extra.Inbox;

/// <summary>
///     Builder structure used to declare content of new inbox entry.
/// </summary>
/// <param name="inboxId">ID of inbox where new entry is created.</param>
/// <param name="inboxApi">Backing inbox api</param>
public ref struct InboxEntryWriterBuilder(string inboxId, IInboxApi inboxApi)
{
	private byte[] _data = [];

	private readonly Dictionary<string, (byte[] privateMeta, byte[] publicMeta, long lenght, byte? fillValue)> _files =
		new();

	private string? _privateKey;

	/// <summary>
	///     Declares data to be written to the entry.
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>Self</returns>
	public InboxEntryWriterBuilder SetData(byte[] data)
	{
		_privateKey = null;
		_data = data;
		return this;
	}

	/// <summary>
	///     Sets private key to be used for encryption.
	/// </summary>
	/// <param name="privateKey">Private key</param>
	/// <returns>Self</returns>
	public InboxEntryWriterBuilder SetPrivateKey(string privateKey)
	{
		_privateKey = privateKey;
		return this;
	}

	/// <summary>
	///     Adds file to the entry.
	/// </summary>
	/// <param name="knownName">Known name that can be later used to reference this file when content is written.</param>
	/// <param name="publicMeta">Public meta.</param>
	/// <param name="privateMeta">Private meta.</param>
	/// <param name="lenght">Size of the file (bytes)</param>
	/// <param name="fillValue">Value used to fill empty space in file on save</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown when another file with the same name was declared.</exception>
	public InboxEntryWriterBuilder AddFile(string knownName, byte[] publicMeta, byte[] privateMeta, long lenght,
		byte? fillValue = null)
	{
		if (!_files.TryAdd(knownName, (privateMeta, publicMeta, lenght, fillValue)))
			throw new InvalidOperationException($"Filed with known name: {knownName}, already exists");
		return this;
	}

	/// <summary>
	///     Informs server that new entry should be created.
	/// </summary>
	/// <param name="token">Cancellation token.</param>
	/// <returns>New entry writer.</returns>
	public ValueTask<ManagedInboxEntryWriter> BuildAsync(CancellationToken token = default)
	{
		return PrepareEntryAsync(inboxApi, inboxId, _data, _privateKey, _files, token);
	}

	private static async ValueTask<ManagedInboxEntryWriter> PrepareEntryAsync(IInboxApi inboxApi, string inboxId,
		byte[] data, string? privateKye,
		Dictionary<string, (byte[] privateMeta, byte[] publicMeta, long lenght, byte? fillValue)> files,
		CancellationToken token = default)
	{
		List<long> handles = new();
		foreach (var (_, file) in files)
		{
			var handle = await inboxApi.CreateFileHandleAsync(file.publicMeta, file.privateMeta, file.lenght, token);
			handles.Add(handle);
		}

		var entryHandle = await inboxApi.PrepareEntryAsync(inboxId, data, handles, privateKye!, token);
		var streams = new Dictionary<string, InboxWriteFileStream>();
		foreach (var (name, privateMeta, publicMeta, lenght, fillValue, handle) in files.Zip(handles,
			         (pair, handle) => (pair.Key, pair.Value.privateMeta, pair.Value.publicMeta, pair.Value.lenght,
				         pair.Value.fillValue, l: handle)))
			streams.Add(name,
				new InboxWriteFileStream(null, entryHandle, handle, lenght, publicMeta, privateMeta, fillValue,
					inboxApi));
		return new ManagedInboxEntryWriter(inboxId, entryHandle, streams, inboxApi);
	}
}