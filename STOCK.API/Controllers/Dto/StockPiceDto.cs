using System;

namespace STOCK.API.Controllers.Dto
{
    public class SaveStockPriceDto
    {
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public int Open { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int Volume { get; set; }
        public int Change { get; set; }
        public double ChangeRatio { get; set; }
        public int StockId { get; set; }

    }

    public class ViewStockPriceDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public int Open { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int Volume { get; set; }
        public int Change { get; set; }
        public double ChangeRatio { get; set; }
        public string Stock { get; set; }
    }
}