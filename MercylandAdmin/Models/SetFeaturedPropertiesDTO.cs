using Microsoft.AspNetCore.Identity;

namespace MercylandAdmin.Models
{
    public class SetFeaturedPropertiesDTO
   {
        public List<int> PropertyIds { get; set; } = new List<int>();
    }
}
