using Backend.DataObjects;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection(nameof(JwtConfig))["SigningToken"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection(nameof(JwtConfig))["Issuer"],
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection(nameof(JwtConfig))["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

            services.Configure<StorageAccountConfig>(Configuration.GetSection(nameof(StorageAccountConfig)))
                    .Configure<DBConfig>(Configuration.GetSection(nameof(DBConfig)))
                    .Configure<CardConfig>(Configuration.GetSection(nameof(CardConfig)))
                    .Configure<JwtConfig>(Configuration.GetSection(nameof(JwtConfig)));

            services.AddSingleton<UploadServices>()
                    .AddSingleton<IdentityDocumentDBRepository<CredentialModel>>()
                    .AddSingleton<CardDocumentDBRepository<CardModel>>()
                    .AddSingleton<AvatarDocumentDBRepository<AvatarModel>>();

            services.AddCors(options =>
            {
                options.AddPolicy("ApiAllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("ApiAllowAll"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Stream}/{action=Index}/{id?}");
            });
        }
    }
}
