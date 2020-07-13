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
    public class StockController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IStockRepo stockRepo;
        private readonly IUnitOfWork unitOfWork;

        public StockController(IMapper mapper, IStockRepo stockRepo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.stockRepo = stockRepo;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await stockRepo.GetAll();
            var result = mapper.Map<IEnumerable<ViewStockDto>>(stocks);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stock = await stockRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            var result = mapper.Map<Stock, ViewStockDto>(stock);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPage([FromQuery] StockParams stockParams)
        {
            var stocks = await stockRepo.GetPaged(stockParams);
            var result = mapper.Map<IEnumerable<Stock>>(stocks);

            Response.AddPagination(stocks.CurrentPage, stocks.PageSize, stocks.TotalCount, stocks.TotalPages);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveStockDto stockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = mapper.Map<SaveStockDto, Stock>(stockDto);
            stockRepo.Add(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Create new stock failed on save");
            }

            stock = await stockRepo.GetById(stock.Id);
            var result = mapper.Map<Stock, ViewStockDto>(stock);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveStockDto stockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await stockRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            stock = mapper.Map(stockDto, stock);
            stockRepo.Update(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Update stock failed on save");
            }

            stock = await stockRepo.GetById(stock.Id);
            var result = mapper.Map<Stock, ViewStockDto>(stock);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stock = await stockRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            stockRepo.Delete(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                throw new Exception(message: "Delete stock failed on save");
            }

            return Ok(id);
        }
    }
}