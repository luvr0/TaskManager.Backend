using TaskManager.Core.BoardAggregate;

namespace TaskManager.Infrastructure.Data.Config;

public class ColumnConfiguration : IEntityTypeConfiguration<Column>
{
  public void Configure(EntityTypeBuilder<Column> builder)
  {
    builder.ToTable("Columns");

    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenIdValueGenerator<AppDbContext, Column, ColumnId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.Name)
      .HasVogenConversion()
      .HasMaxLength(ColumnName.MaxLength)
      .IsRequired();

    builder.Property(entity => entity.BoardId)
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.ColumnOrder)
      .HasVogenConversion();
  }
}
