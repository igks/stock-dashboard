using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOCK.API.Core.Model
{
    public class StockVolume
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int BuyVolume { get; set; }
        public double BuyAverage { get; set; }
        public int SellVolume { get; set; }
        public double SellAverage { get; set; }
        public int NetVolume { get; set; }

        public virtual Broker Broker { get; set; }
        public int BrokerId { get; set; }
        public virtual Stock Stock { get; set; }
        public int StockId { get; set; }
    }
}