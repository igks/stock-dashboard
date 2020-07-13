using System.Collections.Generic;
using System.Threading.Tasks;
using STOCK.API.Core.Model;
using STOCK.API.Helpers;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Core.IRepository
{
    public interface IBrokerRepo
    {
        Task<IEnumerable<Broker>> GetAll();
        Task<Broker> GetById(int id);
        Task<PagedList<Broker>> GetPage(BrokerParams brokerParams);
        void Add(Broker broker);
        void Update(Broker broker);
        void Delete(Broker broker);

    }
}