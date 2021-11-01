using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) {

        }

        public DbSet<User> Users {  get; set; }    
    }
}
