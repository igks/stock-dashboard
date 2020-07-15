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

    public class BrokerController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBrokerRepo brokerRepo;
        private readonly IUnitOfWork unitOfWork;

        public BrokerController(IMapper mapper, IBrokerRepo brokerRepo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.brokerRepo = brokerRepo;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brokers = await brokerRepo.GetAll();
            var result = mapper.Map<IEnumerable<ViewBrokerDto>>(brokers);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var broker = await brokerRepo.GetById(id);
            if (broker == null)
            {
                return NotFound();
            }

            var result = mapper.Map<Broker, ViewBrokerDto>(broker);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPage([FromQuery] BrokerParams brokerParams)
        {
            var brokers = await brokerRepo.GetPage(brokerParams);
            var result = mapper.Map<IEnumerable<ViewBrokerDto>>(brokers);

            Response.AddPagination(brokers.CurrentPage, brokers.PageSize, brokers.TotalCount, brokers.TotalPages);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveBrokerDto brokerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var broker = mapper.Map<SaveBrokerDto, Broker>(brokerDto);
            brokerRepo.Add(broker);
            if (await unitOfWork.CompleteAsync() == false)
            {
                return StatusCode(500, "Create new broker failed on save");
            }

            broker = await brokerRepo.GetById(broker.Id);
            var result = mapper.Map<Broker, ViewBrokerDto>(broker);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveBrokerDto brokerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var broker = await brokerRepo.GetById(id);
            if (broker == null)
            {
                return NotFound();
            }

            broker = mapper.Map(brokerDto, broker);
            brokerRepo.Update(broker);
            if (await unitOfWork.CompleteAsync() == false)
            {
                return StatusCode(500, "Update broker failed on save");
            }

            broker = await brokerRepo.GetById(broker.Id);
            var result = mapper.Map<Broker, ViewBrokerDto>(broker);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var broker = await brokerRepo.GetById(id);
            if (broker == null)
            {
                return NotFound();
            }

            brokerRepo.Delete(broker);
            if (await unitOfWork.CompleteAsync() == false)
            {
                return StatusCode(500, "Delete broker failed on save");
            }
            return Ok(id);
        }
    }
}