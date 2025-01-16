using Azure;
using Azure.Core;
using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MercylandAdmin.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    

    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [Authorize]
        [HttpPost("add-property")]
        public async Task<IActionResult> AddProperty(PropertyDTO propertyDTO)
        {
            var response = await _propertyService.AddProperty(propertyDTO);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-all-properties")]
        public async Task<IActionResult> GetAllProperties() 
        {
            var response = await _propertyService.GetAllProperties();
            return StatusCode((int)response.StatusCode, response);
        }
        
        [HttpGet("get-property/{propertyId}")]
        public async Task<IActionResult> GetProperty(int propertyId)
        {
            var response = await _propertyService.GetProperty(propertyId);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize]
        [HttpDelete("delete-property/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
           var response = await _propertyService.DeleteProperty(propertyId);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("update-property/{id}")]
        public async Task<IActionResult> UpdateProperty(int id, PropertyDTO propertyDTO)
        {
            var response = await _propertyService.UpdateProperty(id, propertyDTO);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-featured-properties")]
        public async Task<IActionResult> GetIsFeaturedProperties() 
        {
            var response = await _propertyService.GetIsFeaturedProperties();
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("set-featured-properties")]
        public async Task<IActionResult> SetIsFeaturedProperties(SetFeaturedPropertiesDTO request)
        {
            var response = await _propertyService.SetIsFeaturedProperties(request);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
