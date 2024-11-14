using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TgBotFrame.Commands.Authorization.Models;

public class DbRoleMember : IEntityTypeConfiguration<DbRoleMember>, IEquatable<DbRoleMember>
{
    public required int RoleId { get; init; }
    public required long UserId { get; init; }

    public DbRole Role { get; init; } = null!;
    public DbUser User { get; init; } = null!;

    public void Configure(EntityTypeBuilder<DbRoleMember> entity)
    {
        entity.HasKey(x => new { x.RoleId, x.UserId });

        entity.HasIndex(x => x.RoleId).IsUnique(false);
        entity.HasIndex(x => x.UserId).IsUnique(false);

        entity.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    public bool Equals(DbRoleMember? other) => other is not null
                                               && (ReferenceEquals(this, other)
                                                   || (RoleId == other.RoleId && UserId == other.UserId));

    public override int GetHashCode() => HashCode.Combine(RoleId, UserId);

    public override bool Equals(object? obj) =>
        obj is not null
        && (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((DbRoleMember)obj)));
}