using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("account");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.EmployeeId).HasColumnName("employeeid").IsRequired();
        builder.Property(a => a.PasswordHash).HasColumnName("password").HasMaxLength(500).IsRequired();
        builder.Property(a => a.IsAdmin).HasColumnName("isadmin").HasDefaultValue(false);
        builder.Property(a => a.IsActive).HasColumnName("isactive").HasDefaultValue(true);
        builder.Property(a => a.CreatedDate).HasColumnName("createddate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(a => a.ModifiedDate).HasColumnName("modifieddate");
        builder.Property(a => a.LastLoginDate).HasColumnName("lastlogindate");
        builder.Property(a => a.CreatedBy).HasColumnName("createdby").HasMaxLength(100);
        builder.Property(a => a.ModifiedBy).HasColumnName("modifiedby").HasMaxLength(100);

        builder.HasOne(a => a.Employee)
            .WithOne(e => e.Account)
            .HasForeignKey<Account>(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.EmployeeId).IsUnique();
    }
}
