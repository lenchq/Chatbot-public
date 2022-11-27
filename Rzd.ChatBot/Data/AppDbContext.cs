using Microsoft.EntityFrameworkCore;
using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Data;

public class AppDbContext : DbContext
{
    public DbSet<Form> Forms { get; set; }

    public AppDbContext()
        : base()
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}