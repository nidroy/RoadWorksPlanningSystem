using Microsoft.EntityFrameworkCore;

namespace DSS.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Road> Roads { get; set; }
        public DbSet<Estimate> Estimates { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<(string, double)> roadsData = new()
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

            int estimateCount = 0;

            for (int i = 0; i < 20; i++)
            {
                (string roadNumber, double roadPriority) = roadsData[i];
                Road road = CreateRoadModel(i + 1, roadNumber, roadPriority, modelBuilder);
                estimateCount = CreateEstimateModels(estimateCount, road, modelBuilder);
            }
        }

        private Road CreateRoadModel(int id, string number, double priority, ModelBuilder modelBuilder)
        {
            Road road = new()
            {
                Id = id,
                Number = number,
                Priority = Math.Round(priority, 2),
                LinkToPassport = ""
            };

            modelBuilder.Entity<Road>().HasData(road);
            return road;
        }

        private int CreateEstimateModels(int estimateCount, Road road, ModelBuilder modelBuilder)
        {
            for (int i = 1; i <= 20; i++)
            {
                Estimate estimate = new()
                {
                    Id = i + estimateCount,
                    Name = $"Смета {i + estimateCount}",
                    LevelOfWorks = Math.Round(i * 0.2, 2),
                    Cost = Math.Round(i * 0.2 * 100000, 2),
                    Link = "",
                    RoadId = road.Id,
                };

                modelBuilder.Entity<Estimate>().HasData(estimate);
            }

            return estimateCount + 20;
        }
    }
}
