using JWT_Authentication_Authorization.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT_Authentication_Authorization.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) 
        { 

        }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<User> User {  get; set; }
    }
    
}
