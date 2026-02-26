using ControleBruto.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Data
{
    public class ControleBrutoContext : IdentityDbContext<User>
    {
        public ControleBrutoContext(DbContextOptions<ControleBrutoContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
    }
}
