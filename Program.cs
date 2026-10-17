using System;
using System.Threading.RateLimiting;
using apuracao.johnmarlon.com.br.Data;
using apuracao.johnmarlon.com.br.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Repositórios e Serviços
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<ApuracaoRepository>();
builder.Services.AddSingleton<OnlinePresenceService>();
builder.Services.AddScoped<TseService>();

// Rate Limiting Nativo (.NET 8)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"sucesso\":false,\"mensagem\":\"Limite de requisições excedido. Aguarde alguns segundos antes de tentar novamente.\",\"codigo\":429}",
            cancellationToken);
    };

    // Limitador Global por IP (Sliding Window: 120 reqs/min para permitir polling legítimo de presença e tela)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        string? ip = null;
        if (httpContext.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
        {
            ip = cfIp.ToString();
        }
        else if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var fwd))
        {
            ip = fwd.ToString().Split(',')[0].Trim();
        }
        else
        {
            ip = httpContext.Connection.RemoteIpAddress?.ToString();
        }

        ip = string.IsNullOrWhiteSpace(ip) ? "anonimo" : ip;

        return RateLimitPartition.GetSlidingWindowLimiter(ip, _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 120,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6,
            QueueLimit = 0
        });
    });

    // Limitador Estrito para Endpoints de API Críticos
    options.AddPolicy("ApiLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonimo";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 25,
            Window = TimeSpan.FromSeconds(10),
            QueueLimit = 0
        });
    });
});

var app = builder.Build();

// Inicializar tabelas no MSSQL (se disponível)
using (var scope = app.Services.CreateScope())
{
    var dbInit = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    _ = dbInit.InitializeAsync();
}

// Cabeçalhos de Segurança HTTP e Ocultação de Fingerprinting
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Remove("Server");
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
