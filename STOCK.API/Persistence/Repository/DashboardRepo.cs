using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STOCK.API.Core.IRepository;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Persistence.Repository
{
    public class DashboardRepo : IDashboardRepo
    {
        private readonly AppDbContext context;

        public DashboardRepo(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<object> GetBarChartData(DashboardParams dashboardParams)
        {
            var volume = context.StockVolume.AsQueryable();
            var price = context.StockPrice.AsQueryable();
            // select price base on params
            price = price.Where(s => s.StockId == Int32.Parse(dashboardParams.Stock));
            price = price.OrderBy(s => s.Date);

            // take max stock
            var maxStock = context.Stock.Where(s => s.Id == Int32.Parse(dashboardParams.Stock)).Select(s => s.MaxVolume);

            // sort from highest net volume then take 5 top broker
            var broker = new List<int>();
            if (dashboardParams.IsTop5)
            {
                var top5Broker = volume.OrderByDescending(v => v.NetVolume);
                foreach (var item in top5Broker)
                {
                    if (!broker.Contains(item.BrokerId))
                    {
                        broker.Add(item.BrokerId);
                    }
                    if (broker.Count == 5)
                    {
                        break;
                    }
                }
            }
            // take broker selected by user
            if (!dashboardParams.IsTop5 && !String.IsNullOrEmpty(dashboardParams.Broker))
            {
                var brokerArr = dashboardParams.Broker.Split(",");
                foreach (var item in brokerArr)
                {
                    broker.Add(Int32.Parse(item));
                }
            }

            // collect database base on selected parameter
            volume = volume
                .Where(v => broker.Contains(v.BrokerId) &&
                        v.Date.Year == dashboardParams.Year &&
                        v.StockId.ToString() == dashboardParams.Stock &&
                        v.NetVolume > 0
                );

            var summary = volume.GroupBy(v => v.BrokerId).Select(g => new
            {
                broker = context.Broker.FirstOrDefault(b => b.Id == g.Key).Name,
                total = g.Sum(i => i.NetVolume)
            });

            // take stock price slider exclude saturday and sunday
            var slider = new List<object>() { };
            foreach (var item in price)
            {
                if (item.Date.DayOfWeek != DayOfWeek.Sunday && item.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    slider.Add(new { stockDate = item.Date, price = item.Price });
                }

            }

            return (new { summary, maxStock, slider });
        }
    }
}