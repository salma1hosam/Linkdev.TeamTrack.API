using Linkdev.TeamTrack.API.Middlewares;
using Linkdev.TeamTrack.Application.MappingProfiles;
using Linkdev.TeamTrack.Application.Services;
using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Infrastructure.Data;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Linkdev.TeamTrack.Infrastructure.UOW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Linkdev.TeamTrack.Infrastructure.EmailService;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Linkdev.TeamTrack.Infrastructure.ElasticSearch;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

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
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Description = "Enter 'Bearer' Followed By Space And Your Token"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()  //Scope
					}
                });
            });

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
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.Configure<SmtpConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITaskService, TaskService>();

            //Elastic Search Registeration
            var elasticSearchConfig = builder.Configuration.GetSection("ElasticSetting").Get<ElasticSearchConfiguration>()
                ?? throw new Exception("Elasticsearch configuration is missing or invalid.");
            var connectionString = new ElasticsearchClientSettings(new Uri(elasticSearchConfig.Url))
                .Authentication(new BasicAuthentication(elasticSearchConfig.Username, elasticSearchConfig.Password))
                .DefaultIndex(elasticSearchConfig.DefaultIndex);
            builder.Services.AddSingleton(new ElasticsearchClient(connectionString));

            builder.Services.AddScoped<IProjectElasticService, ProjectElasticService>();
            #endregion

            var app = builder.Build();

            #region Data Seeding
            using var scope = app.Services.CreateScope();
            var objFromDataSeeding = scope.ServiceProvider.GetService<IDataSeeding>();
            await objFromDataSeeding.RoleSeedingAsync();
            #endregion

            #region Configure the HTTP request pipeline.

            app.UseMiddleware<UnifiedResponseMiddleware>();

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.ConfigObject = new ConfigObject()
                {
                    DisplayRequestDuration = true
                };

                options.DocExpansion(DocExpansion.None);

                //The UI Of Authorization
                options.EnablePersistAuthorization();
            });
            //}

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
