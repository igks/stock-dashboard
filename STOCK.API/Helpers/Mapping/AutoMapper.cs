using AutoMapper;
using STOCK.API.Controllers.Dto;
using STOCK.API.Core.Model;

namespace STOCK.API.Helpers.Mapping
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<SaveBrokerDto, Broker>();
            CreateMap<Broker, ViewBrokerDto>();

            CreateMap<SaveStockDto, Stock>();
            CreateMap<Stock, ViewStockDto>();

            CreateMap<SaveStockPriceDto, StockPrice>();
            CreateMap<StockPrice, ViewStockPriceDto>()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock.Name));

            CreateMap<SaveStockVolumeDto, StockVolume>();
            CreateMap<StockVolume, ViewStockVolumeDto>()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock.Name))
                .ForMember(dest => dest.Broker, opt => opt.MapFrom(src => src.Broker.Name));
        }
    }
}