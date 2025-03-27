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

using Internal;
using PrivMX.Endpoint.Extra.Abstractions;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Inbox;

namespace PrivMX.Endpoint.Extra.Inbox;

/// <summary>
///     Stream that reads data from store file.
///     This class is not thread safe.
/// </summary>
internal sealed class InboxReadFileStream : PrivmxFileStream
{
	private static readonly Logger.SourcedLogger<InboxReadFileStream> Logger = default;
	private readonly long _fileHandle;
	private readonly byte[] _privateMeta;
	private readonly byte[] _publicMeta;
	private DisposeBool _disposed;
	private long _position;
	private IInboxApi _storeApi;

	internal InboxReadFileStream(long size, long fileHandle, IInboxApi storeApi, byte[] publicMeta,
		byte[] privateMeta)
	{
		_storeApi = storeApi;
		_publicMeta = publicMeta;
		_privateMeta = privateMeta;
		_fileHandle = fileHandle;
		Length = size;
	}

	public override string? FileId { get; }
	public override ReadOnlySpan<byte> PublicMeta => _publicMeta;
	public override ReadOnlySpan<byte> PrivateMeta => _privateMeta;

	/// <summary>
	///     Returns if current stream supports reading.
	/// </summary>
	public override bool CanRead => !_disposed;

	/// <summary>
	///     Returns if current stream supports seeking.
	/// </summary>
	public override bool CanSeek => !_disposed;

	/// <summary>
	///     Returns if current stream supports writing.
	/// </summary>
	public override bool CanWrite => false;

	/// <summary>
	///     Returns total length of the stream.
	/// </summary>
	public override long Length { get; }

	/// <summary>
	///     Sets or gets the position with the stream.
	/// </summary>
	public override long Position
	{
		get => _position;
		set => SetPosition(value);
	}

	/// <summary>
	///     Does nothing.
	/// </summary>
	public override void Flush()
	{
		// for now file is unbuffered, so no need to flush
	}

	/// <inheritdoc />
	public override int Read(byte[] buffer, int offset, int count)
	{
		_disposed.ThrowIfDisposed(nameof(InboxReadFileStream));
		var read = _storeApi.ReadFromFile(_fileHandle, Math.Min(count, Length - _position));
		_position += read.Length;
		Array.Copy(read, 0, buffer, offset, read.Length);
		return read.Length;
	}

	/// <inheritdoc />
	public override long Seek(long offset, SeekOrigin origin)
	{
		_disposed.ThrowIfDisposed(nameof(InboxReadFileStream));
		switch (origin)
		{
			case SeekOrigin.Begin:
				SetPosition(offset);
				return _position;
			case SeekOrigin.Current:
				SetPosition(_position + offset);
				return _position;
			case SeekOrigin.End:
				SetPosition(Length - offset);
				return _position;
			default:
				throw new Exception("Unreachable code");
		}
	}

	/// <summary>
	///     Operation not supported
	/// </summary>
	/// <param name="value">Ignored.</param>
	/// <exception cref="NotSupportedException">Always thrown.</exception>
	public override void SetLength(long value)
	{
		throw new NotSupportedException("SetLength is not supported for read only streams.");
	}

	/// <summary>
	///     Opertation not supported.
	/// </summary>
	/// <param name="buffer">Ignored.</param>
	/// <param name="offset">Ignored.</param>
	/// <param name="count">Ignored.</param>
	/// <exception cref="NotSupportedException">Always thrown.</exception>
	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("Writing is not supported for read only streams.");
	}

	private void SetPosition(long position)
	{
		_disposed.ThrowIfDisposed(nameof(InboxReadFileStream));
		_storeApi.SeekInFile(_fileHandle, position);
		_position = position;
	}

	/// <summary>
	///     Disposes the stream and closes the file.
	/// </summary>
	/// <param name="disposing">If true then dispose operation is performed by the user.</param>
	protected override void Dispose(bool disposing)
	{
		if (!_disposed.PerformDispose())
			return;
		try
		{
			_storeApi.CloseFile(_fileHandle);
		}
		catch (Exception exception)
		{
			if (disposing)
				throw;
			Logger.Log(LogLevel.Error, "Unhandled exception during dispose.", exception);
			Internals.Logger.PublishUnobservedException(exception);
		}
		finally
		{
			_storeApi = null!;
		}

		base.Dispose(disposing);
	}

	/// <summary>
	///     Disposes stream asynchronously.
	/// </summary>
	public override async ValueTask DisposeAsync()
	{
		if (!_disposed.PerformDispose())
			return;
		try
		{
			await _storeApi.CloseFileAsync(_fileHandle);
		}
		catch (Exception exception)
		{
			Logger.Log(LogLevel.Error, "Unhandled exception during dispose.", exception);
			Internals.Logger.PublishUnobservedException(exception);
		}
		finally
		{
			_storeApi = null!;
		}
	}
}