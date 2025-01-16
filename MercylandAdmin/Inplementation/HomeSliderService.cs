using Azure;
using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MercylandAdmin.Inplementation
{
    public class HomeSliderService : IHomeSliderService
    {
        private readonly AppDbContext _Context;
        private readonly IDocumentUploadService _document;
        private readonly ILogger<HomeSliderService> _logger;
        //private readonly string _uploadFolder;

        public HomeSliderService(AppDbContext context, IDocumentUploadService documentUpload, ILogger<HomeSliderService> logger)
        {
            _Context = context;
            _document = documentUpload;
            _logger = logger;
            //_uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            //if (Directory.Exists(_uploadFolder))
            //{
            //    Directory.CreateDirectory(_uploadFolder);
            //}

        }
        public async Task<ApiResponse<string>> AddImage(HomeSliderDTO homeSliderDTO)
        {

            if (homeSliderDTO == null)
            {
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Property data transfer object cannot be null.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            string imageUrlDesktop = null;
            string imageUrlMobile = null;

            if (homeSliderDTO.ImageDesktop != null && homeSliderDTO.ImageDesktop.Length>0) 
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtensionDesktop = Path.GetExtension(homeSliderDTO.ImageDesktop.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtensionDesktop))
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Invalid file type. Only JPG, JPEG, and PNG files are allowed.",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                if (homeSliderDTO.ImageDesktop.Length > 524288) // 500 kb limit  
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "File size too large",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                try
                {
                    
                    var uploadResult = await _document.Uploads(homeSliderDTO.ImageDesktop);
                    imageUrlDesktop = uploadResult.SecureUrl.ToString();
                    //var homeSliderImage = Path.GetFileName(homeSliderDTO.Image.FileName);
                    //filePath = Path.Combine(_uploadFolder, homeSliderImage);


                    //if (System.IO.File.Exists(filePath))
                    //{
                    //    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(homeSliderImage);
                    //    var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
                    //    filePath = Path.Combine(_uploadFolder, uniqueFileName);
                    //}

                    //using var stream = new FileStream(filePath, FileMode.Create);
                    //await homeSliderDTO.Image.CopyToAsync(stream);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "An error occured while uploading Image");
                    // Handle the exception (e.g., log it, rethrow it, etc.)  
                    // For demonstration, we'll just throw it higher up 
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "An error occurred during the file upload.",
                        StatusCode = HttpStatusCode.InternalServerError,
                        Success = false
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while uploading HomeSLider Image");
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "An error occurred during the file upload.",
                        StatusCode = HttpStatusCode.InternalServerError,
                        Success = false
                    };
                }

            }

            if (homeSliderDTO.ImageMobile != null && homeSliderDTO.ImageMobile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtensionDesktop = Path.GetExtension(homeSliderDTO.ImageMobile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtensionDesktop))
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Invalid file type. Only JPG, JPEG, and PNG files are allowed.",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                if (homeSliderDTO.ImageMobile.Length > 824288) // 800 kb limit  
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "File size too large",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                try
                {
                    var uploadResult = await _document.Uploads(homeSliderDTO.ImageMobile);
                    imageUrlMobile = uploadResult.SecureUrl.ToString();
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "An error occured while uploading Image");
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "An error occurred during the file upload.",
                        StatusCode = HttpStatusCode.InternalServerError,
                        Success = false
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while uploading HomeSLider Image");
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "An error occurred during the file upload.",
                        StatusCode = HttpStatusCode.InternalServerError,
                        Success = false
                    };
                }

            }

            var sliderImage = new HomeSlider()
            {
                ImageDesktop = imageUrlDesktop,
                ImageMobile = imageUrlMobile,
            };

            _Context.HomeSliders.Add(sliderImage);
            await _Context.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Data = null,
                Message = "Image Added Successully",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }

        public async Task<ApiResponse<List<HomeSlider>>> GetAllImageSlider()
        {
            try
            {
                var response = await _Context.HomeSliders.AsNoTracking().ToListAsync();
                if (!response.Any())
                {
                    return new ApiResponse<List<HomeSlider>>
                    {
                        Data = null,
                        Message = "No Image found, make sure you have images uploaded",
                        StatusCode = HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                return new ApiResponse<List<HomeSlider>>
                {
                    Data = response,
                    Message = "Images retrieved successfully",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occured while fetching HomeSLider Image");
                return new ApiResponse<List<HomeSlider>>
                {
                    Data = null,
                    Message = "Error fetching datas",
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
            
        }

        public async Task<ApiResponse<HomeSlider>> GetImageById(int imageId)
        {
            var response = await _Context.HomeSliders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == imageId);
            if (response == null)
            {
                return new ApiResponse<HomeSlider>
                {
                    Data = null,
                    Message = $"Image with id {imageId} is not found",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }
            return new ApiResponse<HomeSlider>
            {
                Data = response,
                Message = "Images retrieved successfully",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }

        public async Task<ApiResponse<string>> RemoveImage(int imageId)
        {
            var sliderImage = await _Context.HomeSliders.FirstOrDefaultAsync(s => s.Id == imageId);
            if (sliderImage == null)
            {
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Image with id {imageId} not found.",
                    StatusCode = HttpStatusCode.NotFound,
                    Success = false
                };
            }

            _Context.HomeSliders.Remove(sliderImage);
            await _Context.SaveChangesAsync();

            // Make sure to await this  

            return new ApiResponse<string>
            {
                Data = null,
                Message = $"Image with id {imageId} deleted successfully.",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }
    }
}
