using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using MercylandAdmin.Models;
using MercylandAdmin.Interface;

namespace MercylandAdmin.Utilities
{
    public class DocumentUploadService : IDocumentUploadService
    {

        private readonly Cloudinary _cloudinary;
        public DocumentUploadService(IOptions<AppSettings> config)
        {
            var acc = new Account(
                config.Value.CloudinaryConfig.CloudName,
                config.Value.CloudinaryConfig.ApiKey,
                config.Value.CloudinaryConfig.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> Uploads(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }
       
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
