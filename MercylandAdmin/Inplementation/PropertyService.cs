using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using MercylandAdmin.Utilities;

namespace MercylandAdmin.Inplementation
{
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _context;
        private readonly IDocumentUploadService _document;
        private readonly ILogger<PropertyService> _logger;
        public PropertyService(AppDbContext context, IDocumentUploadService documentUpload, ILogger<PropertyService> logger)
        {
            _context = context;
            _document = documentUpload;
            _logger = logger;
            //_uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            //if (!Directory.Exists(_uploadFolder))
            //{
            //    Directory.CreateDirectory(_uploadFolder);
            //}
        }

        public async Task<ApiResponse<string>> AddProperty(PropertyDTO propertyDTO)
        {

            // Validate propertyDTO  
            if (propertyDTO == null)
            {
                return new ApiResponse<string>
                {
                    Message = "Property data transfer object cannot be null.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            string ImageUrl = null;

            // Handle file upload  
            if (propertyDTO.Image != null && propertyDTO.Image.Length > 0)
            {

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(propertyDTO.Image.FileName).ToLower();

                // Check if the file extension is valid  
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new ApiResponse<string>
                    {
                        Message = "Invalid file type. Only JPG, JPEG, and PNG files are allowed.",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                if (propertyDTO.Image.Length > 824288) // 800 kb limit  
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
                    var uploadResult = await _document.Uploads(propertyDTO.Image);
                    ImageUrl = uploadResult.SecureUrl.ToString();

                    //var propertyImage = Path.GetFileName(propertyDTO.Image.FileName);
                    
                    //filePath = Path.Combine(_document, propertyImage);

                    //// Check for file existence and handle conflicts  
                    //if (System.IO.File.Exists(filePath))
                    //{
                    //    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(propertyImage);
                    //    var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
                    //    filePath = Path.Combine(_document, uniqueFileName);
                    //}

                    //using var stream = new FileStream(filePath, FileMode.Create);
                    //await propertyDTO.Image.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while uploading properties.");
                    // Handle the exception (e.g., log it, rethrow it, etc.)  
                    // For demonstration, we'll just throw it higher up  
                    return new ApiResponse<string>
                    {
                        Message = "An error occurred during the file upload.",
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
            }

            // Create a new user instance and save to database  
            var property = new Property()
            {
                Category = propertyDTO.Category,
                Title = propertyDTO.Title,
                Description = propertyDTO.Description,
                State = propertyDTO.State,
                Address = propertyDTO.Address,
                City = propertyDTO.City,
                Bedroom = propertyDTO.Bedroom,
                Bathroom = propertyDTO.Bathroom,
                Surface = propertyDTO.Surface,
                Year = propertyDTO.Year,
                Price = propertyDTO.Price,
                Image = ImageUrl
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Message = "Property added successfully",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }

        public async Task<ApiResponse<string>> DeleteProperty(int propertyId)
        {
            try
            {
                var response = await _context.Properties.FirstOrDefaultAsync(x => x.PropertyId == propertyId);
                if (response == null)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = $"Property with id {propertyId} not found",
                        StatusCode = HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                _context.Properties.Remove(response);
                await _context.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Property deleted successully",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occured.");
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Property not found",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }
        }

        public async Task<ApiResponse<List<Property>>> GetAllProperties()
        {
            try
            {
                var properties = await _context.Properties.AsNoTracking().ToListAsync();
                if(!properties.Any())
                {
                    return new ApiResponse<List<Property>>
                    {
                        Data = null,
                        Message = "No property found",
                        StatusCode = HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                return new ApiResponse<List<Property>>
                {
                    Data = properties,
                    Message = "Successful",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching properties.");

                return new ApiResponse<List<Property>>
                {
                    Data = null,
                    Message = "An error occurred while fetching properties.",
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ApiResponse<List<Property>>> GetIsFeaturedProperties()
        {
            var response = await _context.Properties.AsNoTracking().Where(x => x.IsFeaturedProperty == true).ToListAsync();
            return new ApiResponse<List<Property>> 
            { 
                Data = response,
                Message = "Successful",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }

        public async Task<ApiResponse<List<int>>> SetIsFeaturedProperties(SetFeaturedPropertiesDTO request)
        {
            try
            {
		var featuredPropertyIds = request.PropertyIds;
                var properties = await _context.Properties.ToListAsync();
                var currentFeaturedProperties = properties.Where(x => x.IsFeaturedProperty == true);
                foreach (var property in currentFeaturedProperties)
                {
                    property.IsFeaturedProperty = false;
                }
                foreach (var id in featuredPropertyIds)
                {
                    foreach (var property in properties)
                    {
                        if (property.PropertyId == id)
                        {
                            property.IsFeaturedProperty = true;
                            break;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return new ApiResponse<List<int>>
                {
                    Data = featuredPropertyIds,
                    Message = "Selected Succefully",
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "An error occured.");
                return new ApiResponse<List<int>>
                {
                    Data = null,
                    Message = "Not Selected",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }
            
        }

        public async Task<ApiResponse<Property>> GetProperty(int id)
        {
            var response = await _context.Properties.AsNoTracking().FirstOrDefaultAsync(x => x.PropertyId == id);
            if(response == null)
            {
                return new ApiResponse<Property>
                {
                    Data = null,
                    Message = $"Property with id {id} not found.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            return new ApiResponse<Property>
            {
                Data = response,
                Message = $"Property with id {id} was retrieved Successfully",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }

        public async Task<ApiResponse<string>> UpdateProperty(int id, PropertyDTO propertyDTO)
        {
            var propertyUpdate = await _context.Properties.FirstOrDefaultAsync(x => x.PropertyId == id);
            if (propertyUpdate == null)
            {
                return new ApiResponse<string>
                {
                    Data= null,
                    Message = $"Property with id {id} not found.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                };
            }


            // Handle file upload  
            if (propertyDTO.Image != null && propertyDTO.Image.Length > 0)
            {
                var uploadResult = await _document.Uploads(propertyDTO.Image);
                propertyUpdate.Image = uploadResult.SecureUrl.ToString();
                //var propertyImage = Path.GetFileName(propertyDTO.Image.FileName);
                //var filePath = Path.Combine(_uploadFolder, propertyImage);

                //using var stream = new FileStream(filePath, FileMode.Create);
                //await propertyDTO.Image.CopyToAsync(stream);

                

            }

            propertyUpdate.Category = propertyDTO.Category;
            propertyUpdate.Title = propertyDTO.Title;
            propertyUpdate.Description = propertyDTO.Description;
            propertyUpdate.State = propertyDTO.State;
            propertyUpdate.Address = propertyDTO.Address;
            propertyUpdate.City = propertyDTO.City;
            propertyUpdate.Bedroom = propertyDTO.Bedroom;
            propertyUpdate.Bathroom = propertyDTO.Bathroom;
            propertyUpdate.Surface = propertyDTO.Surface;
            propertyUpdate.Year = propertyDTO.Year;
            propertyUpdate.Price = propertyDTO.Price;

            await _context.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Data = null,
                Message = "Property updated successfully",
                StatusCode = HttpStatusCode.OK,
                Success = true
            };
        }
    }
}
