using Microsoft.EntityFrameworkCore;

namespace DSS.Tests
{
    public class DependencyInjection
    {
        public static ServiceCollection InitilizeServices()
        {
            var services = new ServiceCollection();
            var options = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase("DSS.db").Options;
            services.AddScoped(_ => new ApplicationContext(options));
            return services;
        }
    }
}
