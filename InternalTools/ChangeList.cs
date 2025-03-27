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

using System.Collections.Generic;

namespace Internal;

internal sealed class ChangeList : List<(object obj, ChangeList.Change change)>
{
	public enum Change
	{
		Added,
		Removed
	}

	public ChangeList(int capacity) : base(capacity)
	{
	}
}