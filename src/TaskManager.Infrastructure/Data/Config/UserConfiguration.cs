using TaskManager.Core.UserAggregate;

namespace TaskManager.Infrastructure.Data.Config;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenIdValueGenerator<AppDbContext, User, UserId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.Name)
      .HasVogenConversion()
      .HasMaxLength(UserName.MaxLength)
      .IsRequired();

    builder.Property(entity => entity.Email)
      .HasVogenConversion()
      .HasMaxLength(UserEmail.MaxLength)
      .IsRequired();

    builder.HasIndex(entity => entity.Email)
      .IsUnique();

    builder.Property(entity => entity.PasswordHash)
      .HasVogenConversion()
      .HasMaxLength(UserPassword.MaxLength)
      .IsRequired();

    builder.Property(entity => entity.RefreshToken)
      .HasConversion(
        v => v != null ? v.Value.Value : null,
        v => v != null ? RefreshToken.From(v) : null
      )
      .HasMaxLength(RefreshToken.MaxLength)
      .IsRequired(false);

    builder.Property(entity => entity.RefreshTokenExpiryTime)
      .IsRequired(false);

  }
}
