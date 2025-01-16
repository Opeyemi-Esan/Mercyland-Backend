using Azure;
using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercylandAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeSliderController : ControllerBase
    {
        private readonly IHomeSliderService _homeSliderService;
        public HomeSliderController (IHomeSliderService homeSliderService)
        {
            _homeSliderService = homeSliderService;
        }

        [Authorize]
        [HttpPost("add-homeslider-image")]
        public async Task<IActionResult> AddImage(HomeSliderDTO homeSliderDTO)
        {
            var response = await _homeSliderService.AddImage(homeSliderDTO);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-all-sliderimage")]
        public async Task<IActionResult> GetAllImageSlider()
        {
            var response = await _homeSliderService.GetAllImageSlider();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-sliderimagebyid/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var response = await _homeSliderService.GetImageById(imageId);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize]
        [HttpDelete("remove-sliderimage/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
           var response = await _homeSliderService.RemoveImage(imageId);
           return StatusCode((int)response.StatusCode, response);
        }
    }
}
