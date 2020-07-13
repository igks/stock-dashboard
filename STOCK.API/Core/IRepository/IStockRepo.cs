using System.Collections.Generic;
using System.Threading.Tasks;
using STOCK.API.Core.Model;
using STOCK.API.Helpers;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Core.IRepository
{
    public interface IStockRepo
    {
        Task<IEnumerable<Stock>> GetAll();
        Task<Stock> GetById(int id);
        Task<PagedList<Stock>> GetPaged(StockParams stockParams);
        void Add(Stock stock);
        void Update(Stock stock);
        void Delete(Stock stock);
    }
}