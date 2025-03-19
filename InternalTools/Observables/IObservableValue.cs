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