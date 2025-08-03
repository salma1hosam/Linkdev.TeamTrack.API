using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Linkdev.TeamTrack.Infrastructure.Data
{
    public class DataSeeding(TeamTrackDbContext _dbContext, RoleManager<IdentityRole> _roleManager) : IDataSeeding
    {
        public async Task RoleSeedingAsync()
        {
            try
            {
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                    await _dbContext.Database.MigrateAsync();

                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Project Manager"));
                    await _roleManager.CreateAsync(new IdentityRole("Team Member"));
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //To Do
            }
        }
    }
}
