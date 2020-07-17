using System;

namespace STOCK.API.Helpers.Params
{
    public class StockPriceParams : BaseParams
    {
        public DateTime? Date { get; set; } = null;

        public string Stock { get; set; }
        public string Price { get; set; }
    }
}