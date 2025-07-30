using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Infrastructure.Data;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Linkdev.TeamTrack.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //DbContext Registeration
            builder.Services.AddDbContext<TeamTrackDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //Identity Services Registeration
            builder.Services.AddIdentity<TeamTrackUser , IdentityRole>()
                            .AddEntityFrameworkStores<TeamTrackDbContext>();

            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            #endregion

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var objFromDataSeeding = scope.ServiceProvider.GetService<IDataSeeding>();
            await objFromDataSeeding.RoleSeedingAsync();

            #region Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
