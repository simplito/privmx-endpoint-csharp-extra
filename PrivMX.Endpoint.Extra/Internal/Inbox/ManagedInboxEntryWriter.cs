// Module name: PrivmxEndpointCsharpExtra
// File name: ManagedInboxEntryWriter.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Inbox;

namespace PrivMX.Endpoint.Extra.Inbox;

/// <summary>
///     Represents ongoing write operation for a single entry in the inbox.
/// </summary>
public sealed class ManagedInboxEntryWriter : IAsyncDisposable
{
	private readonly long _entryId;
	private readonly Logger.SourcedLogger<ManagedInboxEntryWriter> Logger = default;
	private DisposeBool _dispsed;
	private IInboxApi _inboxApi;
	private readonly string _inboxId;

	internal ManagedInboxEntryWriter(string inboxId, long entryId,
		Dictionary<string, InboxWriteFileStream> asyncInboxApi, IInboxApi inboxApi)
	{
		_inboxId = inboxId;
		FileStreams = asyncInboxApi;
		_inboxApi = inboxApi;
		_entryId = entryId;
	}

	/// <summary>
	///     Mapping of user selected usernames to file streams.
	/// </summary>
	public IReadOnlyDictionary<string, InboxWriteFileStream> FileStreams { get; }

	/// <summary>
	///     Finishes writing the entry and sends it to the inbox.
	/// </summary>
	/// <exception cref="AggregateException">Thrown when api operations throw exception. Contains all aggregated exceptions.</exception>
	public async ValueTask DisposeAsync()
	{
		if (!_dispsed.PerformDispose())
			return;
		Logger.Log(LogLevel.Trace, "Disposing managed inbox entry writer, inboxId: {0}.", _inboxId);
		List<Exception>? exceptions = null;
		foreach (var value in FileStreams.Values)
			try
			{
				await value.DisposeAsync();
			}
			catch (Exception exception)
			{
				(exceptions ??= new List<Exception>()).Add(exception);
			}

		if (exceptions is not null)
		{
			_inboxApi = null!;
			throw new AggregateException(exceptions);
		}

		await _inboxApi.SendEntryAsync(_entryId);
	}
}