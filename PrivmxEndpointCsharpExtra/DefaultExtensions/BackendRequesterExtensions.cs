// Module name: PrivmxEndpointCsharpExtra
// File name: BackendRequesterExtensions.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra;

public static class BackendRequesterAsyncExtensions
{
	public static ValueTask<string> BackendRequestAsync(this IBackendRequester backendRequester, string serverUrl,
		string accessToken, string method, string paramsAsJson, CancellationToken token = default)
	{
		if (backendRequester is null)
			throw new ArgumentNullException(nameof(backendRequester));
		return WrapperCallsExecutor.Execute(
			() => backendRequester.BackendRequest(serverUrl, accessToken, method, paramsAsJson), token);
	}

	public static ValueTask<string> BackendRequestAsync(this IBackendRequester backendRequester, string serverUrl,
		string method, string paramsAsJson, CancellationToken token = default)
	{
		if (backendRequester is null)
			throw new ArgumentNullException(nameof(backendRequester));
		return WrapperCallsExecutor.Execute(() => backendRequester.BackendRequest(serverUrl, method, paramsAsJson),
			token);
	}

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