using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(d => d.Hotline).HasColumnName("hotline").HasMaxLength(50);
        builder.Property(d => d.ManagerId).HasColumnName("managerid");
        builder.Property(d => d.IsActive).HasColumnName("isactive").HasDefaultValue(true);
        builder.Property(d => d.CreatedDate).HasColumnName("createddate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(d => d.ModifiedDate).HasColumnName("modifieddate");
        builder.Property(d => d.CreatedBy).HasColumnName("createdby").HasMaxLength(100);
        builder.Property(d => d.ModifiedBy).HasColumnName("modifiedby").HasMaxLength(100);

        builder.HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.Name);
    }
}
