using System.Threading.Tasks;

namespace STOCK.API.Core.IRepository
{
    public interface IUnitOfWork
    {
        Task<bool> CompleteAsync();
    }
}