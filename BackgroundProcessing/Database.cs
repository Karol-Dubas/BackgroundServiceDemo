using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BackgroundProcessing;

public class Database : DbContext
{
    public Database(DbContextOptions<Database> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
}

public class User
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}