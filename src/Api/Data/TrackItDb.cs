using Microsoft.EntityFrameworkCore;

namespace TrackIt.Api.Data;

public class TrackItDb(DbContextOptions<TrackItDb> options) : DbContext(options)
{
    public DbSet<Issue> Issues => Set<Issue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var issue = modelBuilder.Entity<Issue>();
        issue.Property(i => i.Title).HasMaxLength(200);
        issue.Property(i => i.Description).HasMaxLength(4000);
        issue.Property(i => i.Status).HasMaxLength(20);
        issue.Property(i => i.Label).HasMaxLength(20);
        issue.HasIndex(i => i.Status);
    }
}
