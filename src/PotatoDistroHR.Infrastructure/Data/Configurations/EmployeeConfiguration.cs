using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employee");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.FirstName).HasColumnName("fname").HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasColumnName("lname").HasMaxLength(100).IsRequired();
        builder.Property(e => e.BirthDate).HasColumnName("birthdate").IsRequired();
        builder.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(e => e.Gender).HasColumnName("gender").IsRequired();
        builder.Property(e => e.Contact).HasColumnName("contact").HasMaxLength(50);
        builder.Property(e => e.SupervisorId).HasColumnName("superid");
        builder.Property(e => e.StartDate).HasColumnName("startdate").IsRequired();
        builder.Property(e => e.EndDate).HasColumnName("enddate");
        builder.Property(e => e.Salary).HasColumnName("salary").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.DepartmentId).HasColumnName("department").IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("isactive").HasDefaultValue(true);
        builder.Property(e => e.CreatedDate).HasColumnName("createddate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.ModifiedDate).HasColumnName("modifieddate");
        builder.Property(e => e.CreatedBy).HasColumnName("createdby").HasMaxLength(100);
        builder.Property(e => e.ModifiedBy).HasColumnName("modifiedby").HasMaxLength(100);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Supervisor)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.DepartmentId);
        builder.HasIndex(e => e.SupervisorId);
    }
}
