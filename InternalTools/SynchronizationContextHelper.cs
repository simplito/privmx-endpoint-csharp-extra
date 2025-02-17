// Module name: InternalTools
// File name: SynchronizationContextHelper.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.Threading;

namespace Internal;

public static class SynchronizationContextHelper
{
	public static SynchronizationContext ThreadPoolSynchronizationContext { get; } = new();
}