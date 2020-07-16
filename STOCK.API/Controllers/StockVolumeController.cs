using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class StockVolumeController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IStockVolumeRepo stockVolumeRepo;
        private readonly IUnitOfWork unitOfWork;

        public StockVolumeController(IMapper mapper, IStockVolumeRepo stockVolumeRepo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.stockVolumeRepo = stockVolumeRepo;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await stockVolumeRepo.GetAll();
            var result = mapper.Map<IEnumerable<ViewStockVolumeDto>>(stocks);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stock = await stockVolumeRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            var result = mapper.Map<StockVolume, ViewStockVolumeDto>(stock);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] StockVolumeParams stockVolumeParams)
        {
            var stocks = await stockVolumeRepo.GetPaged(stockVolumeParams);
            var result = mapper.Map<IEnumerable<ViewStockVolumeDto>>(stocks);

            Response.AddPagination(stocks.CurrentPage, stocks.PageSize, stocks.TotalCount, stocks.TotalPages);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveStockVolumeDto stockVolumeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recordId = stockVolumeRepo.isRecordExist(stockVolumeDto);
            if (recordId > 0)
            {
                string str = "duplicate";
                object data = new { str, recordId };
                return StatusCode(200, data);
            }

            var stock = mapper.Map<SaveStockVolumeDto, StockVolume>(stockVolumeDto);
            stockVolumeRepo.Add(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                return StatusCode(500, "Create new stock volume failed on save");
            }

            stock = await stockVolumeRepo.GetById(stock.Id);
            var result = mapper.Map<StockVolume, ViewStockVolumeDto>(stock);
            return Ok(result);
        }

        [HttpPost("bystockdate")]
        public async Task<IActionResult> UploadByStockDate(IFormFile file)
        {
            var recordAdded = await stockVolumeRepo.RecordByStockDate(file);
            if (recordAdded > 0)
            {
                if (await unitOfWork.CompleteAsync() == false)
                {
                    return StatusCode(500, "Add stock failed on save");
                }
            }
            if (recordAdded == 0)
            {
                return StatusCode(500, "No stock code match in master stock");
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveStockVolumeDto stockVolumeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await stockVolumeRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            stock = mapper.Map(stockVolumeDto, stock);
            stockVolumeRepo.Update(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                return StatusCode(500, "Update stock volume failed on save");
            }

            stock = await stockVolumeRepo.GetById(stock.Id);
            var result = mapper.Map<StockVolume, ViewStockVolumeDto>(stock);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stock = await stockVolumeRepo.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            stockVolumeRepo.Delete(stock);
            if (await unitOfWork.CompleteAsync() == false)
            {
                return StatusCode(500, "Delete stock volume failed on save");
            }
            return Ok(id);
        }
    }
}