using System;

namespace STOCK.API.Controllers.Dto
{
    public class SaveStockVolumeDto
    {
        public DateTime Date { get; set; }
        public int BuyVolume { get; set; }
        public double BuyAverage { get; set; }
        public int SellVolume { get; set; }
        public double SellAverage { get; set; }
        public int NetVolume { get; set; }
        public int BrokerId { get; set; }
        public int StockId { get; set; }
    }

    public class ViewStockVolumeDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int BuyVolume { get; set; }
        public double BuyAverage { get; set; }
        public int SellVolume { get; set; }
        public double SellAverage { get; set; }
        public int NetVolume { get; set; }
        public string Broker { get; set; }
        public string Stock { get; set; }
    }
}