using Diplom_Utkin.Model.DataBase;
using DiplomAPI.Models.db;
using Finansu.Model;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Data
{
    internal class dbContact : DbContext
    {
        private IConfiguration _configuration;
        
        public dbContact(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<DvizhenieSredstv> dvizhenieSredstvs { get; set; }
        public DbSet<InvestTools> InvestTools { get; set; }
        public DbSet<Brokers> Brokers { get; set; }
        public DbSet<InvestToolHistory> InvestToolHistory { get; set; }
        public DbSet<Urisidikciiy> Urisidikciiy { get; set; }
        public DbSet<TypeOfUser> typeOfUser { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<TypeOfRequest> TypeRequest { get; set; }
        public DbSet<BalanceHistory> BalanceHistory { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration["Database:defaultPath"]);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<InvestToolHistory>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Portfolio>()
                .HasKey(x => new { x.UserId, x.InvestToolId });
        }
    }
}
