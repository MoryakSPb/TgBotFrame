using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TgBotFrame.Commands.Authorization.Models;

public class DbUser : IEquatable<DbUser>
{
    public required long Id { get; init; }

    [MaxLength(32)]
    public string? UserName { get; set; }

    [MaxLength(64)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(64)]
    public string? LastName { get; set; } = string.Empty;

    public IList<DbBan> Bans { get; set; } = null!;
    public IList<DbRole> Roles { get; init; } = null!;

    public bool Equals(DbUser? other) => other is not null && (ReferenceEquals(this, other) || Id == other.Id);

    public static void EntityBuild(EntityTypeBuilder<DbUser> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id).ValueGeneratedNever();
        entity.Property(x => x.UserName).HasMaxLength(32).IsUnicode();
        entity.Property(x => x.FirstName).HasMaxLength(64).IsUnicode();
        entity.Property(x => x.LastName).HasMaxLength(64).IsUnicode();

        entity.HasIndex(x => x.UserName).IsUnique();

        entity.HasMany(x => x.Roles).WithMany(x => x.Members).UsingEntity<DbRoleMember>();
        entity.HasMany(x => x.Bans).WithOne(x => x.User).HasForeignKey(x => x.UserId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => GetUserDisplayText(UserName, FirstName, LastName);

    public static string GetUserDisplayText(string? userName, string firstName, string? lastName)
    {
        lastName = lastName?.Length > 0 ? @" " + lastName : string.Empty;
        return userName is not null
            ? @$"@{userName} ({firstName}{lastName})"
            : @$"{firstName}{lastName}";
    }

    public override bool Equals(object? obj) => obj is not null
                                                && (ReferenceEquals(this, obj)
                                                    || (obj.GetType() == GetType() && Equals((DbUser)obj)));
}