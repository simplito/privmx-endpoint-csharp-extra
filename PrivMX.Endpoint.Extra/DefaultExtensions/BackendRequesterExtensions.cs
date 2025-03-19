// Module name: PrivmxEndpointCsharpExtra
// File name: BackendRequesterExtensions.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Extra.Internals;

namespace PrivMX.Endpoint.Extra;

/// <summary>
///     Async extension methods for IBackendRequester interface.
/// </summary>
public static class BackendRequesterAsyncExtensions
{
	/// <summary>
	///     Sends a request to PrivMX Bridge API using access token for authorization.
	/// </summary>
	/// <param name="backendRequester">Extended object.</param>
	/// <param name="serverUrl">PrivMX Bridge server URL.</param>
	/// <param name="accessToken">Token for authorization (see PrivMX Bridge API for more details).</param>
	/// <param name="method">API method to call.</param>
	/// <param name="paramsAsJson">API method's parameters in JSON format.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>JSON string representing raw server response.</returns>
	public static ValueTask<string> BackendRequestAsync(this IBackendRequester backendRequester, string serverUrl,
		string accessToken, string method, string paramsAsJson, CancellationToken token = default)
	{
		if (backendRequester is null)
			throw new ArgumentNullException(nameof(backendRequester));
		return WrapperCallsExecutor.Execute(
			() => backendRequester.BackendRequest(serverUrl, accessToken, method, paramsAsJson), token);
	}

	/// <summary>
	///     Sends request to PrivMX Bridge API.
	/// </summary>
	/// <param name="backendRequester">Extended object.</param>
	/// <param name="serverUrl">PrivMX Bridge server URL.</param>
	/// <param name="method">API method to call.</param>
	/// <param name="paramsAsJson">API method's parameters in JSON format.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>JSON string representing raw server response.</returns>
	public static ValueTask<string> BackendRequestAsync(this IBackendRequester backendRequester, string serverUrl,
		string method, string paramsAsJson, CancellationToken token = default)
	{
		if (backendRequester is null)
			throw new ArgumentNullException(nameof(backendRequester));
		return WrapperCallsExecutor.Execute(() => backendRequester.BackendRequest(serverUrl, method, paramsAsJson),
			token);
	}

	/// <summary>
	///     Sends a request to PrivMX Bridge API using pair of API KEY ID and API KEY SECRET for authorization.
	/// </summary>
	/// <param name="backendRequester">Extended object.</param>
	/// <param name="serverUrl">PrivMX Bridge server URL.</param>
	/// <param name="apiKeyId">API KEY ID (see PrivMX Bridge API for more details).</param>
	/// <param name="apiKeySecret">API KEY SECRET (see PrivMX Bridge API for more details).</param>
	/// <param name="mode">Allows you to set whether the request should be signed (mode = 1) or plain (mode = 0).</param>
	/// <param name="method">API method to call.</param>
	/// <param name="paramsAsJson">API method's parameters in JSON format.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>JSON string representing raw server response.</returns>
	public static ValueTask<string> BackendRequestAsync(this IBackendRequester backendRequester, string serverUrl,
		string apiKeyId, string apiKeySecret, long mode, string method, string paramsAsJson,
		CancellationToken token = default)
	{
		if (backendRequester is null)
			throw new ArgumentNullException(nameof(backendRequester));
		return WrapperCallsExecutor.Execute(
			() => backendRequester.BackendRequest(serverUrl, apiKeyId, apiKeySecret, mode, method, paramsAsJson),
			token);
	}
}