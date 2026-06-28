using Football247.Application.Common.Data;
using Football247.Application.Service.FootballBackground;
using Football247.Application.Service.PaymentService;
using Football247.Application.Service.UserService;
using Football247.Application.SignalR;
using Football247.Authorization;
using Football247.Domain.ValueSettings;
using Football247.IdentityExtensions;
using Football247.Infrastructure;
using Football247.Infrastructure.Services.UserService;
using Football247.Middleware;
using Football247.Models.Entities;
using Football247.Repositories;
using Football247.Repositories.IRepository;
using Football247.Services.Caching;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayOS;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSignalR();
// ✅ ĐỔI THÀNH ĐOẠN MỚI NÀY:
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Bật lỗi chi tiết để dễ debug
})
.AddJsonProtocol(options =>
{
    // 🌟 DÒNG QUAN TRỌNG NHẤT: Bỏ qua lỗi vòng lặp tuần hoàn dữ liệu
    options.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

    // Đảm bảo JSON trả về viết thường chữ cái đầu (camelCase) đúng chuẩn Frontend cần
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.Configure<AppSetting>(builder.Configuration);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSetting>>().Value);
var appSetting = builder.Configuration.Get<AppSetting>() ?? new AppSetting();

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

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
var payOsClientId = builder.Configuration["PayOS:ClientId"];
var payOsApiKey = builder.Configuration["PayOS:ApiKey"];
var payOsChecksumKey = builder.Configuration["PayOS:ChecksumKey"];

// 2. Đăng ký PayOSClient vào hệ thống (BẮT BUỘC PHẢI CÓ)
if (!string.IsNullOrEmpty(payOsClientId) && !string.IsNullOrEmpty(payOsApiKey) && !string.IsNullOrEmpty(payOsChecksumKey))
{
    builder.Services.AddSingleton(new PayOSClient(payOsClientId, payOsApiKey, payOsChecksumKey));
}
else
{
    // Cảnh báo nếu bạn quên cấu hình trong appsettings
    Console.WriteLine("⚠️ THIẾU CẤU HÌNH PAYOS TRONG APPSETTINGS.JSON!");
}

// 3. Đăng ký PaymentService của bạn
builder.Services.AddScoped<IPaymentService, PaymentService>();
//builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddHttpClient<FootballDataClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["FootballData:BaseUrl"]
        ?? "https://api.football-data.org/v4/");

    client.DefaultRequestHeaders.Add(
        "X-Auth-Token",
        builder.Configuration["FootballData:ApiToken"]);

    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<FootballSyncJob>();
builder.Services.AddHostedService<FootballDataBackgroundService>();


// Thêm dòng này để BackgroundService crash không làm tắt app
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddDbContext<Football247DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Football247ConnectionString")));

// Cấu hình Redis Cache
builder.Services.AddStackExchangeRedisCache(option => 
{
    option.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    option.InstanceName = "Football247_";
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>() 
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

    // Cho phép khoảng trắng và các ký tự tiếng Việt có dấu
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđĐ";
    
    // Đảm bảo mỗi Email chỉ đăng ký được 1 tài khoản
    options.User.RequireUniqueEmail = true;
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
            options.SaveToken = true;
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

            options.Events = new JwtBearerEvents
            {
                //OnMessageReceived = context =>
                //{
                //    // Đọc access_token từ query string
                //    var accessToken = context.Request.Query["access_token"];

                //    // Nếu request đến một Hub SignalR (đường dẫn bắt đầu bằng /hubs)
                //    var path = context.HttpContext.Request.Path;
                //    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs") || path.StartsWithSegments("/football247hub")))
                //    {
                //        context.Token = accessToken;
                //    }
                //    return Task.CompletedTask;
                //}

                // 🌟 THÊM ĐOẠN NÀY ĐỂ BẮT LOG LỖI AUTH 🌟
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"\n❌ [SIGNALR AUTH ERROR]: {context.Exception.Message}\n");
                    return Task.CompletedTask;
                },

                OnMessageReceived = context =>
                {
                    // 1. Đọc access_token từ query string
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path.Value?.ToLower() ?? string.Empty;

                    // 2. Kiểm tra an toàn: Nếu URL có chứa chữ "hub" thì gán Token ngay
                    if (!string.IsNullOrEmpty(accessToken) && path.Contains("hub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
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
                                    // ✅ Cho phép Localhost để bạn test dưới máy (nếu cần)
                                    // Nếu không thích bạn có thể xóa dòng localhost này đi
                                    "http://localhost:3000",
                                    "http://localhost:5173",
                                    "http://127.0.0.1:5500",
                                    "http://localhost:5500",
                                    "https://localhost:7087",
                                    "https://football247-fe.vercel.app"
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

builder.Services.AddSingleton<FootballDataCache>();

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

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
app.UseCors(myAllowSpecificOrigins);

// Xác thực người dùng
app.UseAuthentication();

// Quyền hạn của người dùng
app.UseAuthorization();

// Map các controller endpoints
app.MapControllers();

// Map các SignalR hub endpoints
app.MapHub<Football247Hub>("/football247hub");
app.MapHub<ArticleCommentHub>("/hubs/article-comment");  // realtime comment theo từng bài viết
app.MapHub<NotificationHub>("/hubs/notification");        // notification cá nhân theo userId
app.MapHub<FootballHub>("/hubs/football");                // realtime football data theo competition
app.MapHub<FootballHub>("/hubs/match-detail");

app.Run();
