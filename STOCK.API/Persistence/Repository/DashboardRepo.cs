using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using STOCK.API.Core.IRepository;
using STOCK.API.Helpers.Params;
using STOCK.API.Core.Model;

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

        public async Task<object> GetStockChartData(DashboardParams dashboardParams)
        {
            // Retrive data
            var candlesQuery = context.StockPrice.AsQueryable();
            var volumeQuery = context.StockVolume.AsQueryable();
            candlesQuery = candlesQuery.Where(c => c.StockId == Int32.Parse(dashboardParams.Stock)).OrderBy(c => c.Date);
            volumeQuery = volumeQuery.Where(v => v.StockId == Int32.Parse(dashboardParams.Stock)).OrderBy(v => v.Date);

            // Create data for candle, volume and MA
            var candlesList = new List<Candle>();
            var volumeList = new List<VolumeBar>();

            foreach (var candle in candlesQuery)
            {
                // Create data for candle
                candlesList.Add(new Candle
                {
                    Date = candle.Date,
                    Open = candle.Open,
                    Low = candle.Low,
                    High = candle.High,
                    Close = candle.Price
                });

                // Create data for volume
                volumeList.Add(new VolumeBar
                {
                    Date = candle.Date,
                    Price = candle.Price
                });

            }

            // Create data for MA
            var MAList = new List<MA>();
            var maPeriod = 20;
            for (int indexCandle = 0; indexCandle < candlesList.Count(); indexCandle++)
            {
                var accumulative = 0;
                var dataLength = 0;
                if (indexCandle < maPeriod)
                {
                    for (int indexMA = 0; indexMA <= indexCandle; indexMA++)
                    {
                        accumulative += candlesList[indexCandle - indexMA].Close;
                        dataLength++;
                    }
                    var average = accumulative / dataLength;
                    MAList.Add(new MA
                    {
                        Date = candlesList[indexCandle].Date,
                        Average = average
                    });
                }
                if (indexCandle >= maPeriod)
                {
                    for (int indexMA = 0; indexMA <= maPeriod; indexMA++)
                    {
                        accumulative += candlesList[indexCandle - indexMA].Close;
                    }
                    var average = accumulative / maPeriod;
                    MAList.Add(new MA
                    {
                        Date = candlesList[indexCandle].Date,
                        Average = average
                    });
                }
            }

            // Create data for net line
            var net3 = new List<Net3>();
            var net5 = new List<Net5>();
            var netDraft = new List<NetDraft>();
            var allAvailableDate = new List<DateTime>();

            // Create buffer date
            foreach (var volume in volumeQuery)
            {
                allAvailableDate.Add(volume.Date);
            }

            //Remove duplicate date
            var filteredDate = allAvailableDate.Distinct().ToList();

            // Create buffer data for net for each date
            foreach (var item in filteredDate)
            {
                var volumes = new List<StockVolume>();
                volumes = volumeQuery.Where(v => v.Date.Date == item.Date).ToList();

                var TotalBuy3 = extractValue(volumes, 3, true);
                var TotalSell3 = extractValue(volumes, 3, false);
                var TotalBuy5 = extractValue(volumes, 5, true);
                var TotalSell5 = extractValue(volumes, 5, false);

                netDraft.Add(new NetDraft
                {
                    Date = item,
                    TotalBuy3 = TotalBuy3,
                    TotalSell3 = TotalSell3,
                    TotalBuy5 = TotalBuy5,
                    TotalSell5 = TotalSell5,
                    DifNet3 = TotalBuy3 - TotalSell3,
                    DifNet5 = TotalBuy5 - TotalSell5
                });
            }

            // populate net data
            for (int i = 0; i < netDraft.Count(); i++)
            {
                if (i == 0)
                {
                    net3.Add(new Net3
                    {
                        Date = netDraft[i].Date,
                        Net3Value = netDraft[i].DifNet3
                    });

                    net5.Add(new Net5
                    {
                        Date = netDraft[i].Date,
                        Net5Value = netDraft[i].DifNet5
                    });
                }
                if (i > 0)
                {
                    net3.Add(new Net3
                    {
                        Date = netDraft[i].Date,
                        Net3Value = net3[i - 1].Net3Value + netDraft[i].DifNet3
                    });

                    net5.Add(new Net5
                    {
                        Date = netDraft[i].Date,
                        Net5Value = net5[i - 1].Net5Value + netDraft[i].DifNet5
                    });
                }
            }

            return (new { candlesList, volumeList, MAList, net3, net5 });
        }

        private Int32 extractValue(List<StockVolume> volumeQuery, int iteration, bool buy = false)
        {
            var value = 0;
            if (buy)
            {
                var volume = volumeQuery.OrderByDescending(v => v.BuyVolume).ToList();
                if (volume.Count() >= iteration)
                {
                    for (int i = 0; i < iteration; i++)
                    {
                        value += volume[i].BuyVolume;
                    }
                }
            }
            if (!buy)
            {
                var volume = volumeQuery.OrderByDescending(v => v.SellVolume).ToList();
                if (volume.Count() >= iteration)
                {
                    for (int i = 0; i < iteration; i++)
                    {
                        value += volume[i].SellVolume;
                    }
                }
            }
            return value;
        }
    }
}