using Linkdev.TeamTrack.Application.MappingProfiles;
using Linkdev.TeamTrack.Application.Services;
using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Infrastructure.Data;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Linkdev.TeamTrack.Infrastructure.UOW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            builder.Services.AddIdentity<TeamTrackUser, IdentityRole>()
                            .AddEntityFrameworkStores<TeamTrackDbContext>();

            //JWT Services Registeration
            builder.Services.AddAuthentication(configureOption =>
            {
                configureOption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  //Bearer
                configureOption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audienece"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                };
            });

            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            builder.Services.AddScoped<IUserService , UserService>();
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddScoped<IProjectService , ProjectService>();
            #endregion

            var app = builder.Build();

            #region Data Seeding
            using var scope = app.Services.CreateScope();
            var objFromDataSeeding = scope.ServiceProvider.GetService<IDataSeeding>();
            await objFromDataSeeding.RoleSeedingAsync();
            #endregion

            #region Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
