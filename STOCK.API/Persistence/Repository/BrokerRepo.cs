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
    public class BrokerRepo : IBrokerRepo
    {
        private readonly AppDbContext context;

        public BrokerRepo(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Broker>> GetAll()
        {
            return await context.Broker.ToListAsync();
        }

        public async Task<Broker> GetById(int id)
        {
            return await context.Broker.FindAsync(id);
        }

        public async Task<PagedList<Broker>> GetPage(BrokerParams brokerParams)
        {
            var brokers = context.Broker.AsQueryable();

            if (!string.IsNullOrEmpty(brokerParams.Code))
            {
                brokers = brokers.Where(b => b.Code.Contains(brokerParams.Code));
            }
            if (!string.IsNullOrEmpty(brokerParams.Name))
            {
                brokers = brokers.Where(b => b.Name.Contains(brokerParams.Name));
            }

            var columnsMap = new Dictionary<string, Expression<Func<Broker, object>>>()
            {
                ["code"] = b => b.Code,
                ["name"] = b => b.Name
            };

            brokers = brokers.ApplyOrdering(brokerParams, columnsMap);

            return await PagedList<Broker>.CreateAsync(brokers, brokerParams.PageNumber, brokerParams.PageSize);
        }

        public void Add(Broker broker)
        {
            context.Broker.Add(broker);
        }

        public void Update(Broker broker)
        {
            context.Broker.Attach(broker);
            context.Entry(broker).State = EntityState.Modified;
        }

        public void Delete(Broker broker)
        {
            context.Remove(broker);
        }

    }
}