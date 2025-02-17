// Module name: InternalTools
// File name: ObservableValue.cs
// Last edit: 2025-02-17 08:47 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;
using System.Collections.Generic;

namespace Internal.Observables;

public sealed class ObservableValue<T> : IObservableValue<T>, IDisposable
{
	private readonly InvokeObservable<T> _wrapped;
	private T _value;

	public ObservableValue(T initialValue) : this(initialValue, EqualityComparer<T>.Default, true)
	{
	}

	public ObservableValue(T initialValue, IEqualityComparer<T> comparer, bool unsubscribeOnError)
	{
		_value = initialValue;
		Comparer = comparer;
		_wrapped = new InvokeObservable<T>(unsubscribeOnError);
	}

	private IEqualityComparer<T> Comparer { get; }

	public void Dispose()
	{
		_wrapped.Dispose();
	}

	public IDisposable Subscribe(IObserver<T> observer)
	{
		return _wrapped.Subscribe(observer);
	}

	public T Value
	{
		get => _value;
		set
		{
			if (Comparer.Equals(value, _value))
				return;
			_value = value;
			InvokeValueHasChanged();
		}
	}

	public void InvokeValueHasChanged()
	{
		_wrapped.Send(_value);
	}
}