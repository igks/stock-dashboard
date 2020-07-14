using System.Collections.Generic;
using System.Threading.Tasks;
using STOCK.API.Core.Model;
using STOCK.API.Helpers;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Core.IRepository
{
    public interface IStockPriceRepo
    {
        Task<IEnumerable<StockPrice>> GetAll();
        Task<StockPrice> GetById(int id);
        Task<PagedList<StockPrice>> GetPaged(StockPriceParams stockPriceParams);
        void Add(StockPrice stockPrice);
        void Update(StockPrice stockPrice);
        void Delete(StockPrice stockPrice);
    }
}