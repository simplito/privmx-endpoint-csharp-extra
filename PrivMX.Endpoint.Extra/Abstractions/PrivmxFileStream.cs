//
// PrivMX Endpoint C# Extra
// Copyright © 2024 Simplito sp. z o.o.
//
// This file is part of the PrivMX Platform (https://privmx.dev).
// This software is Licensed under the MIT License.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

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