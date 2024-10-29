namespace TgBotFrame.Commands.Authorization.Interfaces;

public interface IAuthorizationData
{
    DbSet<DbRole> Roles { get; }
    DbSet<DbRoleMember> RoleMembers { get; }

    Task SaveChangesAsync();

    static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbRole>(DbRole.EntityBuild);
        modelBuilder.Entity<DbRoleMember>(DbRoleMember.EntityBuild);
    }
}