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
    public class StockRepo : IStockRepo
    {
        private readonly AppDbContext context;

        public StockRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Stock>> GetAll()
        {
            return await context.Stock.ToListAsync();
        }

        public async Task<Stock> GetById(int id)
        {
            return await context.Stock.FindAsync(id);
        }

        public async Task<PagedList<Stock>> GetPaged(StockParams stockParams)
        {
            var stocks = context.Stock.AsQueryable();

            if (!string.IsNullOrEmpty(stockParams.Code))
            {
                stocks = stocks.Where(s => s.Code.Contains(stockParams.Code));
            }
            if (!string.IsNullOrEmpty(stockParams.Name))
            {
                stocks = stocks.Where(s => s.Name.Contains(stockParams.Name));
            }

            var columnsMap = new Dictionary<string, Expression<Func<Stock, object>>>()
            {
                ["code"] = s => s.Code,
                ["name"] = s => s.Name
            };

            stocks = stocks.ApplyOrdering(stockParams, columnsMap);
            return await PagedList<Stock>.CreateAsync(stocks, stockParams.PageNumber, stockParams.PageSize);
        }

        public void Add(Stock stock)
        {
            context.Stock.Add(stock);
        }

        public void Update(Stock stock)
        {
            context.Stock.Attach(stock);
            context.Entry(stock).State = EntityState.Modified;
        }

        public void Delete(Stock stock)
        {
            context.Remove(stock);
        }
    }
}