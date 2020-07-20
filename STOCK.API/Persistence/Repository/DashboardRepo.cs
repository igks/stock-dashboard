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

            // take max stock and start date
            var stock = context.Stock.FirstOrDefault(s => s.Id == Int32.Parse(dashboardParams.Stock));
            var maxStock = stock.MaxVolume;
            var startDate = stock.FirstUpdateVolume;

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
            var volumeList = volume
                .Where(v => broker.Contains(v.BrokerId) &&
                        v.Date.Date >= startDate.Date &&
                        v.StockId.ToString() == dashboardParams.Stock
                ).ToList();

            var allSummary = new List<object>();
            var brokerData = new List<object>();

            for (var indexVolume = 0; indexVolume < volumeList.Count(); indexVolume++)
            {
                for (var indexBroker = 0; indexBroker < broker.Count(); indexBroker++)
                {
                    if (broker[indexBroker] == volumeList[indexVolume].BrokerId)
                    {
                        brokerData.Add(new
                        {
                            date = volumeList[indexVolume].Date,
                            broker = context.Broker.FirstOrDefault(b => b.Id == volumeList[indexVolume].BrokerId).Name,
                            netVolume = volumeList[indexVolume].NetVolume
                        });
                    }
                }
            }
            var brokerdatalength = brokerData.Count();
            Console.WriteLine(brokerdatalength);

            foreach (var item in brokerData)
            {

                Console.WriteLine(item.netVolume);


            }

            return (new { });

            foreach (var item in broker)
            {
                brokerData.Clear();
                for (var indexVolume = 0; indexVolume < volumeList.Count(); indexVolume++)
                {
                    var bufferNetVolumeACC = 0;
                    if (item == volumeList[indexVolume].BrokerId)
                    {
                        if (brokerData.Count() == 0)
                        {
                            var netVolume = 0;
                            if (volumeList[indexVolume].NetVolume > 0)
                            {
                                netVolume = volumeList[indexVolume].NetVolume;
                            }

                            bufferNetVolumeACC = netVolume;
                            brokerData.Add(new
                            {
                                date = volumeList[indexVolume].Date,
                                accVolume = netVolume
                            });
                        }
                        if (brokerData.Count() > 0)
                        {
                            var netVolume = 0;
                            if (volumeList[indexVolume].NetVolume > 0)
                            {
                                netVolume = volumeList[indexVolume].NetVolume;
                            }

                            bufferNetVolumeACC += netVolume;
                            brokerData.Add(new
                            {
                                date = volumeList[indexVolume].Date,
                                accVolume = bufferNetVolumeACC
                            });
                        }
                    }
                }

            }
            // date = y.
            //         })
            //     for (var indexVolume = 0; indexVolume < volumeList.Count(); indexVolume++)
            //     {
            //         if (allSummary.Count() <= 0)
            //         {
            //             allSummary.Add({

            //             })
            //         }
            // allSummary.
            // Console.WriteLine(volumeList[indexVolume].NetVolume);
            //     }


            // var summary = volume.GroupBy(v => v.BrokerId).Select(g => new
            // {
            //     broker = context.Broker.FirstOrDefault(b => b.Id == g.Key).Name,
            //     total = g.Sum(i => i.NetVolume)
            // });

            // // take stock price slider exclude saturday and sunday
            // var slider = new List<object>() { };
            // foreach (var item in price)
            // {
            //     if (item.Date.DayOfWeek != DayOfWeek.Sunday && item.Date.DayOfWeek != DayOfWeek.Saturday)
            //     {
            //         slider.Add(new { stockDate = item.Date, price = item.Price });
            //     }

            // }

            // return (new { summary, maxStock, slider });
        }
    }
}