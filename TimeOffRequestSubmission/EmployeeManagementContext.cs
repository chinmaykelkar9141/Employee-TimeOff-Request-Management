using System;
using Microsoft.EntityFrameworkCore;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Enums;
using Employee = TimeOffRequestSubmission.DataModels.Employee;
using Role = TimeOffRequestSubmission.DataModels.Role;

#nullable disable

namespace TimeOffRequestSubmission
{
    public class EmployeeManagementContext : DbContext
    {
        public EmployeeManagementContext()
        {
        }

        public EmployeeManagementContext(DbContextOptions<EmployeeManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        
        public virtual DbSet<TimeoffRequest> TimeoffRequests { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.State).HasMaxLength(100);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Zip).HasMaxLength(100);
                
                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Managers)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("FK_EmployeeIdManagerId");

                entity.HasOne(d => d.Roles)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RolesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmoployeeRoles");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
            }); 
            
            modelBuilder.Entity<TimeoffRequest>(entity =>
            {
                entity.ToTable("TimeoffRequest");

                entity.Property(e => e.ApprovalStatus)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("approvalStatus")
                    .HasConversion(x => x.ToString(),
                        v => (EApprovalStatus) Enum.Parse(typeof(EApprovalStatus), v));

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("reason");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("startDate");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.TimeOffRequests)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_timeoffrequest_employee");
            });
        }
    }
}
