using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            // query all data
            var volumesQuery = context.StockVolume.AsQueryable();
            volumesQuery = volumesQuery.OrderBy(vq => vq.Date);
            var pricesQuery = context.StockPrice.AsQueryable();

            // select price base on params
            pricesQuery = pricesQuery.Where(s => s.StockId == Int32.Parse(dashboardParams.Stock));
            pricesQuery = pricesQuery.OrderBy(s => s.Date);

            // take max stock and start date
            var stock = context.Stock.FirstOrDefault(s => s.Id == Int32.Parse(dashboardParams.Stock));
            var maxStock = stock.MaxVolume;
            var startDate = stock.FirstUpdateVolume;

            // collect database base on selected parameter
            var volumeList = volumesQuery
                .Where(v => v.Date.Date >= startDate.Date && v.StockId == Int32.Parse(dashboardParams.Stock)
                ).ToList();

            // Add data to each broker
            var brokerList = context.Broker.ToList();
            var brokersData = new List<BrokerData>();
            foreach (var broker in brokerList)
            {
                var brokerData = new BrokerData();
                var data = new List<Data>();
                foreach (var volume in volumeList)
                {
                    if (volume.BrokerId == broker.Id)
                    {
                        var temporaryData = new Data()
                        {
                            Date = volume.Date,
                            NetVolume = volume.NetVolume,
                            AccVolume = 0
                        };
                        data.Add(temporaryData);
                    }
                }
                var temporaryBrokerData = new BrokerData()
                {
                    Id = broker.Id,
                    Name = broker.Name,
                    AccVolume = 0,
                    Data = data
                };
                brokersData.Add(temporaryBrokerData);
            }

            // Create accumulative volume
            foreach (var broker in brokersData)
            {
                for (var index = 0; index < broker.Data.Count(); index++)
                {
                    var accumulator = broker.Data.ElementAt(index).NetVolume < 0 ? 0 : broker.Data.ElementAt(index).NetVolume;
                    if (index > 0)
                    {
                        broker.Data.ElementAt(index).NetVolume = accumulator;
                        broker.Data.ElementAt(index).AccVolume = accumulator + broker.Data.ElementAt(index - 1).AccVolume;
                        broker.AccVolume = accumulator + broker.Data.ElementAt(index - 1).AccVolume;
                    }
                    if (index == 0)
                    {
                        broker.Data.ElementAt(index).NetVolume = accumulator;
                        broker.Data.ElementAt(index).AccVolume = accumulator;
                        broker.AccVolume = accumulator;
                    }
                }
            }

            // sort from highest accumulative volume then take top 5 broker
            var selectedBrokers = new List<BrokerData>();
            if (dashboardParams.IsTop5)
            {
                selectedBrokers = brokersData.OrderByDescending(b => b.AccVolume).Take(5).ToList();
            }
            // take broker selected by user
            if (!dashboardParams.IsTop5 && !String.IsNullOrEmpty(dashboardParams.Broker))
            {
                var brokerArr = dashboardParams.Broker.Split(",");
                foreach (var brokerId in brokerArr)
                {
                    foreach (var brokerData in brokersData)
                    {
                        if (brokerData.Id == Int32.Parse(brokerId))
                        {
                            selectedBrokers.Add(brokerData);
                        }
                    }
                }
            }

            // filter data only return data with year selected in UI
            foreach (var broker in selectedBrokers)
            {
                broker.Data = broker.Data.Where(d => d.Date.Year == dashboardParams.Year).ToList();
            }

            // take stock price slider exclude saturday and sunday
            var slider = new List<object>() { };
            foreach (var item in pricesQuery)
            {
                if (item.Date.Year == dashboardParams.Year &&
                    item.Date.DayOfWeek != DayOfWeek.Sunday &&
                    item.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    slider.Add(new { stockDate = item.Date, price = item.Price });
                }

            }
            return (new { selectedBrokers, maxStock, slider });
        }

    }

    public class BrokerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int32 AccVolume { get; set; }
        public ICollection<Data> Data { get; set; }
    }

    public class Data
    {
        public DateTime Date { get; set; }
        public Int32 NetVolume { get; set; }
        public Int32 AccVolume { get; set; }
    }
}