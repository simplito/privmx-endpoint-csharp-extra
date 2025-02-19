// Module name: PrivmxEndpointCsharpExtra
// File name: StoreReadonlyFileStream.cs
// Last edit: 2025-02-19 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Store;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Store;

/// <summary>
///     Stream that reads data from store file.
///     This class is not thread safe.
/// </summary>
public sealed class StoreReadonlyFileStream : Stream
{
	private static readonly Logger.SourcedLogger<StoreReadonlyFileStream> Logger = default;
	private readonly long _fileHandle;
	private DisposeBool _disposed;
	private long _position;
	internal byte[] _privateMeta;
	internal byte[] _publicMeta;
	private IStoreApi _storeApi;

	internal StoreReadonlyFileStream(long size, long fileHandle, IStoreApi storeApi, byte[] publicMeta,
		byte[] privateMeta)
	{
		_storeApi = storeApi;
		_publicMeta = publicMeta;
		_privateMeta = privateMeta;
		_fileHandle = fileHandle;
		Length = size;
	}

	public ReadOnlySpan<byte> PublicMeta => _publicMeta;
	public ReadOnlySpan<byte> PrivateMeta => _privateMeta;

	public override bool CanRead => !_disposed;
	public override bool CanSeek => !_disposed;
	public override bool CanWrite => false;
	public override long Length { get; }

	public override long Position
	{
		get => _position;
		set => SetPosition(value);
	}

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