// Module name: PrivmxEndpointCsharpExtra
// File name: PagingQueryExtension.cs
// Last edit: 2025-02-23 23:02 by Mateusz Chojnowski mchojnowsk@simplito.com
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of privmx-endpoint-csharp extra published under MIT License.

using PrivMX.Endpoint.Core.Models;

namespace PrivmxEndpointCsharpExtra;

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