// Module name: InternalTools
// File name: NullDisposable.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;

namespace Internal;

public sealed class NullDisposable : IDisposable
{
	private NullDisposable()
	{
	}

	public static IDisposable Instance { get; } = new NullDisposable();

	public void Dispose()
	{
	}
}