using Microsoft.EntityFrameworkCore;
using TelegramApiProject.User;

namespace TelegramApiProject.Wpf
{
    public class UserContext : DbContext
    {
        public DbSet<UserModel> SearchResult { get; set; }

        public DbSet<UserModel> SendResult { get; set; }

        public UserContext()
        {
            Database.EnsureDeleted();

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = new PathService().UsersPath("users.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
    }
}
