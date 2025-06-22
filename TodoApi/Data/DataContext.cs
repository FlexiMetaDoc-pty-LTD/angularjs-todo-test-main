using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure the relationship between User and TodoItem
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.User)
                .WithMany(t => t.TodoItems)
                .HasForeignKey(t => t.UserId);
        }
    }
}
