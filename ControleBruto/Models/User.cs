using Microsoft.AspNetCore.Identity;

namespace ControleBruto.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public User() : base() { }
    }
}
