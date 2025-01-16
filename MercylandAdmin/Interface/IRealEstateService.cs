using MercylandAdmin.Models;

namespace MercylandAdmin.Interface
{
    public interface IRealEstateService
    {
        Task<string> AddData(RealEstateDTO realEstateDTO);
        Task<List<RealEstate>> GetAllData();
        Task<RealEstate> EditDatas(int id);
    }
}
