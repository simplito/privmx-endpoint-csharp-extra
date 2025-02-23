// Module name: PrivmxEndpointCsharpExtra
// File name: ManagedInboxEntry.cs
// Last edit: 2025-02-23 12:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Inbox;
using PrivmxEndpointCsharpExtra.Api;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Inbox;

public sealed class ManagedInboxEntryWriter : IAsyncDisposable
{
	private readonly Logger.SourcedLogger<ManagedInboxEntryWriter> Logger = default;
	private DisposeBool _dispsed;
	private string _inboxId;
	private readonly AsyncInboxApi _api;
	private readonly long _entryId;
	private IInboxApi _inboxApi;
	public IReadOnlyDictionary<string, InboxWriteFileStream> FileStreams { get; } 
	public ManagedInboxEntryWriter(string inboxId, long entryId, Dictionary<string, InboxWriteFileStream> asyncInboxApi, IInboxApi inboxApi)
	{
		_inboxId = inboxId;
		FileStreams = asyncInboxApi;
		_inboxApi = inboxApi;
		_entryId = entryId;
	}

	public async ValueTask DisposeAsync()
	{
		if(!_dispsed.PerformDispose())
			return;
		Logger.Log(LogLevel.Trace, "Disposing managed inbox entry writer, inboxId: {0}.", _inboxId);
		List<Exception>? exceptions = null;
		foreach (var value in FileStreams.Values)
		{
			try
			{
				await value.DisposeAsync();
			}
			catch (Exception exception)
			{
				(exceptions ??= new List<Exception>()).Add(exception);
			}
		}

		if (exceptions is not null)
		{
			_inboxApi = null!;
			throw new AggregateException(exceptions);
		}
		await _inboxApi.SendEntryAsync(_entryId);
	}
}