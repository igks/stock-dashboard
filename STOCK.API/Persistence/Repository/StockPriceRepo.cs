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
            var stocks = context.StockPrice.AsQueryable();

            if (!string.IsNullOrEmpty(stockPriceParams.Date.Date.ToString()))
            {
                stocks = stocks.Where(s => s.Date.Date == stockPriceParams.Date.Date).Include(s => s.Stock);
            }
            if (!string.IsNullOrEmpty(stockPriceParams.Stock))
            {
                stocks = stocks.Where(s => s.Stock.Name == stockPriceParams.Stock).Include(s => s.Stock);
            }

            var columnsMap = new Dictionary<string, Expression<Func<StockPrice, object>>>()
            {
                ["date"] = s => s.Date,
                ["stock"] = s => s.Stock.Name
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

    }
}