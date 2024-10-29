namespace TgBotFrame.Commands.Authorization.Interfaces;

public interface IAuthorizationData
{
    DbSet<DbRole> Roles { get; }
    DbSet<DbRoleMember> RoleMembers { get; }
    DbSet<DbBan> Bans { get; }
    DbSet<DbUser> Users { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> IsUserIdBanned(long userId, CancellationToken cancellationToken = default) => Bans
        .AsNoTracking()
        .IgnoreQueryFilters()
        .AnyAsync(x =>
                x.UserId == userId
                && x.Until > DateTime.UtcNow,
            cancellationToken);

    static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbRole>(DbRole.EntityBuild);
        modelBuilder.Entity<DbRoleMember>(DbRoleMember.EntityBuild);
        modelBuilder.Entity<DbUser>(DbUser.EntityBuild);
        modelBuilder.Entity<DbBan>(DbBan.EntityBuild);
    }
}