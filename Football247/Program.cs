using Football247.Authorization;
using Football247.Data;
using Football247.IdentityExtensions;
using Football247.Mappings;
using Football247.Middleware;
using Football247.Models.Entities;
using Football247.Repositories;
using Football247.Repositories.IRepository;
using Football247.Services;
using Football247.Services.Caching;
using Football247.Services.IService;
using Football247.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Add services to the container.

builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); 
    logging.AddDebug();   
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Để sử dụng IHttpContextAccessor trong các lớp khác như repository, service, ...
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IRealtimeService, RealtimeService>();


// Cấu hình JWT Bearer Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Football247 API", Version = "v1" });

    // Khai báo cơ chế bảo mật
    // Thêm nút "Authorize" ở góc trên bên phải của giao diện Swagger UI
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    // Bảo Swagger gắn JWT Bearer Authentication vào các request
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, 
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Bearer",
                Name = JwtBearerDefaults.AuthenticationScheme, 
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<Football247DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Football247ConnectionString")));

// Cấu hình Redis Cache
builder.Services.AddStackExchangeRedisCache(option => 
{
    option.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    option.InstanceName = "Football247_";
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>() 
    .AddEntityFrameworkStores<Football247DbContext>()
    .AddDefaultTokenProviders()
    // cấu hình quy tắc cho email
    .AddUserValidator<AllowedDomainUserValidator<ApplicationUser>>();


// Cấu hình Quy tắc cho Mật khẩu
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// * ĐĂNG KÝ Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        }
    )
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    }
);

// =============================================================
// 🔒 1. CẤU HÌNH CORS (BẢO MẬT CAO)
// =============================================================
var myAllowSpecificOrigins = "AllowTluHubOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                    // ✅ Chỉ cho phép Domain thật của Frontend
                                    "https://tlu-hub-develop.vercel.app", 
                                    
                                    // ✅ Cho phép Localhost để bạn test dưới máy (nếu cần)
                                    // Nếu không thích bạn có thể xóa dòng localhost này đi
                                    "http://localhost:3000",
                                    "http://localhost:5173"
                                )
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials(); // Cho phép gửi Cookie/Auth nếu sau này cần
                      });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes
        .Where(c => c.Name.EndsWith("Service"))
        .Where(c => c.Name != nameof(RedisCacheService))
    )
    .AsMatchingInterface()
    .WithScopedLifetime()
);

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMemoryCache();

builder.Services.AddAuthorization(options =>
{
    // Lấy tất cả permission đã định nghĩa
    var permissions = Permissions.GetAllPermissions();

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.RequirePermission(permission)); 
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrations();

    await app.SeedRolesAndPermissions();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// Phục vụ file tĩnh (ví dụ: hình ảnh)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});

// Bật cơ chế định tuyến (Routing)
app.UseRouting();

// =============================================================
// 🔒 2. KÍCH HOẠT POLICY VỪA TẠO
// =============================================================
// Quan trọng: Phải đặt UseCors TRƯỚC UseAuthorization
app.UseCors(myAllowSpecificOrigins);

// Xác thực người dùng
app.UseAuthentication();

// Quyền hạn của người dùng
app.UseAuthorization();

// Map các controller endpoints
app.MapControllers();

// Map các SignalR hub endpoints
app.MapHub<Football247Hub>("/football247hub");

app.Run();
