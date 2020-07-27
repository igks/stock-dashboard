using System.Threading.Tasks;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Core.IRepository
{
    public interface IDashboardRepo
    {
        Task<object> GetBarChartData(DashboardParams dashboardParams);
        Task<object> GetStockChartData(DashboardParams dashboardParams);
    }
}