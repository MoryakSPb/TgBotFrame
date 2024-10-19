using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TgBotFrame.Data;

namespace TgBotFrame.Commands.Authorization.Models;

public class DbRole : IModelEntityScheme<DbRole>
{
    public int Id { get; init; }

    [Length(0, 32)]
    public required string Name { get; init; } = string.Empty;

    public bool MentionEnabled { get; set; }

    public IList<DbRoleMember> Members { get; init; } = null!;

    public static void EntityBuild(EntityTypeBuilder<DbRole> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasAlternateKey(x => x.Name);

        entity.Property(x => x.Name).HasMaxLength(32).IsUnicode();
        entity.Property(x => x.MentionEnabled);

        entity.HasMany(x => x.Members)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(new DbRole { Id = -1, Name = @"admin" });
    }

    public override int GetHashCode() => Id;
}