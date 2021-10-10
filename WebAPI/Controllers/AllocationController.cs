using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.ViewModel;
using Service;
using Common;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AllocationController :  ControllerBase
    {
        private readonly IAllocationService _allocationService;
        public AllocationController(IAllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        [HttpGet]        
        public ActionResult<string> Get()
        {            
            return Ok("Hello Welcome to Commercial Allocation System");
        }

        [HttpGet]
        [Route("{model}")]
        public ActionResult<BreaksCollectionViewModel> GetAllocation(string model)
        {
            if (model == Constants.ModelType.Model1)
            {
                var result = _allocationService.AllocateCommercials(Constants.ModelType.Model1);
                return Ok(result);
            }
            if (model == Constants.ModelType.Model2)
            {
                var result = _allocationService.AllocateCommercials(Constants.ModelType.Model2);
                return Ok(result);
            }
            else
              return BadRequest(Constants.ExceptionMessage.BadRequestMessage);
        }   
    }
}
