// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncThreadApi.cs
// Last edit: 2025-02-19 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Thread.Models;
using PrivmxEndpointCsharpExtra.Events;
using Thread = PrivMX.Endpoint.Thread.Models.Thread;

namespace PrivmxEndpointCsharpExtra.Api.Interfaces;

/// <summary>
/// Interface for asynchronous operations related to Threads in the PrivMX platform.
/// </summary>
public interface IAsyncThreadApi
{
    /// <summary>
    /// Creates a new Thread in the given Context.
    /// </summary>
    /// <param name="contextId">ID of the Context to create the Thread in.</param>
    /// <param name="users">List of <see cref="UserWithPubKey"/> indicating who will have access to the created Thread.</param>
    /// <param name="managers">List of <see cref="UserWithPubKey"/> indicating who will have access (and management rights) to the created Thread.</param>
    /// <param name="publicMeta">Public (unencrypted) meta data.</param>
    /// <param name="privateMeta">Private (encrypted) meta data.</param>
    /// <param name="containerPolicy">(optional) Thread policy.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>ID of the created Thread.</returns>
    ValueTask<string> CreateThreadAsync(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, ContainerPolicy? containerPolicy = null, CancellationToken token = default);

    /// <summary>
    /// Updates an existing Thread.
    /// </summary>
    /// <param name="threadId">ID of the Thread to update.</param>
    /// <param name="users">List of <see cref="UserWithPubKey"/> indicating who will have access to the Thread.</param>
    /// <param name="managers">List of <see cref="UserWithPubKey"/> indicating who will have access (and management rights) to the Thread.</param>
    /// <param name="publicMeta">Public (unencrypted) meta data.</param>
    /// <param name="privateMeta">Private (encrypted) meta data.</param>
    /// <param name="version">Current version of the Thread.</param>
    /// <param name="force">Force update (without checking version).</param>
    /// <param name="forceGenerateNewKey">Force to regenerate a key for the Thread.</param>
    /// <param name="containerPolicy">(optional) Thread policy.</param>
    /// <param name="token">Cancellation token.</param>
    ValueTask UpdateThreadAsync(string threadId, List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, long version, bool force, bool forceGenerateNewKey, ContainerPolicy? containerPolicy = null, CancellationToken token = default);

    /// <summary>
    /// Deletes a Thread by given Thread ID.
    /// </summary>
    /// <param name="threadId">ID of the Thread to delete.</param>
    /// <param name="token">Cancellation token.</param>
    ValueTask DeleteThreadAsync(string threadId, CancellationToken token = default);

    /// <summary>
    /// Gets a Thread by given Thread ID.
    /// </summary>
    /// <param name="threadId">ID of the Thread to get.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Information about the Thread.</returns>
    ValueTask<Thread> GetThreadAsync(string threadId, CancellationToken token = default);

    /// <summary>
    /// Gets a list of Threads in the given Context.
    /// </summary>
    /// <param name="contextId">ID of the Context to get the Threads from.</param>
    /// <param name="pagingQuery">List query parameters.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>List of Threads.</returns>
    ValueTask<PagingList<Thread>> ListThreadsAsync(string contextId, PagingQuery pagingQuery, CancellationToken token = default);

    /// <summary>
    /// Gets a message by given message ID.
    /// </summary>
    /// <param name="messageId">ID of the message to get.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Message.</returns>
    ValueTask<Message> GetMessageAsync(string messageId, CancellationToken token = default);

    /// <summary>
    /// Gets a list of messages from a Thread.
    /// </summary>
    /// <param name="threadId">ID of the Thread to list messages from.</param>
    /// <param name="pagingQuery">List query parameters.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>List of messages.</returns>
    ValueTask<PagingList<Message>> ListMessagesAsync(string threadId, PagingQuery pagingQuery, CancellationToken token = default);

    /// <summary>
    /// Sends a message in a Thread.
    /// </summary>
    /// <param name="threadId">ID of the Thread to send the message to.</param>
    /// <param name="publicMeta">Public message metadata.</param>
    /// <param name="privateMeta">Private message metadata.</param>
    /// <param name="data">Content of the message.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>ID of the new message.</returns>
    ValueTask<string> SendMessageAsync(string threadId, byte[] publicMeta, byte[] privateMeta, byte[] data, CancellationToken token = default);

    /// <summary>
    /// Updates a message in a Thread.
    /// </summary>
    /// <param name="messageId">ID of the message to update.</param>
    /// <param name="publicMeta">Public message metadata.</param>
    /// <param name="privateMeta">Private message metadata.</param>
    /// <param name="data">Content of the message.</param>
    /// <param name="token">Cancellation token.</param>
    ValueTask UpdateMessageAsync(string messageId, byte[] publicMeta, byte[] privateMeta, byte[] data, CancellationToken token = default);

    /// <summary>
    /// Deletes a message by given message ID.
    /// </summary>
    /// <param name="messageId">ID of the message to delete.</param>
    /// <param name="token">Cancellation token.</param>
    ValueTask DeleteMessageAsync(string messageId, CancellationToken token = default);

    /// <summary>
    /// Gets threads events.
    /// </summary>
    /// <returns>Stream of Thread events.</returns>
    IObservable<ThreadEvent> GetThreadEvents();

    /// <summary>
    /// Gets the events in a given Thread.
    /// </summary>
    /// <param name="threadId">ID of the Thread to get events from.</param>
    /// <returns>Stream of Thread message events.</returns>
    IObservable<ThreadMessageEvent> GetThreadMessageEvents(string threadId);
}