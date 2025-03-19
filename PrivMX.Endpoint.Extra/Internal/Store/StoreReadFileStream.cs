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
using PrivMX.Endpoint.Store;

namespace PrivMX.Endpoint.Extra.Store;

/// <summary>
///     Stream that reads data from store file.
///     This class is not thread safe.
/// </summary>
internal sealed class StoreReadonlyFileStream : PrivmxFileStream
{
	private static readonly Logger.SourcedLogger<StoreReadonlyFileStream> Logger = default;
	private readonly long _fileHandle;
	private readonly byte[] _publicMeta;
	private DisposeBool _disposed;
	private long _position;
	private byte[] _privateMeta;
	private IStoreApi _storeApi;

	internal StoreReadonlyFileStream(string fileId, long size, long fileHandle, IStoreApi storeApi, byte[] publicMeta,
		byte[] privateMeta)
	{
		_storeApi = storeApi;
		_fileHandle = fileHandle;
		Length = size;
		_publicMeta = publicMeta;
		_privateMeta = privateMeta;
		FileId = fileId;
	}

	public override bool CanRead => !_disposed;
	public override bool CanSeek => !_disposed;
	public override bool CanWrite => false;
	public override long Length { get; }

	public override long Position
	{
		get => _position;
		set => SetPosition(value);
	}

	public override string? FileId { get; }
	public override ReadOnlySpan<byte> PublicMeta => _publicMeta;
	public override ReadOnlySpan<byte> PrivateMeta => _publicMeta;

	public override void Flush()
	{
		// for now file is unbuffered, so no need to flush
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		_disposed.ThrowIfDisposed(nameof(StoreReadonlyFileStream));
		var read = _storeApi.ReadFromFile(_fileHandle, Math.Min(count, Length - _position));
		_position += read.Length;
		Array.Copy(read, 0, buffer, offset, read.Length);
		return read.Length;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		_disposed.ThrowIfDisposed(nameof(StoreReadonlyFileStream));
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

	public override void SetLength(long value)
	{
		throw new NotSupportedException("SetLength is not supported for read only streams.");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("Writing is not supported for read only streams.");
	}

	private void SetPosition(long position)
	{
		_disposed.ThrowIfDisposed(nameof(StoreReadonlyFileStream));
		_storeApi.SeekInFile(_fileHandle, position);
		_position = position;
	}

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