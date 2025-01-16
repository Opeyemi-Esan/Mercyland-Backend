using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MercylandAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        public UsersController(IAuthenticationService authService) 
        { 
            _authService = authService;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registaration([FromForm] UserDTO userDTO)
        {
            var response = await _authService.Registration(userDTO);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO) 
        {
            var response = await _authService.Login(loginDTO);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _authService.GetUsers();
            return StatusCode((int)response.StatusCode, response);
        }

        //[HttpGet]
        //[Route("GetUser/{id}")]
        //public async Task<IActionResult> GetUser(int id)
        //{
        //    var response = await _authService.GetUser(id);
        //    return StatusCode((int)response.StatusCode, response);
        //}

        //[HttpDelete]
        //[Route("delete-user")]
        //public async Task<IActionResult> DeleteUser(int id, string password)
        //{
        //    var response = await _authService.DeleteUser(id, password);
        //    return StatusCode((int)response.StatusCode, response);
        //}

        //[HttpPut("change-password")]
        //public async Task<IActionResult> ChangePassword(int id, string password, string newPassword)
        //{
        //    var response = await _authService.ChangePassword(id, password, newPassword);
        //    return StatusCode((int)response.StatusCode, response);
        //}
    }
}
