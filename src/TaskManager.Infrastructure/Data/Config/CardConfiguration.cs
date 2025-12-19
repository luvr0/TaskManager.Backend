using TaskManager.Core.BoardAggregate;

namespace TaskManager.Infrastructure.Data.Config;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
  public void Configure(EntityTypeBuilder<Card> builder)
  {
    builder.ToTable("Cards");

    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenIdValueGenerator<AppDbContext, Card, CardId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.Title)
      .HasVogenConversion()
      .HasMaxLength(CardTitle.MaxLength)
      .IsRequired();

    builder.Property(entity => entity.Description)
      .HasConversion(
        v => v.HasValue ? v.Value.Value : null,
        v => v != null ? CardDescription.From(v) : null)
      .HasMaxLength(CardDescription.MaxLength)
      .IsRequired(false);

    builder.Property(entity => entity.ColumnId)
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.CardOrder)
      .HasVogenConversion();

    builder.Property(entity => entity.Status)
      .HasConversion(x => x.Value, x => CardStatus.FromValue(x));
  }
}
