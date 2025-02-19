// Module name: PrivmxEndpointCsharpExtra
// File name: StoreWriteFileStream.cs
// Last edit: 2025-02-19 22:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using Internal;
using PrivMX.Endpoint.Store;
using PrivmxEndpointCsharpExtra.Internals;

namespace PrivmxEndpointCsharpExtra.Store;
/// <summary>
/// Stream that represents remote file in the store.
/// All data is written to the store when file is closed/disposed.
/// This class is not thread safe.
/// </summary>
public sealed class StoreWriteFileStream : Stream
{
	private static readonly Logger.SourcedLogger<StoreWriteFileStream> Logger = default;
	private DisposeBool _disposed = new();
	private readonly MemoryStream _memoryStream;
	private IStoreApi _storeApi;
	private long _fileHandle;
	private byte[] _privateMeta;
	private byte[] _publicMeta;
	private readonly string? _storeId;
	/// <summary>
	/// File ID. May be null if file is not yet created.
	/// May change after object disposal when data is saved on the server.
	/// </summary>
	public string? FileId { get; private set; }
	public byte[] PublicMeta
	{
		get => _publicMeta;
		set
		{
			_disposed.ThrowIfDisposed(nameof(StoreWriteFileStream));
			_publicMeta = value ?? throw new ArgumentNullException(nameof(value));
		}
	}
	
	public byte[] PrivateMeta
	{
		get => _privateMeta;
		set
		{
			_disposed.ThrowIfDisposed(nameof(StoreWriteFileStream));
			_privateMeta = value ?? throw new ArgumentNullException(nameof(value));
		}
	}


	internal StoreWriteFileStream(string? storeId, string? fileId, byte[] initialData, byte[] publicMeta, byte[] privateMeta, IStoreApi storeApi)
	{
		Logger.Log(LogLevel.Trace, "Creating new StoreWriteFileStream, storeId: {0}, fileId: {1}, initialDataLenght: {2}, publicMetaLenght: {3}, privateMetaLength: {4}", storeId, fileId, initialData.Length, publicMeta.Length, privateMeta.Length);
		_storeId = storeId;
		FileId = fileId;
		_publicMeta = publicMeta ?? throw new ArgumentNullException(nameof(publicMeta));
		_privateMeta = privateMeta ?? throw new ArgumentNullException(nameof(privateMeta));
		_memoryStream = new MemoryStream();
		_memoryStream.Write(initialData);
		_storeApi = storeApi;
	}

	public override void Flush()
	{
		
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return _memoryStream.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _memoryStream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		_memoryStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_memoryStream.Write(buffer, offset, count);
	}

	protected override void Dispose(bool disposing)
	{          
		if(!_disposed.PerformDispose())
			return;
		try
		{
			if (FileId is not null)
			{
				_fileHandle = _storeApi.UpdateFile(FileId, PublicMeta, PrivateMeta, _memoryStream.Length);
			}
			else
			{
				_fileHandle = _storeApi.CreateFile(_storeId, PublicMeta, PrivateMeta, _memoryStream.Length);
			}
			_storeApi.WriteToFile(_fileHandle, _memoryStream.GetBuffer()[..(int)_memoryStream.Length]);
			FileId = _storeApi.CloseFile(_fileHandle);
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
		if(!_disposed.PerformDispose())
			return;
		try
		{
			if (FileId is not null)
			{
				_fileHandle = await _storeApi.UpdateFileAsync(FileId, PublicMeta, PrivateMeta, _memoryStream.Length);
			}
			else
			{
				_fileHandle = await _storeApi.CreateFileAsync(_storeId!, PublicMeta, PrivateMeta, _memoryStream.Length);
			}
			await _storeApi.WriteToFileAsync(_fileHandle, _memoryStream.GetBuffer()[..(int)_memoryStream.Length]);
			FileId = await _storeApi.CloseFileAsync(_fileHandle);
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

	public override bool CanRead => !_disposed;
	public override bool CanSeek => !_disposed;
	public override bool CanWrite => !_disposed;

	public override long Length
	{
		get
		{
			_disposed.ThrowIfDisposed(nameof(StoreWriteFileStream));
			return _memoryStream.Length;
		}
	}

	public override long Position
	{
		get
		{
			_disposed.ThrowIfDisposed(nameof(StoreWriteFileStream));
			return _memoryStream.Position;
		}
		set
		{
			_disposed.ThrowIfDisposed(nameof(StoreWriteFileStream));
			_memoryStream.Position = value;
		}
	}
}