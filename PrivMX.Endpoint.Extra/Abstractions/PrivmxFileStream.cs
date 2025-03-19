// Module name: PrivmxEndpointCsharpExtra
// File name: PrivmxFileStream.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

namespace PrivMX.Endpoint.Extra.Abstractions;

/// <summary>
///     Abstract class representing a Privmx file stream.
/// </summary>
public abstract class PrivmxFileStream : Stream
{
    /// <summary>
    ///     Gets the file identifier.
    ///     FileId may be null if the file is not yet uploaded.
    /// </summary>
    public abstract string? FileId { get; }

    /// <summary>
    ///     Gets the public metadata.
    /// </summary>
    public abstract ReadOnlySpan<byte> PublicMeta { get; }

    /// <summary>
    ///     Gets the private metadata.
    /// </summary>
    public abstract ReadOnlySpan<byte> PrivateMeta { get; }
}