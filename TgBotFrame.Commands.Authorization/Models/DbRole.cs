using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TgBotFrame.Commands.Authorization.Models;

public class DbRole : IEntityTypeConfiguration<DbRole>, IEquatable<DbRole>
{
    public int Id { get; init; }

    [MaxLength(32)]
    public required string Name { get; init; } = string.Empty;

    public bool MentionEnabled { get; set; }

    public IList<DbUser> Members { get; init; } = null!;

    public void Configure(EntityTypeBuilder<DbRole> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasAlternateKey(x => x.Name);

        entity.Property(x => x.Name).HasMaxLength(32).IsUnicode(false);
        entity.Property(x => x.MentionEnabled);

        entity.HasMany(x => x.Members).WithMany(x => x.Roles).UsingEntity<DbRoleMember>();

        entity.HasData(new() { Id = -1, Name = @"admin" }, new() { Id = -2, Name = @"ban_list" });
    }

    public bool Equals(DbRole? other) => other is not null && (ReferenceEquals(this, other) || Id == other.Id);

    public override int GetHashCode() => Id;

    public override bool Equals(object? obj) => obj is not null
                                                && (ReferenceEquals(this, obj)
                                                    || (obj.GetType() == GetType() && Equals((DbRole)obj)));

    public override string ToString() => Name;
}