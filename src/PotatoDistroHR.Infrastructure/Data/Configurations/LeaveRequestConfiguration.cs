using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PotatoDistroHR.Domain.Entities;

namespace PotatoDistroHR.Infrastructure.Data.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("leaverequest");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");

        builder.Property(l => l.EmployeeId).HasColumnName("employeeid").IsRequired();
        builder.Property(l => l.StartDate).HasColumnName("startdate").IsRequired();
        builder.Property(l => l.EndDate).HasColumnName("enddate").IsRequired();
        builder.Property(l => l.Reason).HasColumnName("reason").HasMaxLength(1000);
        builder.Property(l => l.Status).HasColumnName("status").HasMaxLength(50).HasDefaultValue("Pending");
        builder.Property(l => l.ApprovedBy).HasColumnName("approvedby");
        builder.Property(l => l.ApprovedDate).HasColumnName("approveddate");
        builder.Property(l => l.RejectionReason).HasColumnName("rejectionreason").HasMaxLength(1000);
        builder.Property(l => l.IsActive).HasColumnName("isactive").HasDefaultValue(true);
        builder.Property(l => l.CreatedDate).HasColumnName("createddate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(l => l.ModifiedDate).HasColumnName("modifieddate");
        builder.Property(l => l.CreatedBy).HasColumnName("createdby").HasMaxLength(100);
        builder.Property(l => l.ModifiedBy).HasColumnName("modifiedby").HasMaxLength(100);

        builder.HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Approver)
            .WithMany()
            .HasForeignKey(l => l.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.EmployeeId);
        builder.HasIndex(l => l.Status);
    }
}
