// Module name: InternalTools
// File name: CancellationTokenExtensions.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;
using System.Threading;

namespace Internal.Extensions;

public static class CancellationTokenExtensions
{
	public static IDisposable LinkIfNeeded(this CancellationToken token1, CancellationToken token2,
		out CancellationToken linkedToken)
	{
		if (!token1.CanBeCanceled)
		{
			linkedToken = token2;
			return NullDisposable.Instance;
		}

		if (!token2.CanBeCanceled)
		{
			linkedToken = token1;
			return NullDisposable.Instance;
		}

		var cts = CancellationTokenSource.CreateLinkedTokenSource(token1, token2);
		linkedToken = cts.Token;
		return cts;
	}
}