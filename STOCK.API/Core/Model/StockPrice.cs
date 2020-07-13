using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOCK.API.Core.Model
{
    public class StockPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public int Open { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int Volume { get; set; }
        public int Change { get; set; }
        public double ChangeRatio { get; set; }
        public virtual Stock Stock { get; set; }
        public int StockId { get; set; }
    }
}