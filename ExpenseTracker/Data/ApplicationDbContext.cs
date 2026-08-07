using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();
        
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();
        
        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Balance)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<UserToken>()
            .Property(t=>t.Jti)
            .HasMaxLength(100)
            .IsRequired();
        
        modelBuilder.Entity<UserToken>()
            .HasOne(t => t.User)
            .WithMany(u=>u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Category>()
            .Property(c => c.Description)
            .HasMaxLength(255);
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Description)
            .HasMaxLength(500);
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();
        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}