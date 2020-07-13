using AutoMapper;
using STOCK.API.Controllers.Dto;
using STOCK.API.Core.Model;

namespace STOCK.API.Helpers.Mapping
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Broker, ViewBrokerDto>();
            CreateMap<SaveBrokerDto, Broker>();

            CreateMap<Stock, ViewStockDto>();
            CreateMap<SaveStockDto, Stock>();
        }
    }
}