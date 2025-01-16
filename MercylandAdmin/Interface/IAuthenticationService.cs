using MercylandAdmin.Models;
using Microsoft.AspNetCore.Identity;

namespace MercylandAdmin.Interface
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
        Task<ApiResponse<IdentityResult>> Registration(UserDTO signUpDTO);
        Task<ApiResponse<List<UserModel>>> GetUsers();
        //Task<ApiResponse<IdentityResult>> GetUser(int id);
        //Task<ApiResponse<string>> ChangePassword(int id, string password, string newPassword);
        //Task<ApiResponse<string>> DeleteUser(int id, string password);
    }
}
