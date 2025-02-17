// Module name: InternalTools
// File name: ChangeList.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System.Collections.Generic;

namespace Internal;

internal sealed class ChangeList : List<(object obj, ChangeList.Change change)>
{
	public enum Change
	{
		Added,
		Removed
	}

	public ChangeList(int capacity) : base(capacity)
	{
	}
}