using Microsoft.AspNetCore.Identity;

namespace MoneyManagerUI.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
