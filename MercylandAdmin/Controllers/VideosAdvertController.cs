using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercylandAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosAdvertController : ControllerBase
    {
        private readonly IVideoAdvertService _videoAdvertService;
        public VideosAdvertController(IVideoAdvertService videoAdvertService)
        {
            _videoAdvertService = videoAdvertService;
        }

        [HttpPost("upload-videoads")]
        public async Task<IActionResult> AddVideoAdvert(VideoAdvertDTO videoAdvertDTO)
        {
            var response = await _videoAdvertService.AddVideoAdvert(videoAdvertDTO);
            return Ok(response);
        }

        [HttpPost("get-videosads")]
        public async Task<IActionResult> GetVideoAdvert()
        {
            var response = await _videoAdvertService.GetVideoAdvert();
            return Ok(response);
        }

        [HttpPut("update-videoads")]
        public async Task<IActionResult> UpdateVideoAdvert(int id, VideoAdvertDTO videoAdvertDTO)
        {
            var response = await _videoAdvertService.UpdateVideoAdvert(id, videoAdvertDTO);
            return Ok(response);
        }
    }
}
