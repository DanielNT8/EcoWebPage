using EcoBO.DTO.PayOS;
using EcoBO.Models;
using EcoBO.Settings;
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
            builder.Services.AddScoped<ICommunityService, CommunityService>();
            builder.Services.AddScoped<IMediaService, MediaService>();
            // Repository DI
            builder.Services.AddScoped<IWebLogRepository, WebLogRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
            builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // 🔒 CORS CONFIGURATION
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowTrustedOrigins", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // 1. Lấy cấu hình (Shared Config)
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOS"));

            // 2. Chỉ cấu hình xác thực (Verify), KHÔNG cấu hình sinh token
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Kiểm tra chữ ký: Quan trọng nhất. Nếu Key không khớp -> Token giả -> Chặn
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

                        // Kiểm tra xem Token này có phải do "EcoIdentityServer" cấp không?
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        // Kiểm tra xem Token này có phải dành cho hệ thống này không?
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        // Kiểm tra hạn sử dụng
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });


           



            var app = builder.Build();

            // Listen on Render's provided port
            //var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            //app.Urls.Add($"http://*:{port}");

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
