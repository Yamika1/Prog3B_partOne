using Microsoft.EntityFrameworkCore;

public class InteractiveDashboardContext(DbContextOptions<InteractiveDashboardContext> options) : DbContext(options)
{
    public DbSet<InteractiveDashboard.Models.SensorPayload> SensorPayload { get; set; } = default!;
}
