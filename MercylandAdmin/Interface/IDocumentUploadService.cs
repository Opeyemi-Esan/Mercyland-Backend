using CloudinaryDotNet.Actions;

namespace MercylandAdmin.Interface
{
    public interface IDocumentUploadService
    {
        Task<ImageUploadResult> Uploads(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
