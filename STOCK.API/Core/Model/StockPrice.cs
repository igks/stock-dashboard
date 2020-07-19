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

        [Column(TypeName = "bigint")]
        public int Volume { get; set; }
        public int Change { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal ChangeRatio { get; set; }
        public virtual Stock Stock { get; set; }
        public int StockId { get; set; }
    }
}