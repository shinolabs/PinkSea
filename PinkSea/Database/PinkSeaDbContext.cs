using Microsoft.EntityFrameworkCore;
using PinkSea.Database.Models;

namespace PinkSea.Database;

/// <summary>
/// The DB context.
/// </summary>
public class PinkSeaDbContext : DbContext
{
    /// <summary>
    /// The oekaki table.
    /// </summary>
    public DbSet<OekakiModel> Oekaki { get; set; } = null!;
    
    /// <summary>
    /// The tags table.
    /// </summary>
    public DbSet<TagModel> Tags { get; set; } = null!;
    
    /// <summary>
    /// The tag/oekaki relations.
    /// </summary>
    public DbSet<TagOekakiRelationModel> TagOekakiRelations { get; set; } = null!;
    
    /// <summary>
    /// The user table.
    /// </summary>
    public DbSet<UserModel> Users { get; set; } = null!;
    
    /// <summary>
    /// The Oekaki table.
    /// </summary>
    public DbSet<OAuthStateModel> OAuthStates { get; set; } = null!;

    /// <summary>
    /// The Oekaki table.
    /// </summary>
    public DbSet<ConfigurationModel> Configuration { get; set; } = null!;
    
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OekakiModel>();
        modelBuilder.Entity<TagModel>();
        modelBuilder.Entity<TagOekakiRelationModel>();
        modelBuilder.Entity<UserModel>();
        modelBuilder.Entity<OAuthStateModel>();
        modelBuilder.Entity<ConfigurationModel>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=pinksea.db");
}