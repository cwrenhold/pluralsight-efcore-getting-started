using Microsoft.EntityFrameworkCore;

namespace SamuraiApp.Data;

public class SamuraiContextNoTracking : SamuraiContext
{
    public SamuraiContextNoTracking()
        : base()
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }   
}