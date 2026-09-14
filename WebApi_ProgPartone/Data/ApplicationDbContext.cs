using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using WebApi_ProgPartone.Models.Entities;

namespace WebApi_ProgPartone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<SensorPayload> SensorPayloads { get; set; }
        public DbSet<SensorPayloadFile> SensorPayloadFile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorPayload>().ToTable("SensorPayload");
        }
    }
}
