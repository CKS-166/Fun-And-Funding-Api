using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.SoftDeleteService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Fun_Funding.Infrastructure.Database
{
    public class MyDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public MyDbContext()
        {
            
        }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Configure Identity to use Guid for keys
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-generate GUID
            });

            // Configure the relationship between Order and User explicitly
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion if needed

            // Configure the relationship between PackageBacker and User explicitly
            modelBuilder.Entity<PackageBacker>()
                .HasOne(pb => pb.User)
                .WithMany(u => u.PackageUsers)
                .HasForeignKey(pb => pb.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion if needed

            // Add other model configurations below if needed
        }



        public DbSet<Project> Project { get; set; }
        public DbSet<Package> Package { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<PackageBacker> PackageBacker { get; set; }
        public DbSet<SystemWallet> SystemWallet { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<WithdrawRequest> WithdrawRequest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString())
                    .AddInterceptors(new SoftDeleteInterceptor());
            }
        }

        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            return configuration.GetConnectionString("DefaultConnection");
        }
    }
}
