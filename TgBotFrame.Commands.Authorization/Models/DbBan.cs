using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TgBotFrame.Commands.Authorization.Models;

public class DbBan : IEntityTypeConfiguration<DbBan>, IEquatable<DbBan>
{
    public Guid Id { get; init; }
#if NET9_0_OR_GREATER
        = Guid.CreateVersion7();
#else
    = TgBotFrame.Utility.UUIDv7.NewUUIDv7Fast();
#endif

    public required long UserId { get; init; }
    public required DateTime Until { get; init; } = DateTime.MaxValue;

    [MaxLength(2048)]
    public string Description { get; init; } = string.Empty;

    public DbUser User { get; init; } = null!;

    public bool Equals(DbBan? other) => other is not null && (ReferenceEquals(this, other) || Id.Equals(other.Id));

    public void Configure(EntityTypeBuilder<DbBan> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.UserId);
        entity.Property(x => x.Until);
        entity.Property(x => x.Description).HasMaxLength(2048).IsUnicode();

        entity.HasOne(x => x.User).WithMany(x => x.Bans).IsRequired().OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.UserId, x.Until }).IsUnique(false);

        entity.HasQueryFilter(x => x.Until > DateTime.UtcNow);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override bool Equals(object? obj) => obj is not null
                                                && (ReferenceEquals(this, obj)
                                                    || (obj.GetType() == GetType() && Equals((DbBan)obj)));
}