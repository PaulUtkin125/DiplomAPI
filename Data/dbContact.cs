using Diplom_Utkin.Model.DataBase;
using Finansu.Model;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Data
{
    internal class dbContact : DbContext
    {
        public DbSet<DvizhenieSredstv> dvizhenieSredstvs { get; set; }
        public DbSet<InvestTools> InvestTools { get; set; }
        public DbSet<Brokers> Brokers { get; set; }
        public DbSet<InvestToolHistory> InvestToolHistory { get; set; }
        public DbSet<Urisidikciiy> Urisidikciiy { get; set; }
        public DbSet<TypeOfUser> typeOfUser { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<TypeOfRequest> TypeRequest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FinanceManagerdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
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
