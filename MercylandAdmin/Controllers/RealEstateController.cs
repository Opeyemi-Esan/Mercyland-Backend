using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercylandAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealEstateController : ControllerBase
    {
        private readonly IRealEstateService _realEstateService;
        public RealEstateController(IRealEstateService realEstateService)
        {
            _realEstateService = realEstateService;
        }

        [HttpGet("get-all-realestate-datas")]
        public IActionResult GetAllData() 
        {
            var datas = _realEstateService.GetAllData();
            return Ok(datas);
        }

        [HttpPost("add-datas")]
        public IActionResult AddData(RealEstateDTO realEstateDTO)
        {
            var newdata = _realEstateService.AddData(realEstateDTO);
            return Ok(newdata);
        }

    }
}
