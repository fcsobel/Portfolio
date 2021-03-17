using System;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context.Mapping;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context
{
    public class PortfolioContext : DbContext
    {
        public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockDividend> StockDividend { get; set; }
        public DbSet<StockPrice> StockPrice { get; set; }
        public DbSet<LoginProfile> LoginProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserAccess> UserAccess { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OnModelCreating
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new AccountMap());
            modelBuilder.ApplyConfiguration(new InvestmentMap());
            modelBuilder.ApplyConfiguration(new StockMap());
            modelBuilder.ApplyConfiguration(new StockPriceMap());
            modelBuilder.ApplyConfiguration(new StockDividendMap());
            modelBuilder.ApplyConfiguration(new LoginProfileConfig());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserAccessMap());
        }
    }
}