using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using STOCK.API.Controllers.Dto;
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
        Task<int> RecordDailyPrice(IFormFile file);
        Task<int> RecordHistoryPrice(IFormFile file);
        int isRecordExist(SaveStockPriceDto stockPriceDto);
    }
}