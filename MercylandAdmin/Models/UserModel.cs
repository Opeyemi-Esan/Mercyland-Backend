using Microsoft.AspNetCore.Identity;

namespace MercylandAdmin.Models
{
    public class UserModel : IdentityUser
   {
        //public int UserId { get; set; } = 0;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        public string ProfileImage { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedOn { get; set; } 
    }
}
