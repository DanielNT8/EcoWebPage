using EcoBO.DTO.PayOS;
using EcoBO.Models;
using EcoRepository.Interfaces;
using EcoRepository.Repositories;
using EcoService;
using EcoService.Interfaces;
using EcoService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace EcoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "EcoAPI", Version = "v1" });

                // 🔐 Thêm JWT Bearer Authorization vào Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token vào dạng: Bearer {token}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });




            builder.Services.AddAuthorization();

            // DBContext
            builder.Services.AddDbContext<EcoDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Service DI
            builder.Services.AddScoped<IPayOSService, PayOSService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IContactService, ContactService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            // Repository DI
            builder.Services.AddScoped<IWebLogRepository, WebLogRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<TransactionHistoryRepository>();


            // 🔒 CORS CONFIGURATION
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowTrustedOrigins", policy =>
                {
                    policy.WithOrigins(
                            "https://eco.info.vn"      // domain chính thức
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });



            // cấu hình settings
            builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOS"));



            var app = builder.Build();

            // Listen on Render's provided port
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Urls.Add($"http://*:{port}");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowTrustedOrigins");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapGet("/", () => "✅ EcoWebPage API is running successfully!");

            app.Run();
        }
    }
}
