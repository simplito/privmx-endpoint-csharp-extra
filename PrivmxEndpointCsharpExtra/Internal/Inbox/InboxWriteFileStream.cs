// Module name: PrivmxEndpointCsharpExtra
// File name: StoreWriteFileStream.cs
// Last edit: 2025-02-19 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Inbox;
using PrivmxEndpointCsharpExtra.Abstractions;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Inbox;
/// <summary>
/// Stream that represents remote file in the store.
/// All data is written to the store when file is closed/disposed.
/// This class is not thread safe.
/// </summary>
public sealed class InboxWriteFileStream : PrivmxFileStream
{
	private static readonly Logger.SourcedLogger<InboxWriteFileStream> Logger = default;
	private DisposeBool _disposed = new();
	private IInboxApi _storeApi;
	private readonly long _fileHandle;
	private readonly long _inboxHandle;
	private long _position;
	private readonly byte? _fillValue = null;
	private readonly byte[] _publicMeta;
	private readonly byte[] _privateMeta;
	private string? _fileId;

	/// <summary>
	/// File ID. May be null if file is not yet created.
	/// May change after object disposal when the file is closed and saved on the server.
	/// </summary>
	public override string? FileId => _fileId;

	public override ReadOnlySpan<byte> PublicMeta => _publicMeta;
	public override ReadOnlySpan<byte> PrivateMeta => _privateMeta;

	internal InboxWriteFileStream(string? fileId, long inboxHandle, long fileHandle, long size, byte[] publicMeta, byte[] privateMeta, byte? fillValue, IInboxApi storeApi)
	{
		Logger.Log(LogLevel.Trace, "Creating new StoreWriteFileStream, handle: {0}, fileId: {1}, fileLength: {2}", fileId, fileId, size);
		_fileHandle = fileHandle;
		_inboxHandle = inboxHandle;
		_fileId = fileId;
		Length = size;
		_storeApi = storeApi;
		_publicMeta = publicMeta;
		_privateMeta = privateMeta;
		_fillValue = fillValue;
	}

	public override void Flush()
	{
		
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException($"{nameof(InboxWriteFileStream)} is write only stream.");
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException($"{nameof(InboxWriteFileStream)} is not seekable.");
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException($"{nameof(InboxWriteFileStream)} has fixed length.");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_position += count;
		_storeApi.WriteToFile(_inboxHandle, _fileHandle, buffer[offset..(offset + count)]);
	}

	public void Fill(byte value = 0x0)
	{
		var array = new byte[Length - _position];
		Array.Fill(array, value);
		_storeApi.WriteToFile(_inboxHandle, _fileHandle, array);
		_position = Length;                 
	}

	public async ValueTask FillAsync(byte value = 0x0, CancellationToken token = default)
	{
		var array = new byte[Length - _position];
		Array.Fill(array, value);
		await _storeApi.WriteToFileAsync(_inboxHandle, _fileHandle, array, token);
		_position = Length;
	}

	protected override void Dispose(bool disposing)
	{          
		if(!_disposed.PerformDispose())
			return;
		try
		{
			if (_fillValue.HasValue)
				Fill(_fillValue.Value);
			// _fileId = _storeApi.CloseFile(_fileHandle);
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
		if(!_disposed.PerformDispose())
			return;
		try
		{
			if (_fillValue.HasValue)
				await FillAsync(_fillValue.Value);
			// _fileId = await _storeApi.CloseFileAsync(_fileHandle);
		}
		finally
		{
			_storeApi = null!;
		}
	}

	public override bool CanRead => false;
	public override bool CanSeek => false;
	public override bool CanWrite => !_disposed;
	public override long Length { get; }

	public override long Position
	{
		get => _position;
		set => throw new NotSupportedException($"{nameof(InboxWriteFileStream)} is not seekable.");
	}
}