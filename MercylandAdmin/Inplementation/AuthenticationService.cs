using Azure.Core;
using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MercylandAdmin.Inplimentation
{
    public class AuthenticationService : IAuthenticationService
    { 


         private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IConfiguration _configuration;
        //private readonly AppDbContext _dbContext;
        private readonly IDocumentUploadService _document;
        //private readonly string _uploadFolder;
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(UserManager<UserModel> usermanager, SignInManager<UserModel> signInManager, ILogger<AuthenticationService> logger, IDocumentUploadService document, IConfiguration configuration) 
        { 
            //_dbContext = dbContext;
            _logger = logger;
            _document = document;
            _userManager = usermanager;
            _signInManager = signInManager;
            _configuration = configuration;
            //_uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            //if (!Directory.Exists(_uploadFolder))
            //{
            //    Directory.CreateDirectory(_uploadFolder);
            //}
        }

        //public async Task<ApiResponse<IdentityResult>> GetUser(int id)
        //{
        //    try
        //    {
        //        var user = await _dbContext.Users.Where(user => user.Id == id).FirstOrDefaultAsync();
        //        if(user == null)
        //        {
        //            return new ApiResponse<IdentityResult>
        //            {
        //                Data = null,
        //                Message = $"User with id {id} not found",
        //                StatusCode = HttpStatusCode.NotFound,
        //                Success = false
        //            };
        //        }
        //        return new ApiResponse<IdentityResult>
        //        {
        //            Data = user,
        //            Message = "Successful",
        //            StatusCode = HttpStatusCode.OK,
        //            Success = true
        //        };
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex, "An Error occured while fetching data");
        //        return new ApiResponse<IdentityResult>
        //        {
        //            Data = null,
        //            Message = "An Error occured while fetching data",
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            Success = false
        //        };
        //    }
            
        //}

        public async Task<ApiResponse<List<UserModel>>> GetUsers()
        {
            try
            {
                var user = await _userManager.Users.ToListAsync();
                return new ApiResponse<List<UserModel>>
                {
                    Data = user,
                    Message = "Successful",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An Error occured while fetching data");
                return new ApiResponse<List<UserModel>>
                {
                    Data = null,
                    Message = "An Error occured while fetching data",
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        

        public async Task<ApiResponse<IdentityResult>> Registration(UserDTO userDTO)
        {
            //var user = await _dbContext.Users.FirstOrDefaultAsync();
            //if(user.Email == userDTO.Email)
            //{
            //    return new ApiResponse<string>
            //    {
            //        Message = "User Already Exist",
            //        StatusCode = HttpStatusCode.BadRequest,
            //        Success = false
            //    };
            //}

                string ImageUrl = null;

                // Handle file upload  
                if (userDTO.ProfileImage != null && userDTO.ProfileImage.Length > 0)
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(userDTO.ProfileImage.FileName).ToLower();

                    // Check if the file extension is valid  
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return new ApiResponse<IdentityResult>
                        {
                            Message = "Invalid file type. Only JPG, JPEG, and PNG files are allowed.",
                            StatusCode = HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }
                    if(userDTO.ProfileImage.Length> 524288)
                    {
                        return new ApiResponse<IdentityResult>
                        {
                            Message = "File too large.",
                            StatusCode = HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }

                    try
                    {
                        var uploadResult = await _document.Uploads(userDTO.ProfileImage);
                        ImageUrl = uploadResult.SecureUrl.ToString();
                        //    var userImage = Path.GetFileName(userDTO.ProfileImage.FileName);
                        //    filePath = Path.Combine(_uploadFolder, userImage);

                        //    // Check for file existence and handle conflicts  
                        //    if (System.IO.File.Exists(filePath))
                        //    {
                        //        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(userImage);
                        //        var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
                        //        filePath = Path.Combine(_uploadFolder, uniqueFileName);
                        //    }

                        //    using var stream = new FileStream(filePath, FileMode.Create);
                        //    userDTO.ProfileImage.CopyTo(stream);
                    }
                    catch (IOException ex)
                    {
                        _logger.LogError(ex, "An error occurred during the file upload.");
                        // Handle the exception (e.g., log it, rethrow it, etc.)  
                        // For demonstration, we'll just throw it higher up  
                        return new ApiResponse<IdentityResult>
                        {
                            Message = "An error occurred during the file upload.",
                            StatusCode = HttpStatusCode.InternalServerError,
                            Success = false
                        };
                    }
                }

                var users = new UserModel()
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    UserName = userDTO.Email,
                    ProfileImage = ImageUrl,
                    IsActive = 1,
                    CreatedOn = DateTime.Now
                };
                var result = await _userManager.CreateAsync(users, userDTO.Password);

                //if (!result.Succeeded)
                //{
                //    return new ApiResponse<IdentityResult>
                //    {
                //        Message = "Registration failed",
                //        StatusCode = HttpStatusCode.BadRequest,
                //        Success = false
                //    };
                //}

                return new ApiResponse<IdentityResult>
                {
                    Data = result,
                    Message = "Registered Successfully!",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
           

        }

        public async Task<ApiResponse<string>> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false);
                if (!user.Succeeded)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Enter valid credentials",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }


               var authClaims = new List<Claim>
               {
                   new Claim(ClaimTypes.Name, loginDTO.Email),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
               };

                var jwtSecret = _configuration["JwtSettings:Secret"];
                if (string.IsNullOrEmpty(jwtSecret))
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "JwtSettings.Secret configuration value is missing.",
                        StatusCode = HttpStatusCode.InternalServerError,
                        Success = false
                    };
                }

                var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:ValidIssuer"],
                    audience: _configuration["JwtSettings:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                    );

                var result = new JwtSecurityTokenHandler().WriteToken(token);

                return new ApiResponse<string>
                {
                    Data = result,
                    Message = "Successful",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured while Logging in");
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Login Error",
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        //public async Task<ApiResponse<User>> ChangePassword(int id, string password, string newPassword)
        //{
        //    try
        //    {
        //        var user = await _dbContext.Users.Where(x => x.UserId == id && x.Password == password).FirstOrDefaultAsync();
        //        if (user == null)
        //        {
        //            return new ApiResponse<User>
        //            {
        //                Data = null,
        //                Message = "User not found",
        //                StatusCode = HttpStatusCode.NotFound,
        //                Success = false
        //            };
        //        }
        //        user.Password = newPassword;
        //        await _dbContext.SaveChangesAsync();
        //        return new ApiResponse<User>
        //        {
        //            Data = user,
        //            Message = "Password Changed Successfully!",
        //            StatusCode = HttpStatusCode.OK,
        //            Success = true
        //        };
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex, "Error Occured While changing password");
        //        return new ApiResponse<User>
        //        {
        //            Data = null,
        //            Message = "Error Occured while changing password",
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            Success = false
        //        };
        //    }

        //}

        //public async Task<ApiResponse<string>> DeleteUser(int id, string password)
        //{
        //    var user = await _dbContext.Users.Where(u => u.UserId == id && u.Password == password).FirstOrDefaultAsync();

        //    if(user == null)
        //    {
        //        return new ApiResponse<string>
        //        {
        //            Data = null,
        //            Message = "User not found",
        //            StatusCode = HttpStatusCode.NotFound,
        //            Success = false
        //        };
        //    }
        //        _dbContext.Users.Remove(user);
        //        _dbContext.SaveChanges();
        //    return new ApiResponse<string>
        //    {
        //        Message = "Account deleted successfully!",
        //        StatusCode = HttpStatusCode.OK,
        //        Success = true
        //    };

        //}
    }
}
