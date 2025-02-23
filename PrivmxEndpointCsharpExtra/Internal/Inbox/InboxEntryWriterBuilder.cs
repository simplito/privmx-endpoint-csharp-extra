// Module name: PrivmxEndpointCsharpExtra
// File name: InboxEntryWriterBuilder.cs
// Last edit: 2025-02-23 13:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Inbox;
using PrivmxEndpointCsharpExtra.Abstractions;

namespace PrivmxEndpointCsharpExtra.Inbox;

public ref struct InboxEntryWriterBuilder(string inboxId, IInboxApi inboxApi)
{
	private byte[] _data = [];
	private Dictionary<string, (byte[] privateMeta, byte[] publicMeta, long lenght, byte? fillValue)> _files = new();
	private IInboxApi _inboxApi = inboxApi;
	private string? _privateKey;

	public InboxEntryWriterBuilder SetData(byte[] data)
	{
		_privateKey = null;
		_data = data;
		return this;
	}

	public InboxEntryWriterBuilder SetPrivateKey(string privateKey)
	{
		_privateKey = privateKey;
		return this;
	}

	public InboxEntryWriterBuilder AddFile(string knownName, byte[] publicMeta, byte[] privateMeta, long lenght, byte? fillValue = null)
	{
		if (!_files.TryAdd(knownName, (privateMeta, publicMeta, lenght, fillValue)))
			throw new InvalidOperationException($"Filed with known name: {knownName}, already exists");
		return this;
	}

	public ValueTask<ManagedInboxEntryWriter> BuildAsync(CancellationToken token = default)
	{
		return PrepareEntryAsync(inboxApi, inboxId, _data, _privateKey, _files, token);
	}

	private static async ValueTask<ManagedInboxEntryWriter> PrepareEntryAsync(IInboxApi inboxApi, string inboxId, byte[] data, string? privateKye, Dictionary<string, (byte[] privateMeta, byte[] publicMeta, long lenght, byte? fillValue)> files, CancellationToken token = default)
	{
		List<long> handles = new();
		foreach (var (_, file) in files)
		{
			var handle = await inboxApi.CreateFileHandleAsync(file.publicMeta, file.privateMeta, file.lenght, token);
			handles.Add(handle);
		}
		var entryHandle = await inboxApi.PrepareEntryAsync(inboxId, data, handles, privateKye!, token);
		var streams = new Dictionary<string, InboxWriteFileStream>();
		foreach (var (name, privateMeta, publicMeta, lenght, fillValue, handle) in files.Zip(handles, (pair, handle) => (pair.Key, pair.Value.privateMeta, pair.Value.publicMeta, pair.Value.lenght, pair.Value.fillValue, l: handle)))
		{
			streams.Add(name, new InboxWriteFileStream(null, entryHandle, handle, lenght, publicMeta, privateMeta, fillValue, inboxApi));
		}
		return new ManagedInboxEntryWriter(inboxId, entryHandle, streams, inboxApi);
	}
}