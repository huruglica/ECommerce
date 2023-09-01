
using AutoMapper;
using BankAccountService.Validator;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserAndBankAccountServices.Data;
using UserAndBankAccountServices.Helpers;
using UserAndBankAccountServices.Model.Dtos;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Repository;
using UserAndBankAccountServices.Repository.IRepository;
using UserAndBankAccountServices.Services.IServices;
using UserAndBankAccountServices.Validator;

namespace UserAndBankAccountServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var mapperConfiguration = new MapperConfiguration(
                mc => mc.AddProfile(new Helpers.AutoMapper()));

            IMapper mapper = mapperConfiguration.CreateMapper();

            builder.Services.AddSingleton(mapper);

            builder.Services.AddDbContext<ECommerceDbContex>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DeafultConnection")));

            builder.Services.AddTransient<IUserService, Service.UserService>();
            builder.Services.AddTransient<IBankAccountService, Service.BankAccountService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();

            builder.Services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
            builder.Services.AddScoped<IValidator<BankAccountDto>, BankAccountDtoValidator>();

            builder.Services.AddGrpc();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "dev-nq3upfdndrxpn4bz.us.auth0.com",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3RiMMI3eusj2CJu15cJQIpXP8YallpXQQj8ad_13GiLu4uS7sUxL3Wezw6HpzfLL"))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce", Version = "v1" });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization",
                    In = ParameterLocation.Header
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            var app = builder.Build();

            app.MapGrpcService<Service.UserService>();
            app.MapGrpcService<Service.BankAccountService>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DisplayRequestDuration();
                    c.DefaultModelExpandDepth(0);

                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce");
                    c.OAuthClientId("pwhlJHybcPXXkCWZ2MGLXlDFTXD9oTK4");
                    c.OAuthClientSecret("3RiMMI3eusj2CJu15cJQIpXP8YallpXQQj8ad_13GiLu4uS7sUxL3Wezw6HpzfLL");
                    c.OAuthAppName("ECommerce");
                    //c.OAuthUsePkce();
                    //c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}