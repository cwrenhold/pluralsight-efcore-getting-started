using Microsoft.EntityFrameworkCore;

namespace SamuraiApp.Data;

public class SamuraiContextNoTracking : SamuraiContext
{
    public SamuraiContextNoTracking(DbContextOptions<SamuraiContext> options)
        : base(options)
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }   
}