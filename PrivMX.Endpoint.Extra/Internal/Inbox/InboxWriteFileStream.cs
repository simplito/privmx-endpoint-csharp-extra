// Module name: PrivmxEndpointCsharpExtra
// File name: InboxWriteFileStream.cs
// Last edit: 2025-02-24 21:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Extra.Abstractions;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Inbox;

namespace PrivMX.Endpoint.Extra.Inbox;

/// <summary>
///     Stream that represents remote file in the store.
///     All data is written to the store when file is closed/disposed.
///     This class is not thread safe.
/// </summary>
public sealed class InboxWriteFileStream : PrivmxFileStream
{
	private static readonly Logger.SourcedLogger<InboxWriteFileStream> Logger = default;
	private readonly long _fileHandle;
	private readonly byte? _fillValue;
	private readonly long _inboxHandle;
	private readonly byte[] _privateMeta;
	private readonly byte[] _publicMeta;
	private DisposeBool _disposed;
	private long _position;
	private IInboxApi _storeApi;

	internal InboxWriteFileStream(string? fileId, long inboxHandle, long fileHandle, long size, byte[] publicMeta,
		byte[] privateMeta, byte? fillValue, IInboxApi storeApi)
	{
		Logger.Log(LogLevel.Trace, "Creating new StoreWriteFileStream, handle: {0}, fileId: {1}, fileLength: {2}",
			fileId, fileId, size);
		_fileHandle = fileHandle;
		_inboxHandle = inboxHandle;
		FileId = fileId;
		Length = size;
		_storeApi = storeApi;
		_publicMeta = publicMeta;
		_privateMeta = privateMeta;
		_fillValue = fillValue;
	}

	/// <summary>
	///     File ID. May be null if file is not yet created.
	///     May change after object disposal when the file is closed and saved on the server.
	/// </summary>
	public override string? FileId { get; }

	/// <summary>
	///     File public metadata.
	/// </summary>
	public override ReadOnlySpan<byte> PublicMeta => _publicMeta;

	/// <summary>
	///     File private (encrypted) metadata.
	/// </summary>
	public override ReadOnlySpan<byte> PrivateMeta => _privateMeta;

	/// <summary>
	///     Always false/
	/// </summary>
	public override bool CanRead => false;

	/// <summary>
	///     Always false/
	/// </summary>
	public override bool CanSeek => false;

	/// <summary>
	///     True until file is disposed.
	/// </summary>
	public override bool CanWrite => !_disposed;

	/// <summary>
	///     Fixed length of the file.
	/// </summary>
	public override long Length { get; }

	/// <summary>
	///     Returns current position in the file.
	/// </summary>
	/// <exception cref="NotSupportedException">Thrown when user attempts to set caret position.</exception>
	public override long Position
	{
		get => _position;
		set => throw new NotSupportedException($"{nameof(InboxWriteFileStream)} is not seekable.");
	}

	/// <summary>
	///     Does nothing.
	/// </summary>
	public override void Flush()
	{
	}

	/// <summary>
	///     Unsupported operation.
	/// </summary>
	/// <exception cref="NotSupportedException">Always thrown.</exception>
	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException($"{nameof(InboxWriteFileStream)} is write only stream.");
	}

	/// <summary>
	///     Unsupported operation.
	/// </summary>
	/// <exception cref="NotSupportedException">Always thrown.</exception>
	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException($"{nameof(InboxWriteFileStream)} is not seekable.");
	}

	/// <summary>
	///     Unsupported operation.
	/// </summary>
	/// <exception cref="NotSupportedException">Always thrown.</exception>
	public override void SetLength(long value)
	{
		throw new NotSupportedException($"{nameof(InboxWriteFileStream)} has fixed length.");
	}

	/// <summary>
	///     Writes data to inbox file.
	/// </summary>
	/// <param name="buffer">Input buffer/</param>
	/// <param name="offset">Offset in buffer.</param>
	/// <param name="count">Number of bytes written.</param>
	public override void Write(byte[] buffer, int offset, int count)
	{
		_position += count;
		_storeApi.WriteToFile(_inboxHandle, _fileHandle, buffer[offset..(offset + count)]);
	}

	/// <summary>
	///     Fills rest of the file with given value.
	/// </summary>
	/// <param name="value">Value to fill.</param>
	public void Fill(byte value = 0x0)
	{
		var array = new byte[Length - _position];
		Array.Fill(array, value);
		_storeApi.WriteToFile(_inboxHandle, _fileHandle, array);
		_position = Length;
	}

	/// <summary>
	///     Fills rest of the file with given value.
	/// </summary>
	/// <param name="value">Value to fill.</param>
	/// <param name="token">Cancellation token</param>
	public async ValueTask FillAsync(byte value = 0x0, CancellationToken token = default)
	{
		var array = new byte[Length - _position];
		Array.Fill(array, value);
		await _storeApi.WriteToFileAsync(_inboxHandle, _fileHandle, array, token);
		_position = Length;
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (!_disposed.PerformDispose())
			return;
		try
		{
			if (_fillValue.HasValue)
				Fill(_fillValue.Value);
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

	/// <inheritdoc />
	public override async ValueTask DisposeAsync()
	{
		if (!_disposed.PerformDispose())
			return;
		try
		{
			if (_fillValue.HasValue)
				await FillAsync(_fillValue.Value);
		}
		finally
		{
			_storeApi = null!;
		}
	}
}