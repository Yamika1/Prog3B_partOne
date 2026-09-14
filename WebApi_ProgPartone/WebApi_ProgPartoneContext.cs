using Microsoft.EntityFrameworkCore;

public class WebApi_ProgPartoneContext(DbContextOptions<WebApi_ProgPartoneContext> options) : DbContext(options)
{
    public DbSet<WebApi_ProgPartone.Models.AddSensorPayloadDTO> AddSensorPayloadDTOs { get; set; } = default!;
    
}
