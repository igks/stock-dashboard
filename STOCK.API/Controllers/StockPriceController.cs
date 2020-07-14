using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STOCK.API.Controllers.Dto;
using STOCK.API.Core.IRepository;
using STOCK.API.Core.Model;
using STOCK.API.Helpers;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockPriceController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IStockPriceRepo stockPriceRepo;
        private readonly IUnitOfWork unitOfWork;

        public StockPriceController(IMapper mapper, IStockPriceRepo stockPriceRepo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.stockPriceRepo = stockPriceRepo;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await stockPriceRepo.GetAll();
            var result = mapper.Map<IEnumerable<ViewStockPriceDto>>(stocks);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stock = await stockPriceRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            var result = mapper.Map<StockPrice, ViewStockPriceDto>(stock);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] StockPriceParams stockPriceParams)
        {
            var stocks = await stockPriceRepo.GetPaged(stockPriceParams);
            var result = mapper.Map<IEnumerable<ViewStockPriceDto>>(stocks);

            Response.AddPagination(stocks.CurrentPage, stocks.PageSize, stocks.TotalCount, stocks.TotalPages);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveStockPriceDto stockPriceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = mapper.Map<SaveStockPriceDto, StockPrice>(stockPriceDto);
            stockPriceRepo.Add(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Create new stock price failed on save");
            }

            stock = await stockPriceRepo.GetById(stock.Id);
            var result = mapper.Map<StockPrice, ViewStockPriceDto>(stock);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveStockPriceDto stockPriceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await stockPriceRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            stock = mapper.Map(stockPriceDto, stock);
            stockPriceRepo.Update(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Update stock price failed on save");
            }

            stock = await stockPriceRepo.GetById(stock.Id);
            var result = mapper.Map<StockPrice, ViewStockPriceDto>(stock);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stock = await stockPriceRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            stockPriceRepo.Delete(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Delete stock price failed on save");
            }
            return Ok(id);
        }

    }
}