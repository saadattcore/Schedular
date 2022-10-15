using Emaratech.DatabaseLayer;

namespace Emaratech.Services.Scheduler.Entities
{
    // This providers class is needed to avoid EF package installation to DataAccess projects
    public static class SchedulerDbProviders
    {
        public static IDbContext GetDbContext()
        {
            return new SchedulerContext();
        }

        public static IUnitOfWork GetUnitOfWork()
        {
            return new UnitOfWork(GetDbContext());
        }
    }
}
