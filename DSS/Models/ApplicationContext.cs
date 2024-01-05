using Microsoft.EntityFrameworkCore;

namespace DSS.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Road> Roads { get; set; }
        public DbSet<Estimate> Estimates { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<(string, double)> roadData = new()
            {
                ("18 ОП РЗ 18Р-1", 1),
                ("18 ОП РЗ 18Р-1-1", 3),
                ("18 ОП РЗ 18Р-1-2", 4),
                ("18 ОП РЗ 18Р-1-3", 4),
                ("18 ОП РЗ 18Р-1-4", 1),
                ("18 ОП РЗ 18Р-1-5", 4),
                ("18 ОП РЗ 18Р-1-6", 3),
                ("18 ОП РЗ 18Р-1-7", 4),
                ("18 ОП РЗ 18Р-1-8", 4),
                ("18 ОП РЗ 18Р-2", 4),
                ("18 ОП РЗ 18Р-2-1", 3),
                ("18 ОП РЗ 18Р-2-2", 3),
                ("18 ОП РЗ 18Р-2-3", 3),
                ("18 ОП РЗ 18Р-2-4", 4),
                ("18 ОП РЗ 18Р-2-5", 4),
                ("18 ОП РЗ 18Р-2-6", 2),
                ("18 ОП РЗ 18Р-2-7", 2),
                ("18 ОП РЗ 18Р-2-8", 2),
                ("18 ОП РЗ 18Р-2-9", 2),
                ("18 ОП РЗ 18Р-2-10", 1)
            };

            int numberOfEstimates = 0;

            for (int i = 0; i < 20; i++)
            {
                (string roadNumber, double priorityOfRoad) = roadData[i];
                Road road = CreateRoadModel(i + 1, roadNumber, priorityOfRoad, modelBuilder);
                numberOfEstimates = CreateEstimateModels(numberOfEstimates, road, modelBuilder);
            }
        }

        private Road CreateRoadModel(int id, string number, double priority, ModelBuilder modelBuilder)
        {
            Road road = new()
            {
                Id = id,
                Number = number,
                Priority = priority,
                LinkToPassport = ""
            };

            modelBuilder.Entity<Road>().HasData(road);
            return road;
        }

        private int CreateEstimateModels(int numberOfEstimates, Road road, ModelBuilder modelBuilder)
        {
            for (int i = 1; i <= 20; i++)
            {
                Estimate estimate = new()
                {
                    Id = i + numberOfEstimates,
                    Name = $"Смета {i + numberOfEstimates}",
                    LevelOfWork = i * 0.2,
                    Cost = i * 0.2 * 100000,
                    Link = "",
                    RoadId = road.Id,
                };

                modelBuilder.Entity<Estimate>().HasData(estimate);
            }

            return numberOfEstimates + 20;
        }
    }
}
