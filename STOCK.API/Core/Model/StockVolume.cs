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

        [Column(TypeName = "bigint")]
        public int BuyVolume { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal BuyAverage { get; set; }

        [Column(TypeName = "bigint")]
        public int SellVolume { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal SellAverage { get; set; }

        [Column(TypeName = "bigint")]
        public int NetVolume { get; set; }

        public virtual Broker Broker { get; set; }
        public int BrokerId { get; set; }
        public virtual Stock Stock { get; set; }
        public int StockId { get; set; }
    }
}