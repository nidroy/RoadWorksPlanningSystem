using DSS.Parsers;
using Microsoft.EntityFrameworkCore;

namespace DSS.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Road> Roads { get; set; }
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<TechnicalConditionOfRoad> TechnicalConditionsOfRoads { get; set; }

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

            List<string> estimatesData = new()
            {
                "планирование и проведение регулярной диагностики состояния дорожного покрытия",
                "поддержание и ремонт асфальтового покрытия",
                "ремонт трещин и ям на дорожном покрытии",
                "укладку нового асфальтового покрытия",
                "очистку и обновление дорожных обочин",
                "маркировку и разметку дорог для обеспечения безопасности движения",
                "установку и обновление дорожных знаков и сигнальной арматуры",
                "обслуживание и ремонт дорожных ограждений и барьеров",
                "поддержание и обновление дорожного освещения",
                "ремонт и обслуживание дренажной системы для отвода воды с дорожной поверхности",
                "очистку и обслуживание дренажных канав",
                "поддержание и ремонт мостов, путепроводов и путеподъездных путей",
                "укрепление и ремонт обочин и береговых склонов",
                "очистку и обслуживание дорожных тоннелей и подземных пешеходных переходов",
                "поддержание и ремонт дорожных сооружений, таких как шумоизоляционные экраны",
                "установку и обновление средств пожаротушения и аварийного оборудования",
                "ремонт и обслуживание дорожных септиков и систем водоотведения",
                "поддержание и ремонт дорожных канализационных систем",
                "очистку и обновление зеленых насаждений вдоль дорог",
                "разработку и реализацию мероприятий по снижению загрязнения окружающей среды от дорожного движения"
            };

            int estimateCount = 0;

            for (int i = 0; i < 20; i++)
            {
                (string roadNumber, double roadPriority) = roadsData[i];
                Road road = CreateRoadModel(i + 1, roadNumber, roadPriority, modelBuilder);
                estimateCount = CreateEstimateModels(estimateCount, estimatesData, road, modelBuilder);
            }

            CreateTechnicalConditionOfRoadModels(modelBuilder);
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

        private int CreateEstimateModels(int estimateCount, List<string> estimatesData, Road road, ModelBuilder modelBuilder)
        {
            for (int i = 1; i <= 20; i++)
            {
                Estimate estimate = new()
                {
                    Id = i + estimateCount,
                    Name = $"Смета на {estimatesData[i - 1]}",
                    LevelOfWorks = Math.Round(i * 0.2, 2),
                    Cost = Math.Round(i * 0.2 * 100000, 2),
                    Link = "",
                    RoadId = road.Id,
                };

                modelBuilder.Entity<Estimate>().HasData(estimate);
            }

            return estimateCount + 20;
        }

        private void CreateTechnicalConditionOfRoadModels(ModelBuilder modelBuilder)
        {
            string folderPath = @"Data\TechnicalConditionsOfRoads";
            var technicalConditionsOfRoads = ExcelParser.ParseTechnicalConditionsOfRoads(folderPath);

            if (technicalConditionsOfRoads == null)
            {
                return;
            }

            for (int i = 0; i < technicalConditionsOfRoads.Count; i++)
            {
                TechnicalConditionOfRoad technicalConditionOfRoad = new()
                {
                    Id = i + 1,
                    Year = technicalConditionsOfRoads[i].Year,
                    Month = technicalConditionsOfRoads[i].Month,
                    TechnicalCondition = technicalConditionsOfRoads[i].TechnicalCondition,
                    RoadId = technicalConditionsOfRoads[i].RoadId
                };

                modelBuilder.Entity<TechnicalConditionOfRoad>().HasData(technicalConditionOfRoad);
            }
        }
    }
}
