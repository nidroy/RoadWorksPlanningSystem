using DSS.Handlers;
using DSS.Parsers;
using Microsoft.EntityFrameworkCore;

namespace DSS.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Road> Roads { get; set; }
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<TechnicalConditionOfRoad> TechnicalConditionsOfRoads { get; set; }
        public DbSet<RoadWorksProgram> RoadWorksPrograms { get; set; }
        public DbSet<RoadWorksProgramToEstimate> RoadWorksProgramsToEstimates { get; set; }

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
                "разработку и реализацию мероприятий по снижению загрязнения окружающей среды от дорожного движения",
                "регулировку и обновление сигнальных светофоров и световых указателей",
                "планирование и проведение периодической проверки состояния мостовых сооружений",
                "очистку и обслуживание дорожных кюветов и систем дренажа",
                "поддержание и обновление средств организации дорожного движения, таких как конусы и барьеры",
                "укладку и ремонт дорожного бетона и брусчатки",
                "организацию и проведение дорожных мероприятий, направленных на улучшение безопасности дорожного движения",
                "разработку и установку дорожных знаков и информационных панелей для предупреждения о дорожных условиях",
                "укрепление и ремонт барьеров безопасности на дорогах с опасными поворотами",
                "планирование и проведение работ по укреплению краев дороги для предотвращения обвалов",
                "организацию и проведение обучающих программ для водителей о безопасном вождении и соблюдении ПДД",
                "разработку и внедрение инновационных технологий для улучшения качества и долговечности дорожного покрытия",
                "организацию и проведение мероприятий по ремонту и обновлению дорожных переходов для пешеходов",
                "планирование и проведение специализированных работ по очистке и обновлению дорожных туннелей и метрополитенов",
                "разработку и реализацию проектов по созданию экологически чистых зон вдоль автомобильных дорог",
                "поддержание и обновление системы дорожных знаков и информационных табличек для навигации и ориентации водителей",
                "организацию и проведение регулярных инспекций и технического обслуживания автомобильных доро",
                "разработку и внедрение программ по мониторингу и контролю за состоянием дорожной инфраструктуры",
                "проведение работ по обновлению и расширению дорожной сети в соответствии с городскими планами развития",
                "организацию и проведение мероприятий по охране и защите природных ресурсов в зоне автомобильных дорог",
                "разработку и реализацию программ по снижению уровня шума и вибрации от автотранспорта в городских и пригородных районах"
            };

            int estimateCount = 0;

            for (int i = 0; i < 20; i++)
            {
                (string roadNumber, double roadPriority) = roadsData[i];
                Road road = CreateRoadModel(i + 1, roadNumber, roadPriority, modelBuilder);
                estimateCount = CreateEstimateModels(estimateCount, estimatesData, road, modelBuilder);
            }

            CreateTechnicalConditionOfRoadModels(modelBuilder);

            CreateRoadWorksProgramModels(modelBuilder);
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
            for (int i = 1; i <= 40; i++)
            {
                Estimate estimate = new()
                {
                    Id = i + estimateCount,
                    Name = $"Смета на {estimatesData[i - 1]}.",
                    LevelOfWorks = Math.Round(i * 0.1, 1),
                    Cost = Math.Round(i * 0.2 * 100000, 2),
                    Link = "",
                    RoadId = road.Id
                };

                modelBuilder.Entity<Estimate>().HasData(estimate);
            }

            return estimateCount + 40;
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

        private void CreateRoadWorksProgramModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoadWorksProgramToEstimate>()
                .HasOne(pe => pe.RoadWorksProgram)
                .WithMany()
                .HasForeignKey(pe => pe.RoadWorksProgramId)
                .OnDelete(DeleteBehavior.NoAction);

            //string folderPath = @"Data\RoadWorksPrograms";
            //var years = FolderHandler.GetSubfolderNames(folderPath);

            //int roadWorksProgramId = 1;
            //int roadWorksProgramToEstimateId = 1;

            //foreach (var year in years)
            //{
            //    var roadWorksPrograms = ExcelParser.ParseRoadWorksPrograms(Path.Combine(folderPath, year));

            //    if (roadWorksPrograms == null)
            //    {
            //        return;
            //    }

            //    for (int i = 0; i < roadWorksPrograms.Count; i++)
            //    {
            //        RoadWorksProgram roadWorksProgram = new()
            //        {
            //            Id = roadWorksProgramId,
            //            Year = roadWorksPrograms[i].Year,
            //            Month = roadWorksPrograms[i].Month,
            //            Cost = roadWorksPrograms[i].Cost,
            //            RoadId = roadWorksPrograms[i].RoadId
            //        };

            //        modelBuilder.Entity<RoadWorksProgram>().HasData(roadWorksProgram);

            //        foreach (var estimateId in roadWorksPrograms[i].EstimatesId)
            //        {
            //            RoadWorksProgramToEstimate roadWorksProgramToEstimate = new()
            //            {
            //                Id = roadWorksProgramToEstimateId,
            //                RoadWorksProgramId = roadWorksProgramId,
            //                EstimateId = estimateId
            //            };

            //            modelBuilder.Entity<RoadWorksProgramToEstimate>().HasData(roadWorksProgramToEstimate);
            //            roadWorksProgramToEstimateId++;
            //        }

            //        roadWorksProgramId++;
            //    }
            //}
        }
    }
}
