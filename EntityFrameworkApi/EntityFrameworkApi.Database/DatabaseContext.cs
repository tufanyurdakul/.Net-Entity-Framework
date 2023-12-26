using EntityFrameworkApi.Model.Database.User;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkApi.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserDto> Users { get; set; }
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}