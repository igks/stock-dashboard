using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOCK.API.Core.Model
{
    public class Stock
    {
        public Stock()
        {
            StockPrice = new List<StockPrice>();
            StockVolume = new List<StockVolume>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(6)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Column(TypeName = "bigint")]
        public int MaxVolume { get; set; }
        public DateTime FirstUpdateVolume { get; set; }

        public virtual ICollection<StockPrice> StockPrice { get; set; }
        public virtual ICollection<StockVolume> StockVolume { get; set; }


    }
}