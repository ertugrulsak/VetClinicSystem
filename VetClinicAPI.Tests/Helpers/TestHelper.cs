using Microsoft.EntityFrameworkCore;
using VetClinicAPI.Data;

namespace VetClinicAPI.Tests.Helpers
{
    public static class TestHelper
    {
        public static AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}