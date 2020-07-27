using System;

namespace STOCK.API.Core.Model
{
    public class NetDraft
    {
        public DateTime Date { get; set; }
        public Int32 TotalBuy5 { get; set; }
        public Int32 TotalSell5 { get; set; }
        public Int32 TotalBuy3 { get; set; }
        public Int32 TotalSell3 { get; set; }
        public Int32 DifNet5 { get; set; }
        public Int32 DifNet3 { get; set; }
    }
}