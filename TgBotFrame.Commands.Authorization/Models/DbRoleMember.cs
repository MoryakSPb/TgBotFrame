using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TgBotFrame.Data;

namespace TgBotFrame.Commands.Authorization.Models;

public class DbRoleMember : IModelEntityScheme<DbRoleMember>
{
    public required int RoleId { get; init; }
    public required long UserId { get; init; }
    public DateTime CreateTime { get; init; } = DateTime.UtcNow;
    public required long? CreatedBy { get; init; }

    public DbRole Role { get; init; } = null!;

    public static void EntityBuild(EntityTypeBuilder<DbRoleMember> entity)
    {
        entity.HasKey(x => new { x.RoleId, x.UserId });
        entity.HasIndex(x => x.RoleId).IsUnique(false);

        entity.Property(x => x.CreateTime);
        entity.Property(x => x.CreatedBy);

        entity.HasOne(x => x.Role).WithMany(x => x.Members).HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override int GetHashCode() => RoleId;
}