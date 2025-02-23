// Module name: PrivmxEndpointCsharpExtra
// File name: IAsyncConnection.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;

namespace PrivmxEndpointCsharpExtra.Api.Interfaces;

/// <summary>
///     Interface representing an asynchronous connection.
/// </summary>
public interface IAsyncConnection
{
	/// <summary>
	///     Gets the ID of the current connection.
	/// </summary>
	/// <returns>ID of the connection.</returns>
	public long GetConnectionId();

	/// <summary>
	///     Gets a list of Contexts available for the user.
	/// </summary>
	/// <param name="pagingQuery">List query parameters</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a paging list of contexts.</returns>
	ValueTask<PagingList<Context>> ListContexts(
		PagingQuery pagingQuery, CancellationToken token = default);
}