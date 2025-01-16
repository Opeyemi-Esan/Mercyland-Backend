using MercylandAdmin.Models;

namespace MercylandAdmin.Interface
{
    public interface IPropertyService
    {
        Task<ApiResponse<string>> AddProperty(PropertyDTO propertyDTO);
        Task<ApiResponse<List<Property>>> GetAllProperties();
        Task<ApiResponse<Property>> GetProperty(int id);
        Task<ApiResponse<string>> UpdateProperty(int id, PropertyDTO propertyDTO);
        Task<ApiResponse<string>> DeleteProperty(int propertyId);
        Task<ApiResponse<List<Property>>> GetIsFeaturedProperties();
        Task<ApiResponse<List<int>>> SetIsFeaturedProperties(SetFeaturedPropertiesDTO request);
    }
}
