using MercylandAdmin.Models;

namespace MercylandAdmin.Interface
{
    public interface IVideoAdvertService
    {
        Task<string> AddVideoAdvert(VideoAdvertDTO videoAdvertDTO);
        Task<VideosAdvert> GetVideoAdvert();
        Task<string> UpdateVideoAdvert(int id, VideoAdvertDTO videoAdvertDTO);

    }
}
