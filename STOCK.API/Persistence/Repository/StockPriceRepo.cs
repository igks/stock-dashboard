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
    public class StockPriceRepo : IStockPriceRepo
    {
        private readonly AppDbContext context;

        public StockPriceRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<StockPrice>> GetAll()
        {
            return await context.StockPrice
                .Include(sp => sp.Stock)
                .ToListAsync();
        }

        public async Task<StockPrice> GetById(int id)
        {
            return await context.StockPrice
            .Include(sp => sp.Stock)
            .SingleOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<PagedList<StockPrice>> GetPaged(StockPriceParams stockPriceParams)
        {
            var stocks = context.StockPrice
                    .Include(s => s.Stock)
                    .AsQueryable();

            if (stockPriceParams.Date.HasValue)
            {
                stocks = stocks.Where(s => s.Date.Date == stockPriceParams.Date.Value.Date);
            }
            if (!string.IsNullOrEmpty(stockPriceParams.Stock))
            {
                stocks = stocks.Where(s => s.Stock.Name.Contains(stockPriceParams.Stock));
            }
            if (!string.IsNullOrEmpty(stockPriceParams.Price))
            {
                var priceParam = Int32.Parse(stockPriceParams.Price);
                stocks = stocks.Where(s => s.Price == priceParam);
            }

            var columnsMap = new Dictionary<string, Expression<Func<StockPrice, object>>>()
            {
                ["date"] = s => s.Date,
                ["stock"] = s => s.Stock.Name,
                ["price"] = s => s.Price
            };

            stocks = stocks.ApplyOrdering(stockPriceParams, columnsMap);
            return await PagedList<StockPrice>.CreateAsync(stocks, stockPriceParams.PageNumber, stockPriceParams.PageSize);
        }

        public void Add(StockPrice stockPrice)
        {
            context.StockPrice.Add(stockPrice);
        }

        public void Update(StockPrice stockPrice)
        {
            context.StockPrice.Attach(stockPrice);
            context.Entry(stockPrice).State = EntityState.Modified;
        }

        public void Delete(StockPrice stockPrice)
        {
            context.Remove(stockPrice);
        }

        public async Task<int> RecordDailyPrice(IFormFile file)
        {
            // Extract date from file name
            // filename format must follow this format
            // "sp-ABCDE-YYYY-MM-DD.csv"
            //            0          1  
            var fileName = file.FileName.Split(".");
            var stringDate = fileName[0].Split("-");
            // "sp-ABCDE-YYYY-MM-DD"
            //   0  1     2   3  4  
            var stringYear = stringDate[2];
            var stringMonth = stringDate[3];
            var stringDay = stringDate[4];
            var dateData = String.Join("-", stringYear, stringMonth, stringDay);

            // Track if any valid data was add to DB
            var recordAdded = 0;

            // Set this variable to skip record header
            var skipHeader = 1;

            // Read uploaded file
            using (var csv = new CachedCsvReader(new StreamReader(file.OpenReadStream()), false))
            {
                while (csv.ReadNextRecord())
                {
                    if (skipHeader == 0)
                    {
                        //Prepare data, index is based on this format
                        // "","Code","Price","Change","Change(%)","Change(%)","High","Low","Bid Vol","Bid","Offer","Offer Vol","Volume"
                        // 0    1      2       3         4             5         6      7      8        9     10      11           12
                        // Add data to DB and skip the data if the stock code not exist
                        var StockId = TakeStockId(csv[1]);
                        if (StockId > -1)
                        {
                            // Stock id valid mean stock code is exist in DB
                            // Check if record exist
                            var Dto = new SaveStockPriceDto()
                            {
                                Date = DateTime.Parse(dateData.Replace('"', ' ').Trim()),
                                StockId = StockId
                            };
                            var recordId = isRecordExist(Dto);

                            // Overwrite record if it already exist
                            if (recordId > -1)
                            {
                                var existRecord = await context.StockPrice.FindAsync(recordId);

                                existRecord.Date = DateTime.Parse(dateData.Replace('"', ' ').Trim());
                                existRecord.StockId = StockId;
                                existRecord.Price = Int32.Parse(csv[2].Replace(",", ""));
                                existRecord.Open = 0;
                                existRecord.High = Int32.Parse(csv[6].Replace(",", ""));
                                existRecord.Low = Int32.Parse(csv[7].Replace(",", ""));
                                existRecord.Volume = Int32.Parse(csv[12].Replace(",", ""));
                                existRecord.Change = Int32.Parse(csv[4].Replace(",", ""));
                                existRecord.ChangeRatio = decimal.Parse(csv[5].Replace(",", ""));

                                context.StockPrice.Attach(existRecord);
                                context.Entry(existRecord).State = EntityState.Modified;
                                recordAdded++;
                            }
                            // Create new record if the data not exist
                            // Increase record tracker
                            if (recordId == -1)
                            {
                                var stockPrice = new StockPrice()
                                {
                                    Date = DateTime.Parse(dateData.Replace('"', ' ').Trim()),
                                    StockId = StockId,
                                    Price = Int32.Parse(csv[2].Replace(",", "")),
                                    Open = 0,
                                    High = Int32.Parse(csv[6].Replace(",", "")),
                                    Low = Int32.Parse(csv[7].Replace(",", "")),
                                    Volume = Int32.Parse(csv[12].Replace(",", "")),
                                    Change = Int32.Parse(csv[4].Replace(",", "")),
                                    ChangeRatio = decimal.Parse(csv[5].Replace(",", ""))
                                };
                                context.StockPrice.Add(stockPrice);
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

        public async Task<int> RecordHistoryPrice(IFormFile file)
        {
            // Extract stock code from file name
            // filename format must follow this format
            // "sph-ABCDE-YYYY-MM-DD.csv"
            //            0           1 
            var fileName = file.FileName.Split(".");
            var stringCode = fileName[0].Split("-");
            // "sph-ABCDE-YYYY-MM-DD"
            //   0   1     2   3  4 
            var stockCode = stringCode[1];
            var StockId = TakeStockId(stockCode);

            // Track if any valid data was add to DB
            var recordAdded = 0;

            // Set this variable to skip record header
            var skipHeader = 1;

            // Read uploaded file
            using (var csv = new CachedCsvReader(new StreamReader(file.OpenReadStream()), false))
            {
                while (csv.ReadNextRecord())
                {
                    if (skipHeader == 0)
                    {
                        //Prepare data, index is based on this format
                        //"Date","Open","High","Low","Close","Change","Change(%)","Ratio(%)","Volume","Value(T)"
                        //  0      1      2      3      4        5       6            7         8        9
                        // Add data to DB and skip the data if the stock code not exist
                        if (StockId > -1)
                        {
                            // Stock id valid mean stock code is exist in DB
                            // Check if record exist
                            var Dto = new SaveStockPriceDto()
                            {
                                Date = DateTime.Parse(csv[0].Replace('/', '-').Trim()),
                                StockId = StockId
                            };
                            var recordId = isRecordExist(Dto);

                            // Overwrite record if it already exist
                            if (recordId > -1)
                            {
                                var existRecord = await context.StockPrice.FindAsync(recordId);

                                existRecord.Date = DateTime.Parse(csv[0].Replace('/', '-').Trim());
                                existRecord.StockId = StockId;
                                existRecord.Price = Int32.Parse(csv[4].Replace(",", ""));
                                existRecord.Open = Int32.Parse(csv[1].Replace(",", ""));
                                existRecord.High = Int32.Parse(csv[2].Replace(",", ""));
                                existRecord.Low = Int32.Parse(csv[3].Replace(",", ""));
                                existRecord.Volume = Int32.Parse(csv[8].Replace(",", ""));
                                existRecord.Change = Int32.Parse(csv[6].Replace(",", ""));
                                existRecord.ChangeRatio = decimal.Parse(csv[7].Replace(",", ""));

                                context.StockPrice.Attach(existRecord);
                                context.Entry(existRecord).State = EntityState.Modified;
                                recordAdded++;
                            }

                            // Create new record if tha data not exist 
                            // Increase record tracker
                            if (recordId == -1)
                            {
                                var stockPrice = new StockPrice()
                                {
                                    Date = DateTime.Parse(csv[0].Replace('/', '-').Trim()),
                                    StockId = StockId,
                                    Price = Int32.Parse(csv[4].Replace(",", "")),
                                    Open = Int32.Parse(csv[1].Replace(",", "")),
                                    High = Int32.Parse(csv[2].Replace(",", "")),
                                    Low = Int32.Parse(csv[3].Replace(",", "")),
                                    Volume = Int32.Parse(csv[8].Replace(",", "")),
                                    Change = Int32.Parse(csv[6].Replace(",", "")),
                                    ChangeRatio = decimal.Parse(csv[7].Replace(",", ""))
                                };
                                context.StockPrice.Add(stockPrice);
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

        public int isRecordExist(SaveStockPriceDto stockPriceDto)
        {
            var paramDate = stockPriceDto.Date.Date;
            var paramId = stockPriceDto.StockId;
            var recordId = -1;
            var stocks = context.StockPrice.FirstOrDefault(s => s.Date.Date == paramDate && s.StockId == paramId);
            if (stocks != null)
            {
                recordId = stocks.Id;
            }
            return recordId;
        }
    }
}