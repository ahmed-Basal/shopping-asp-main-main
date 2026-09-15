using core.Entities;
using core.interfaces;
using core.Services;
using inftastructer.Data;
using inftastructer.Repository;
using inftastructer.Repository.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace inftastructer
{
    public static class InferastruterRegister
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            services.AddScoped(typeof(IGenricRepo<>), typeof(GenericRepositories<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenGenerate, TokenGenerate>();

         
            services.AddScoped<IAccountServic, AccountServices>();
            
            services.AddScoped<IEmailServices, EmailServices>();
                
            services.AddSingleton<IIamgeServices, Imagemangemt>();

            var wwwrootPath = Path.Combine(contentRootPath, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                var candidates = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                    Path.Combine(Directory.GetCurrentDirectory(), "api", "wwwroot"),
                    Path.Combine(AppContext.BaseDirectory, "wwwroot")
                };

                foreach (var candidate in candidates)
                {
                    if (Directory.Exists(candidate))
                    {
                        wwwrootPath = candidate;
                        break;
                    }
                }

                if (!Directory.Exists(wwwrootPath))
                {
                    var dir = AppContext.BaseDirectory;
                    while (!string.IsNullOrEmpty(dir))
                    {
                        var path1 = Path.Combine(dir, "wwwroot");
                        if (Directory.Exists(path1))
                        {
                            wwwrootPath = path1;
                            break;
                        }
                        var path2 = Path.Combine(dir, "api", "wwwroot");
                        if (Directory.Exists(path2))
                        {
                            wwwrootPath = path2;
                            break;
                        }
                        dir = Path.GetDirectoryName(dir);
                    }
                }
            }

            if (!Directory.Exists(wwwrootPath))
            {
                wwwrootPath = Path.Combine(contentRootPath, "wwwroot");
                Directory.CreateDirectory(wwwrootPath);
            }

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var options = ConfigurationOptions.Parse(
                    config.GetConnectionString("Redis"), true);

                return ConnectionMultiplexer.Connect(options);
            });

            services.AddDbContext<AppDbContext>(op =>
            {
                op.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = false;
                op.SaveToken = true;

                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Token:Secret"])),

                    ValidateIssuer = true,
                    ValidIssuer = configuration["Token:Issuer"],

                    ValidateAudience = false,
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                };


                op.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["authToken"];
                        if (!string.IsNullOrEmpty(token))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<IPaymentServices, PaymentServices>();
            services.AddScoped<ICommentService, CommentService>();

            return services;
        }
    }
}
