﻿using System.Collections.Generic;

namespace VSLee.IEXSharp.Model.Stock.Response
{
	public class HistoricalPriceDynamicResponse
	{
		public string range { get; set; }
		public List<HistoricalPriceResponse> data { get; set; }
	}
}