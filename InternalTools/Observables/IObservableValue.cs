// Module name: InternalTools
// File name: IObservableValue.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using System;

namespace Internal.Observables;

/// <summary>
///     An observable that also stores last observed value.
/// </summary>
/// <typeparam name="T">Type of the observed value.</typeparam>
public interface IObservableValue<out T> : IObservable<T>
{
	/// <summary>
	///     Current value.
	/// </summary>
	public T Value { get; }
}