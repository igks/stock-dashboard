using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOCK.API.Core.Model
{
    public class Broker
    {
        public Broker()
        {
            StockVolume = new List<StockVolume>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(5)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        public virtual ICollection<StockVolume> StockVolume { get; set; }
    }
}