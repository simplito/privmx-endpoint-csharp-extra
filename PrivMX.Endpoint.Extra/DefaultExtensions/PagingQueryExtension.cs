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

using PrivMX.Endpoint.Core.Models;

namespace PrivMX.Endpoint.Extra;

public static class PagingQueryExtension
{
	public enum Order
	{
		Ascending,
		Descending
	}

	public static PagingQuery DefaultQuery =>
		new PagingQuery().SetDefault(); // always return new copy because ListQuery is mutable

	public static PagingQuery SetOrder(this PagingQuery @object, Order order)
	{
		switch (order)
		{
			case Order.Ascending:
				@object.SortOrder = "asc";
				break;
			case Order.Descending:
				@object.SortOrder = "desc";
				break;
		}

		return @object;
	}

	public static PagingQuery SetDefault(this PagingQuery @object)
	{
		@object.Limit = 100; // max value?
		@object.Skip = 0;
		return @object.SetOrder(Order.Ascending);
	}
}