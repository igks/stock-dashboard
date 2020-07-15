using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var stocks = context.StockVolume.AsQueryable();

            if (!string.IsNullOrEmpty(stockVolumeParams.Broker))
            {
                stocks = stocks.Where(s => s.Broker.Name == stockVolumeParams.Broker)
                    .Include(s => s.Broker)
                    .Include(s => s.Stock);
            }
            if (!string.IsNullOrEmpty(stockVolumeParams.Broker))
            {
                stocks = stocks.Where(s => s.Stock.Name == stockVolumeParams.Stock)
                    .Include(s => s.Broker)
                    .Include(s => s.Stock);
            }
            if (!string.IsNullOrEmpty(stockVolumeParams.Date.ToString()))
            {
                stocks = stocks.Where(s => s.Date.Date == stockVolumeParams.Date.Date)
                    .Include(s => s.Broker)
                    .Include(s => s.Stock);
            }

            var columnsMap = new Dictionary<string, Expression<Func<StockVolume, object>>>()
            {
                ["date"] = s => s.Date,
                ["broker"] = s => s.Broker,
                ["stock"] = s => s.Stock
            };

            stocks.ApplyOrdering(stockVolumeParams, columnsMap);
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
    }
}