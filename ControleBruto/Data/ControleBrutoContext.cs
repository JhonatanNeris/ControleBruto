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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
