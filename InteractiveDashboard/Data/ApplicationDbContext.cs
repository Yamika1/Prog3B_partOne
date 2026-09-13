using InteractiveDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace InteractiveDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SensorPayload> SensorPayloads { get; set; }
        public DbSet<SensorPayloadFile> SensorPayloadFiles { get; set; }
    }
}