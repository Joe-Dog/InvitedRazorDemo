using Microsoft.EntityFrameworkCore;
using RazorDemo.Models;

namespace RazorDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ListBoxItem> ListBoxItems { get; set; }

}
