using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using PinkSea.Database.Models;
using PinkSea.Models;

namespace PinkSea.Database;

/// <summary>
/// The DB context.
/// </summary>
public class PinkSeaDbContext(
    IOptions<PostgresConfig> postgresConfig) : DbContext
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
        const string caseInsensitiveCollation = "case-insensitive";
        
        modelBuilder.UseIdentityColumns();
        modelBuilder.HasCollation(caseInsensitiveCollation, "und-u-ks-level2", provider: "icu", deterministic: false);

        modelBuilder.Entity<TagOekakiRelationModel>(b =>
        {
            b.Property(to => to.TagId)
                .UseCollation(caseInsensitiveCollation);
        });
        
        modelBuilder.Entity<TagModel>(b =>
        {
            b.Property(t => t.Name)
                .UseCollation(caseInsensitiveCollation);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var config = postgresConfig.Value;
        options.UseNpgsql($"Host={config.Hostname};Port={config.Port};Database={config.Database};Username={config.Username};Password={config.Password};Include Error Detail=True");
    }
}