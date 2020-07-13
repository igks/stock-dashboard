using System;

namespace STOCK.API.Controllers.Dto
{
    public class SaveStockDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int MaxVolume { get; set; }
        public DateTime FirstUpdateVolume { get; set; }

    }

    public class ViewStockDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int MaxVolume { get; set; }
        public DateTime FirstUpdateVolume { get; set; }
    }
}