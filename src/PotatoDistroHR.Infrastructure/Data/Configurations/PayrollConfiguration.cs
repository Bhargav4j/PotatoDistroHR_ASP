using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Infrastructure.Data.Configurations;

public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.ToTable("payroll");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.EmployeeId).HasColumnName("employeeid").IsRequired();
        builder.Property(p => p.BaseSalary).HasColumnName("basesalary").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Bonus).HasColumnName("bonus").HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(p => p.Deductions).HasColumnName("deductions").HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(p => p.NetSalary).HasColumnName("netsalary").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.PayPeriodStart).HasColumnName("payperiodstart").IsRequired();
        builder.Property(p => p.PayPeriodEnd).HasColumnName("payperiodend").IsRequired();
        builder.Property(p => p.PayDate).HasColumnName("paydate").IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").HasMaxLength(50).HasDefaultValue("Pending");
        builder.Property(p => p.IsActive).HasColumnName("isactive").HasDefaultValue(true);
        builder.Property(p => p.CreatedDate).HasColumnName("createddate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.ModifiedDate).HasColumnName("modifieddate");
        builder.Property(p => p.CreatedBy).HasColumnName("createdby").HasMaxLength(100);
        builder.Property(p => p.ModifiedBy).HasColumnName("modifiedby").HasMaxLength(100);

        builder.HasOne(p => p.Employee)
            .WithMany(e => e.Payrolls)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.EmployeeId);
        builder.HasIndex(p => p.PayPeriodStart);
        builder.HasIndex(p => p.PayPeriodEnd);
    }
}
