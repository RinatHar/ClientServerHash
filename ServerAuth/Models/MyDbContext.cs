using Microsoft.EntityFrameworkCore;

namespace ServerHash.Models
{
    public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
    : base(options)
    { }

    public DbSet<Data> Data { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Right> Rights { get; set; }

    public DbSet<UserRight> UsersRights { get; set; }
}
}
