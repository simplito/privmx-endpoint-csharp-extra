// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncInboxApi.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Extra.Abstractions;
using PrivMX.Endpoint.Extra.Events;
using PrivMX.Endpoint.Extra.Inbox;
using PrivMX.Endpoint.Inbox.Models;

namespace PrivMX.Endpoint.Extra.Api.Interfaces;

/// <summary>
///     Interface representing async inbox API.
/// </summary>
public interface IAsyncInboxApi
{
	/// <summary>
	///     Creates a new Inbox.
	/// </summary>
	/// <param name="contextId">ID of the Context of the new Inbox.</param>
	/// <param name="users">Vector of UserWithPubKey structs which indicates who will have access to the created Inbox.</param>
	/// <param name="managers">
	///     Vector of UserWithPubKey structs which indicates who will have access (and management rights) to
	///     the created Inbox.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) metadata.</param>
	/// <param name="privateMeta">Private (encrypted) metadata.</param>
	/// <param name="filesConfig">Optional configuration of files.</param>
	/// <param name="policies">(optional) Inbox policy.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>ID of the created Inbox.</returns>
	ValueTask<string> CreateInboxAsync(string contextId, List<UserWithPubKey> users,
		List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig,
		ContainerPolicyWithoutItem? policies = null, CancellationToken token = default);

	/// <summary>
	///     Updates an existing Inbox.
	/// </summary>
	/// <param name="inboxId">ID of the Inbox to update.</param>
	/// <param name="users">Vector of UserWithPubKey structs which indicates who will have access to the created Inbox.</param>
	/// <param name="managers">
	///     Vector of UserWithPubKey structs which indicates who will have access (and management rights) to
	///     the created Inbox.
	/// </param>
	/// <param name="publicMeta">Public (unencrypted) metadata.</param>
	/// <param name="privateMeta">Private (encrypted) metadata.</param>
	/// <param name="filesConfig">Optional configuration of files.</param>
	/// <param name="version">Current version of the updated Inbox.</param>
	/// <param name="force">Force update without checking version.</param>
	/// <param name="forceGenerateNewKey">Force to regenerate a key for the Inbox.</param>
	/// <param name="policies">(optional) Inbox policy.</param>
	/// <param name="token">Cancellation token.</param>
	ValueTask UpdateInboxAsync(string inboxId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
		byte[] publicMeta, byte[] privateMeta, FilesConfig filesConfig, long version, bool force,
		bool forceGenerateNewKey, ContainerPolicyWithoutItem? policies = null, CancellationToken token = default);

	/// <summary>
	///     Gets a Inbox by given Inbox ID.
	/// </summary>
	/// <param name="inboxId">ID of the Inbox to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Information about about the Inbox.</returns>
	ValueTask<PrivMX.Endpoint.Inbox.Models.Inbox> GetInboxAsync(string inboxId,
		CancellationToken token = default);

	/// <summary>
	///     Gets s list of Inboxes in given Context.
	/// </summary>
	/// <param name="contextId">ID of the Context to get Inboxes from.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of Inboxes.</returns>
	ValueTask<PagingList<PrivMX.Endpoint.Inbox.Models.Inbox>> ListInboxesAsync(string contextId,
		PagingQuery pagingQuery, CancellationToken token = default);

	/// <summary>
	///     Gets public data of an Inbox.
	///     You do not have to be logged in to call this function.
	/// </summary>
	/// <param name="inboxId">ID of the Inbox to get.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Public accessible information about the Inbox.</returns>
	ValueTask<InboxPublicView> GetInboxPublicViewAsync(string inboxId, CancellationToken token = default);

	/// <summary>
	///     Deletes an Inbox by given Inbox ID.
	/// </summary>
	/// <param name="inboxId">ID of the Inbox to delete.</param>
	/// <param name="token">Cancellation token</param>
	ValueTask DeleteInboxAsync(string inboxId, CancellationToken token = default);

	/// <summary>
	///     Returns entry builder that can be used to create new entries in the inbox.
	/// </summary>
	/// <param name="inboxId">ID of inbox where entry will be created.</param>
	/// <returns>Inbox entry builder.</returns>
	InboxEntryWriterBuilder GetEntryBuilder(string inboxId);

	/// <summary>
	///     Gets an entry from an Inbox.
	/// </summary>
	/// <param name="inboxEntryId">ID of an entry to read from the Inbox.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Data of the entry stored in the Inbox.</returns>
	ValueTask<InboxEntry> ReadEntryAsync(string inboxEntryId, CancellationToken token = default);

	/// <summary>
	///     Gets list of entries in given Inbox.
	/// </summary>
	/// <param name="inboxId">ID of the Inbox.</param>
	/// <param name="pagingQuery">List query parameters.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>List of entries.</returns>
	ValueTask<PagingList<InboxEntry>> ListEntriesAsync(string inboxId, PagingQuery pagingQuery,
		CancellationToken token = default);

	/// <summary>
	///     Delete an entry from an Inbox.
	/// </summary>
	/// <param name="inboxEntryId">ID of an entry to delete.</param>
	/// <param name="token">Cancellation token.</param>
	ValueTask DeleteEntryAsync(string inboxEntryId, CancellationToken token = default);

	/// <summary>
	///     Opens file for user to read.
	/// </summary>
	/// <param name="entry">Entry from which file will be opended.</param>
	/// <param name="fileId">ID of file to open.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Fixed size seekable and readable stream.</returns>
	ValueTask<PrivmxFileStream> OpenFileForRead(InboxEntry entry, string fileId, CancellationToken token = default);

	/// <summary>
	///     Get inbox events.
	/// </summary>
	/// <returns>Stream of inbox events.</returns>
	IObservable<InboxEvent> GetInboxEvents();

	/// <summary>
	///     Get events related to specific inbox entries.
	/// </summary>
	/// <param name="inboxId">ID of the inbox.</param>
	/// <returns>Stream of entries events.</returns>
	IObservable<InboxEntryEvent> GetEntryEvents(string inboxId);
}