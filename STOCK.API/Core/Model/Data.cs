using System;

namespace STOCK.API.Core.Model
{
    public class Data
    {
        public DateTime Date { get; set; }
        public Int32 NetVolume { get; set; }
        public Int32 AccVolume { get; set; }
    }
}