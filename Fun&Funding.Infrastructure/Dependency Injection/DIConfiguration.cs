﻿using Fun_Funding.Application;
using Fun_Funding.Application.IRepository;

using Fun_Funding.Application.IService;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.ITokenService;
using Fun_Funding.Application.Service;

using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Database;
using Fun_Funding.Infrastructure.Mapper;
using Fun_Funding.Infrastructure.Repository;
using Fun_Funding.Infrastructure.SoftDeleteService;
using Fun_Funding.Infrastructure.StorageService;
using Fun_Funding.Infrastructure.TokenGeneratorService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fun_Funding.Infrastructure.Dependency_Injection
{
    public static class DIConfiguration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            //DBContext
            service.AddDbContext<MyDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(DIConfiguration).Assembly.FullName))
            .AddInterceptors(new SoftDeleteInterceptor()), ServiceLifetime.Scoped);

            //Identity
            service.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<MyDbContext>()
                .AddDefaultTokenProviders();


            //Authentication
            service.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(configuration["Jwt:key"]!))
                };
            });



            //BaseRepository          
            service.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            // Register the UnitOfWork
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IAzureService, AzureService>();
            service.AddAutoMapper(typeof(MapperConfig).Assembly);
            #region Repositories
            service.AddScoped<IBankAccountRepository, BankAccountRepository>();
            service.AddScoped<ICategoryRepository, CategoryRepository>();
            service.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            service.AddScoped<IOrderRepository, OrderRepository>();
            service.AddScoped<IPackageBackerRepository, PackageBackerRepository>();
            service.AddScoped<IPackageRepository, PackageRepository>();
            service.AddScoped<IProjectRepository, ProjectRepository>();
            service.AddScoped<IRewardItemRepository, RewardItemRepository>();
            service.AddScoped<ISourceFileRepository, SourceFileRepository>();
            service.AddScoped<ISystemWalletRepository, SystemWalletRepository>();
            service.AddScoped<ITransactionRepository, TransactionRepository>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IWalletRepository, WalletRepository>();
            service.AddScoped<IWithdrawRequestRepository, WithdrawRequestRepository>();
            #endregion
            #region Sevices
            service.AddScoped<IAuthenticationService, AuthenticationService>();
            service.AddScoped<ITokenGenerator, TokenGenerator>();
            service.AddScoped<IFundingProjectService, FundingProjectManagementService>();
            service.AddScoped<ICategoryService, CategoryService>();
            #endregion
            return service;

        }
    }
}
