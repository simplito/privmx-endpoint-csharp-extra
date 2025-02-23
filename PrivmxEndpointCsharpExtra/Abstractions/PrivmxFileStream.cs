// Module name: PrivmxEndpointCsharpExtra
// File name: PrivmxFileStream.cs
// Last edit: 2025-02-23 12:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

namespace PrivmxEndpointCsharpExtra.Abstractions;

public abstract class PrivmxFileStream : Stream
{
	public abstract string? FileId { get; } 
	public abstract ReadOnlySpan<byte> PublicMeta { get; }
	public abstract ReadOnlySpan<byte> PrivateMeta { get; }
}