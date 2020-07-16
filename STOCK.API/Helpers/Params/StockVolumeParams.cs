using System;

namespace STOCK.API.Helpers.Params
{
    public class StockVolumeParams : BaseParams
    {
        public DateTime? Date { get; set; } = null;
        public string Broker { get; set; }
        public string Stock { get; set; }
    }
}