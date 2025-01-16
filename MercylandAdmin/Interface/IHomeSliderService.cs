using MercylandAdmin.Models;

namespace MercylandAdmin.Interface
{
    public interface IHomeSliderService
    {
        Task<ApiResponse<string>> AddImage(HomeSliderDTO homeSliderDTO);
        Task<ApiResponse<string>> RemoveImage(int imageId);
        Task<ApiResponse<HomeSlider>> GetImageById(int imageId);
        Task<ApiResponse<List<HomeSlider>>> GetAllImageSlider();
    }
}
