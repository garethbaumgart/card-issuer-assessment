using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Infrastructure.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        // Table configuration
        builder.ToTable("Cards");
        
        // Primary key
        builder.HasKey(c => c.Id);
        
        // Properties
        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedNever(); // Domain generates the GUID
            
        builder.Property(c => c.CreditLimit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(c => c.Currency)
            .HasMaxLength(3)
            .IsRequired();
            
        builder.Property(c => c.CreatedAt)
            .IsRequired();
            
        // Indexes for performance (minimal for now)
        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Cards_CreatedAt");
    }
}