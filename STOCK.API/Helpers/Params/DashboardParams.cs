using System.Collections.Generic;

namespace STOCK.API.Helpers.Params
{
    public class DashboardParams
    {
        public string Stock { get; set; }
        public string Broker { get; set; }
        public bool IsTop5 { get; set; } = false;
        public int Year { get; set; }
    }
}