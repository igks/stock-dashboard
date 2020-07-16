using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using STOCK.API.Controllers.Dto;
using STOCK.API.Core.IRepository;
using STOCK.API.Core.Model;
using STOCK.API.Helpers;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Persistence.Repository
{
    public class StockVolumeRepo : IStockVolumeRepo
    {
        private readonly AppDbContext context;

        public StockVolumeRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<StockVolume>> GetAll()
        {
            return await context.StockVolume
                 .Include(sv => sv.Broker)
                 .Include(sv => sv.Stock)
                 .ToListAsync();
        }

        public async Task<StockVolume> GetById(int id)
        {
            return await context.StockVolume
                .Include(sv => sv.Broker)
                .Include(sv => sv.Stock)
                .SingleOrDefaultAsync(sv => sv.Id == id);
        }

        public async Task<PagedList<StockVolume>> GetPaged(StockVolumeParams stockVolumeParams)
        {
            var stocks = context.StockVolume
                .Include(s => s.Broker)
                .Include(s => s.Stock)
                .AsQueryable();

            if (stockVolumeParams.Date.HasValue)
            {
                stocks = stocks.Where(s => s.Date.Date == stockVolumeParams.Date.Value.Date);
            }

            if (!string.IsNullOrEmpty(stockVolumeParams.Broker))
            {
                stocks = stocks.Where(s => s.Broker.Name.Contains(stockVolumeParams.Broker));
            }
            if (!string.IsNullOrEmpty(stockVolumeParams.Stock))
            {
                stocks = stocks.Where(s => s.Stock.Name.Contains(stockVolumeParams.Stock));
            }

            var columnsMap = new Dictionary<string, Expression<Func<StockVolume, object>>>()
            {
                ["date"] = s => s.Date,
                ["broker"] = s => s.Broker.Name,
                ["stock"] = s => s.Stock.Name
            };

            stocks = stocks.ApplyOrdering(stockVolumeParams, columnsMap);
            return await PagedList<StockVolume>.CreateAsync(stocks, stockVolumeParams.PageNumber, stockVolumeParams.PageSize);
        }

        public void Add(StockVolume stockVolume)
        {
            context.StockVolume.Add(stockVolume);
        }

        public void Update(StockVolume stockVolume)
        {
            context.StockVolume.Attach(stockVolume);
            context.Entry(stockVolume).State = EntityState.Modified;
        }

        public void Delete(StockVolume stockVolume)
        {
            context.Remove(stockVolume);
        }

        public async Task<int> RecordByStockDate(IFormFile file)
        {
            // Extract stock code from file name
            // filename format must follow this format
            // "Stock-YYYY-MM-DD.csv"
            //   0     1   2  3  
            var fileName = file.FileName.Split(".");
            var stringCode = fileName[0].Split("-");
            var stockCode = stringCode[0];
            var StockId = TakeStockId(stockCode);
            var stringYear = stringCode[1];
            var stringMonth = stringCode[2];
            var stringDay = stringCode[3];
            var dateData = String.Join("-", stringYear, stringMonth, stringDay);

            // Track if any valid data was add to DB
            var recordAdded = 0;

            // Set this variable to skip record header
            var skipHeader = 2;

            // Read uploaded file
            using (var csv = new CachedCsvReader(new StreamReader(file.OpenReadStream()), false))
            {
                while (csv.ReadNextRecord())
                {
                    if (skipHeader == 0)
                    {
                        //Prepare data, index is based on this format
                        // "Broker","Buy","Buy","Buy","Buy","Sell","Sell","Sell","Sell","Net Vol"
                        // "Broker","Freq","Vol","Avg","%","Freq","Vol","Avg","%","Net"
                        //     0       1     2     3    4     5      6    7    8    9
                        // Add data to DB and skip the data if the stock code not exist
                        if (StockId > -1)
                        {
                            // Check if broker exist in DB, add if there are not.
                            var setBrokerId = TakeBrokerId(csv[0]);
                            if (setBrokerId == -1)
                            {
                                var broker = new Broker()
                                {
                                    Code = csv[0],
                                    Name = csv[0] + " Automatic add",
                                };
                                context.Broker.Add(broker);
                                context.SaveChanges();
                                setBrokerId = broker.Id;
                            }


                            // Stock id valid mean stock code is exist in DB
                            // Check if record exist
                            var stockVolumeDto = new SaveStockVolumeDto()
                            {
                                Date = DateTime.Parse(dateData.Replace('"', ' ').Trim()),
                                StockId = StockId,
                                BrokerId = setBrokerId
                            };
                            var recordId = isRecordExist(stockVolumeDto);


                            // Overwrite record if it already exist
                            if (recordId > -1)
                            {
                                var existRecord = await context.StockVolume.FindAsync(recordId);

                                existRecord.Date = DateTime.Parse(dateData.Replace('"', ' ').Trim());
                                existRecord.StockId = StockId;
                                existRecord.BrokerId = setBrokerId;
                                existRecord.BuyVolume = Int32.Parse(csv[2].Replace(",", ""));
                                existRecord.BuyAverage = decimal.Parse(csv[3].Replace(",", ""));
                                existRecord.SellVolume = Int32.Parse(csv[6].Replace(",", ""));
                                existRecord.SellAverage = decimal.Parse(csv[7].Replace(",", ""));
                                existRecord.NetVolume = Int32.Parse(csv[9].Replace(",", ""));

                                context.StockVolume.Attach(existRecord);
                                context.Entry(existRecord).State = EntityState.Modified;
                                recordAdded++;
                            }

                            // Create new record if tha data not exist 
                            // Increase record tracker
                            if (recordId == -1)
                            {
                                var stockVolume = new StockVolume()
                                {
                                    Date = DateTime.Parse(dateData.Replace('"', ' ').Trim()),
                                    StockId = StockId,
                                    BrokerId = setBrokerId,
                                    BuyVolume = Int32.Parse(csv[2].Replace(",", "")),
                                    BuyAverage = decimal.Parse(csv[3].Replace(",", "")),
                                    SellVolume = Int32.Parse(csv[6].Replace(",", "")),
                                    SellAverage = decimal.Parse(csv[7].Replace(",", "")),
                                    NetVolume = Int32.Parse(csv[9].Replace(",", "")),
                                };
                                context.StockVolume.Add(stockVolume);
                                recordAdded++;
                            }
                        }
                    }
                    else
                    {
                        skipHeader--;
                    }
                }
            }
            return recordAdded;
        }

        private int TakeStockId(string code)
        {
            // Get Stock Code in DB
            var StockList = context.Stock.ToArray();
            foreach (var stock in StockList)
            {
                if (stock.Code.ToLower() == code.ToLower())
                {
                    return stock.Id;
                }
            }
            return -1;
        }

        private int TakeBrokerId(string code)
        {
            // Get Stock Code in DB
            var BrokerList = context.Broker.ToArray();
            foreach (var broker in BrokerList)
            {
                if (broker.Code.ToLower() == code.ToLower())
                {
                    return broker.Id;
                }
            }
            return -1;
        }

        public int isRecordExist(SaveStockVolumeDto stockVolumeDto)
        {
            var paramDate = stockVolumeDto.Date.Date;
            var paramBrokerId = stockVolumeDto.BrokerId;
            var paramStockId = stockVolumeDto.StockId;
            var recordId = -1;
            var stocks = context.StockVolume.FirstOrDefault(s => s.Date.Date == paramDate && s.StockId == paramStockId && s.BrokerId == paramBrokerId);
            if (stocks != null)
            {
                recordId = stocks.Id;
            }
            return recordId;
        }
    }
}