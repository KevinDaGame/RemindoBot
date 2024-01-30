using Microsoft.EntityFrameworkCore;

namespace DAL;

public class RemindoDbContext : DbContext
{
    public RemindoDbContext()
    {
    }

    public RemindoDbContext(DbContextOptions<RemindoDbContext> options) : base(options)
    {
    }


    public DbSet<Reminder> Reminders { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        var connectionString = "Server=localhost;Port=3306;Database=RemindoDb;User=root;Password=root";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reminder>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Reminder>()
            .Property(r => r.Message).HasMaxLength(256)
            .IsRequired();
    }
}