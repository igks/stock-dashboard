using System;

namespace STOCK.API.Core.Model
{
    public class Candle
    {
        public DateTime Date { get; set; }
        public Int32 Open { get; set; }
        public Int32 Low { get; set; }
        public Int32 High { get; set; }
        public Int32 Close { get; set; }
    }
}