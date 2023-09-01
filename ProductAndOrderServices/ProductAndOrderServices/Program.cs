using AutoMapper;
using FluentValidation;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductAndOrderServices.Data;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository;
using ProductAndOrderServices.Repository.IRepository;
using ProductAndOrderServices.Services;
using ProductAndOrderServices.Services.IServices;
using ProductAndOrderServices.Validator;
using System.Text;
using MongoDB.Driver;
using static BankAccountService.BankAccountService;
using static UserService.UserService;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using static EmailService.EmailService;
using MassTransit;
using Elasticsearch.Net;
using Nest;
using ProductAndOrderServices.ElasasticSearch;
using Microsoft.Extensions.Options;

namespace ProductAndOrderServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHangfire(config =>
            {
                var mongoOptions = new MongoStorageOptions();
                mongoOptions.MigrationOptions.MigrationStrategy = new MigrateMongoMigrationStrategy();
                mongoOptions.MigrationOptions.BackupStrategy = new CollectionMongoBackupStrategy();

                config.UseMongoStorage("mongodb://localhost:27017/ECommerceHangfire", mongoOptions);
            });
            builder.Services.AddHangfireServer();

            builder.Services.AddRabbitMQMassTransit();

            var mapperConfiguration = new MapperConfiguration(
                mc => mc.AddProfile(new Helpers.AutoMapper()));

            IMapper mapper = mapperConfiguration.CreateMapper();

            builder.Services.AddSingleton(mapper);

            builder.Services.AddSingleton(sp =>
            {
                return new MongoClient("mongodb://localhost:27017");
            });
            builder.Services.AddScoped<ECommerceDbContext>();

            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<IValidator<ProductCreateDto>, ProductCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<ProductUpdateDto>, ProductUpdateDtoValidator>();
            
            builder.Services.AddScoped<IValidator<OrderCreateDto>, OrderCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<OrderUpdateDto>, OrderUpdateDtoValidator>();

            builder.Services.AddTransient<ElasticSearch>();

            builder.Services.AddGrpc();
            builder.Services.AddGrpcClient<UserServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7247");
            });
            builder.Services.AddGrpcClient<BankAccountServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7247");
            });
            builder.Services.AddGrpcClient<EmailServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7018");
            });

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

            var node = new SingleNodeConnectionPool(new Uri("https://localhost:9200"));

            var connectionSettings = new ConnectionSettings(node)
                .BasicAuthentication("elastic", "re49_mU-iSSbzae3Ritn")
                .CertificateFingerprint("c11c134af2fb11a1702db07c75cccf26d0467a4a06ca5ea492f9685cb6f1c5a4");

            var client = new ElasticClient(connectionSettings);

            builder.Services.AddSingleton(client);

            var app = builder.Build();

            app.UseHangfireDashboard();

            var scope = app.Services.CreateScope();

            var orderService = scope.ServiceProvider.GetService<IOrderService>()
                             ?? throw new Exception("OrderService is not loaded");

            var bus = scope.ServiceProvider.GetService<IBus>()
                    ?? throw new Exception("Bus is not loaded");

            HangfireService hangfireService = new(orderService, bus);

            RecurringJob.AddOrUpdate("GetUserSpendMost", () => hangfireService.GetUserSpendMost(), Cron.Daily);

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