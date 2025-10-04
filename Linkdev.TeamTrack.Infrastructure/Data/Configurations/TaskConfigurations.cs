using Linkdev.TeamTrack.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Linkdev.TeamTrack.Infrastructure.Data.Configurations
{
    public class TaskConfigurations : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.Property(T => T.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.Property(T => T.LastUpdatedDate).HasComputedColumnSql("GETDATE()");

            builder.HasOne(T => T.AssignedUser)
                   .WithMany(U => U.Tasks)
                   .HasForeignKey(T => T.AssignedUserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
