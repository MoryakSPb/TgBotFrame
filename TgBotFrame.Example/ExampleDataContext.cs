using Microsoft.EntityFrameworkCore;
using TgBotFrame.Commands.Authorization.Interfaces;
using TgBotFrame.Commands.Authorization.Models;

namespace TgBotFrame.Example;

public class ExampleDataContext(DbContextOptions<ExampleDataContext> options)
    : DbContext(options), IAuthorizationData
{
    public DbSet<DbRole> Roles { get; set; } = null!;
    public DbSet<DbRoleMember> RoleMembers { get; set; } = null!;
    public DbSet<DbBan> Bans { get; set; } = null!;
    public DbSet<DbUser> Users { get; set; } = null!;

    Task IAuthorizationData.SaveChangesAsync(CancellationToken cancellationToken) =>
        base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        IAuthorizationData.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DbRole>(entity =>
        {
            entity.HasData(new DbRole
            {
                Id = -100,
                Name = "sum",
                MentionEnabled = true,
            });
        });
    }
}