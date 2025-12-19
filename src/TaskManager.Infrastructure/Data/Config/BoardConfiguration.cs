using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Infrastructure.Data.Config;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
  public void Configure(EntityTypeBuilder<Board> builder)
  {
    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenIdValueGenerator<AppDbContext, Board, BoardId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.Name)
      .HasVogenConversion()
      .HasMaxLength(BoardName.MaxLength)
      .IsRequired();

    builder.Property(entity => entity.UserId)
      .HasVogenConversion()
      .IsRequired();

    builder.OwnsMany(b => b.Members, mb =>
    {
      mb.ToTable("BoardMembers");
      mb.WithOwner().HasForeignKey("BoardId");
      mb.Property<int>("Id")
        .ValueGeneratedOnAdd();
      mb.HasKey("Id");

      mb.Property(member => member.UserId)
        .HasColumnName("UserId")
        .HasVogenConversion()
        .IsRequired();

      mb.Property(member => member.Role)
        .HasColumnName("Role")
        .HasConversion(
          r => r.Value,
          v => BoardRole.From(v))
        .HasMaxLength(BoardRole.MaxLength)
        .IsRequired();

      mb.Ignore(member => member.Permissions);

      mb.HasIndex("BoardId", "UserId").IsUnique();
    });

    builder.Navigation(b => b.Members)
      .HasField("_members")
      .UsePropertyAccessMode(PropertyAccessMode.Field);

    // relationships configured in ColumnConfiguration
  }
}
