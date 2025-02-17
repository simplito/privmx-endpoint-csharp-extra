// Module name: InternalTools
// File name: SynchronizationContextAwaiter.cs
// Last edit: 2025-02-17 08:48 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Internal;

public readonly struct SynchronizationContextAwaiter : INotifyCompletion
{
	private static readonly SendOrPostCallback PostCallback = state => ((Action)state)();

	private readonly SynchronizationContext _context;

	public SynchronizationContextAwaiter(SynchronizationContext context)
	{
		_context = context;
	}

	public bool IsCompleted => _context == SynchronizationContext.Current;

	public void OnCompleted(Action continuation)
	{
		_context.Post(PostCallback, continuation);
	}

	public void GetResult()
	{
	}
}