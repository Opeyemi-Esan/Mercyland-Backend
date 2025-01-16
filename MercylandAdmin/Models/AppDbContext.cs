using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MercylandAdmin.Models
{
    public class AppDbContext : IdentityDbContext<UserModel>
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        //public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<HomeSlider> HomeSliders { get; set; }
        public DbSet<RealEstate> RealEstates { get; set; }
        public DbSet<DelightBankDetail> DelightBankDetails { get; set; }
        public DbSet<VideosAdvert> VideosAdverts {  get; set; }
    }
}
