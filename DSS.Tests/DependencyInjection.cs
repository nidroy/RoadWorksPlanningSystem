using DSS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DSS.Tests
{
    public class DependencyInjection
    {
        public static ServiceCollection InitilizeServices()
        {
            var services = new ServiceCollection();
            var options = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase("applicationdb").Options;
            services.AddScoped(_ => new ApplicationContext(options));
            return services;
        }
    }
}
