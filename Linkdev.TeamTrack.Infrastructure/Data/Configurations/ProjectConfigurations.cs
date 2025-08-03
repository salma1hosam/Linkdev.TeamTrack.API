using Linkdev.TeamTrack.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Linkdev.TeamTrack.Infrastructure.Data.Configurations
{
    public class ProjectConfigurations : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(P => P.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.Property(P => P.LastUpdatedDate).HasComputedColumnSql("GETDATE()");
        }
    }
}
