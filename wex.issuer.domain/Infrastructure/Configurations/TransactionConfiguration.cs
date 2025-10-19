using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        // Table configuration
        builder.ToTable("Transactions");
        
        // Primary key
        builder.HasKey(t => t.Id);
        
        // Properties
        builder.Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedNever(); // Domain generates the GUID
            
        builder.Property(t => t.CardId)
            .IsRequired();
            
        builder.Property(t => t.Description)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(t => t.TransactionDate)
            .IsRequired();
            
        builder.Property(t => t.PurchaseAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(t => t.CreatedAt)
            .IsRequired();
            
        // Relationships
        builder.HasOne(t => t.Card)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CardId)
            .OnDelete(DeleteBehavior.Cascade); // When card is deleted, delete all transactions
            
        // Index for performance
        builder.HasIndex(t => t.CardId)
            .HasDatabaseName("IX_Transactions_CardId");
    }
}