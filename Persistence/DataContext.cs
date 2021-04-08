using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    //Inheriting from IdentityDbContext will add necessary tables to DB related to SignIn and LogIn.
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //Not necessary to add AppUser because it is done automatically by base class.
        public DbSet<Activity> Activities { get; set; }
    }
}