using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Linkdev.TeamTrack.Infrastructure.Data.Contexts
{
    public class TeamTrackDbContext(DbContextOptions<TeamTrackDbContext> dbContextOptions) : IdentityDbContext<TeamTrackUser>(dbContextOptions)
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TeamTrackUser>().Property(U => U.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.ApplyConfiguration(new ProjectConfigurations());
            builder.ApplyConfiguration(new TaskConfigurations());
        }
    }
}
