using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using STOCK.API.Controllers.Dto;
using STOCK.API.Core.Model;
using STOCK.API.Helpers;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Core.IRepository
{
    public interface IStockVolumeRepo
    {
        Task<IEnumerable<StockVolume>> GetAll();
        Task<StockVolume> GetById(int id);
        Task<PagedList<StockVolume>> GetPaged(StockVolumeParams stockVolumeParams);
        void Add(StockVolume stockVolume);
        void Update(StockVolume stockVolume);
        void Delete(StockVolume stockVolume);
        Task<int> RecordByStockDate(IFormFile file);
        int isRecordExist(SaveStockVolumeDto stockVolumeDto);
    }
}