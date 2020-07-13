using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STOCK.API.Core.IRepository;

namespace STOCK.API.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CompleteAsync()
        {
            int saveResult = 0;
            string msg = "";
            try
            {
                saveResult = await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {

                saveResult = 0;
                msg = ex.ToString();
            }
            return saveResult > 0;
        }
    }
}