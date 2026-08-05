using System.Text;
using ExpenseTracker.Data;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Services;
using ExpenseTracker.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace ExpenseTracker;

public static partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT токен: Bearer {token}"
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>(Array.Empty<string>())
                });
        });
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped<ICategoryService, CategoryService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<IJwtService, JwtService>();
        
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>()
            .AddValidatorsFromAssemblyContaining<LoginRequestValidator>()
            .AddValidatorsFromAssemblyContaining<CreateCategoryValidator>()
            .AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>()
            .AddValidatorsFromAssemblyContaining<CreateTransactionValidator>()
            .AddValidatorsFromAssemblyContaining<UpdateTransactionValidator>();
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt токен не найден")))
                };
            });
        
        builder.Services.AddAuthorization();
        
        var app = builder.Build();

        ConfigureExceptionHandling(app);
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.Run();
    }
    
    static void ConfigureExceptionHandling(WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features
                    .Get<IExceptionHandlerFeature>();

                var ex = feature?.Error;

                if (ex is BusinessException be)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    var errors = new Dictionary<string, string[]>
                    {
                        [""] = new[] { be.Message }
                    };

                    var problem = new ValidationProblemDetails(errors)
                    {
                        Title = "Ошибка операции",
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://httpstatuses.com/400",
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = context.TraceIdentifier;

                    if (!string.IsNullOrWhiteSpace(be.Code))
                    {
                        problem.Extensions["code"] = be.Code;
                    }

                    context.Response.ContentType = "application/problem+json";

                    await context.Response.WriteAsJsonAsync(problem);
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await Results.Problem(
                    title: "Внутренняя ошибка сервера",
                    statusCode: StatusCodes.Status500InternalServerError,
                    type: "https://httpstatuses.com/500",
                    instance: context.Request.Path
                ).ExecuteAsync(context);
            });
        });
    }
}