// Module name: PrivmxEndpointCsharpExtra
// File name: StoreFileStream.cs
// Last edit: 2025-02-17 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Store;
using File = PrivMX.Endpoint.Store.Models.File;

namespace PrivmxEndpointCsharpExtra.Api;
/// <summary>
/// Stream that read and writes data to a Store file.
/// This class is not thread safe.
/// </summary>
public sealed class StoreFileStream : Stream
{
	private DisposeBool _disposed;
	private File _serverFileInfo;
	private StoreApi _storeApi;
	private long _fileHandle;
	private long _position;
	private long _length;

	internal StoreFileStream(File serverFileInfo, long fileHandle, StoreApi storeApi)
	{
		_serverFileInfo = serverFileInfo;
		_storeApi = storeApi;
		_fileHandle = fileHandle;
	}
		 
	public override void Flush()
	{
		// for now file is unbuffered, so no need to flush
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		_disposed.ThrowIfDisposed(nameof(StoreFileStream));
		var read = _storeApi.ReadFromFile(_fileHandle, Math.Min(count, _length - _position));
		_position += read.Length;
		Array.Copy(read, read.Length, buffer, offset, read.Length);
		return read.Length;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		_disposed.ThrowIfDisposed(nameof(StoreFileStream));
		switch (origin)
		{
			case SeekOrigin.Begin:
				SetPosition(offset);
				return _position;
			case SeekOrigin.Current:
				SetPosition(_position + offset);
				return _position;
			case SeekOrigin.End:
				SetPosition(_serverFileInfo.Size - offset);
				return _position;
			default:
				throw new Exception("Unreachable code");
		}

	}

	public override void SetLength(long value)
	{
		_disposed.ThrowIfDisposed(nameof(StoreFileStream));
		_fileHandle = _storeApi.UpdateFile(_serverFileInfo.Info.FileId, _serverFileInfo.PublicMeta,
			_serverFileInfo.PrivateMeta, value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_disposed.ThrowIfDisposed(nameof(StoreFileStream));
		_storeApi.WriteToFile(_fileHandle, buffer[offset..count]);
		_position += count;
	}

	private void SetPosition(long position)
	{
		_disposed.ThrowIfDisposed(nameof(StoreFileStream));
		_storeApi.SeekInFile(_fileHandle, position);
		_position = position;
	}



	public override bool CanRead => !_disposed;
	public override bool CanSeek => !_disposed;
	public override bool CanWrite => !_disposed;
	public override long Length => _length;

	public override long Position
	{
		get => _position;
		set => SetPosition(value);
	}
}