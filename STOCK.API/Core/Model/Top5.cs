using System;

namespace STOCK.API.Core.Model
{
    public class Top5
    {
        public DateTime Date { get; set; }
        public Int32 TotalBuy { get; set; }
        public Int32 TotalSell { get; set; }
    }
}