using Microsoft.AspNetCore.Identity;

namespace ControleBruto.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }

        public User() : base()
        {

        }
    }
}
