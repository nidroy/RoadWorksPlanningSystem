namespace DSS.Tests
{
    public class RoadWorksProgramsApiControllerTests
    {
        private readonly Mock<ILogger<ApiController>> _mock = new();
        private readonly IServiceProvider _serviceProvider;

        public RoadWorksProgramsApiControllerTests()
        {
            _serviceProvider = DependencyInjection.InitilizeServices().BuildServiceProvider();
        }

        [Fact]
        public void ReadAllRoadWorksProgramsTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadWorksProgramsApiController(context, _mock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void ReadRoadWorksProgramByIdTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadWorksProgramsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            Estimate estimate = new()
            {
                Name = "Смета 10",
                LevelOfWorks = 0.2,
                Cost = 20000,
                Link = "",
                RoadId = road.Id
            };
            context.Estimates.Add(estimate);
            context.SaveChanges();

            RoadWorksProgram roadWorksProgram = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                Cost = 25000,
                RoadId = road.Id
            };
            context.RoadWorksPrograms.Add(roadWorksProgram);
            context.SaveChanges();

            RoadWorksProgramToEstimate roadWorksProgramToEstimate = new()
            {
                RoadWorksProgramId = roadWorksProgram.Id,
                EstimateId = estimate.Id
            };
            context.RoadWorksProgramsToEstimates.Add(roadWorksProgramToEstimate);
            context.SaveChanges();

            // Act
            var result = controller.Get(roadWorksProgram.Id);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultRoadWorksProgram = JsonConvert.DeserializeObject<RoadWorksProgramEstimatesViewModel>(value.ToString());

            Assert.Equal(roadWorksProgram.Id, resultRoadWorksProgram.Id);
            Assert.Equal(roadWorksProgram.Year, resultRoadWorksProgram.Year);
            Assert.Equal(roadWorksProgram.Month, resultRoadWorksProgram.Month);
            Assert.Equal(roadWorksProgram.Cost, resultRoadWorksProgram.Cost);
            Assert.Equal(roadWorksProgram.RoadId, resultRoadWorksProgram.RoadId);

            Assert.Equal(estimate.Id, resultRoadWorksProgram.Estimates.First().Id);
            Assert.Equal(estimate.Name, resultRoadWorksProgram.Estimates.First().Name);
            Assert.Equal(estimate.LevelOfWorks, resultRoadWorksProgram.Estimates.First().LevelOfWorks);
            Assert.Equal(estimate.Cost, resultRoadWorksProgram.Estimates.First().Cost);
            Assert.Equal(estimate.Link, resultRoadWorksProgram.Estimates.First().Link);
            Assert.Equal(estimate.RoadId, resultRoadWorksProgram.Estimates.First().RoadId);

            roadWorksProgramToEstimate = context.RoadWorksProgramsToEstimates.Last();

            Assert.Equal(roadWorksProgramToEstimate.RoadWorksProgramId, resultRoadWorksProgram.Id);
            Assert.Equal(roadWorksProgramToEstimate.EstimateId, resultRoadWorksProgram.Estimates.First().Id);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void CreateRoadWorksProgramTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadWorksProgramsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            Estimate estimate = new()
            {
                Name = "Смета 10",
                LevelOfWorks = 0.2,
                Cost = 20000,
                Link = "",
                RoadId = road.Id
            };
            context.Estimates.Add(estimate);
            context.SaveChanges();

            RoadWorksProgramViewModel roadWorksProgramData = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                Cost = 25000,
                EstimatesId = new()
                {
                    estimate.Id
                },
                RoadId = road.Id
            };

            // Act
            var result = controller.Post(roadWorksProgramData);

            // Assert
            Assert.NotNull(result);

            RoadWorksProgram roadWorksProgram = context.RoadWorksPrograms.Last();
            var roadWorksProgramId = ((ObjectResult)result).Value;

            Assert.Equal(roadWorksProgram.Id, roadWorksProgramId);

            RoadWorksProgramToEstimate roadWorksProgramToEstimate = context.RoadWorksProgramsToEstimates.Last();

            Assert.Equal(roadWorksProgramToEstimate.RoadWorksProgramId, roadWorksProgramId);
            Assert.Equal(roadWorksProgramToEstimate.EstimateId, estimate.Id);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void UpdateRoadWorksProgramTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadWorksProgramsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            Estimate estimate1 = new()
            {
                Name = "Смета 11",
                LevelOfWorks = 0.2,
                Cost = 20000,
                Link = "",
                RoadId = road.Id
            };
            context.Estimates.Add(estimate1);
            context.SaveChanges();

            Estimate estimate2 = new()
            {
                Name = "Смета 12",
                LevelOfWorks = 0.4,
                Cost = 40000,
                Link = "",
                RoadId = road.Id
            };
            context.Estimates.Add(estimate2);
            context.SaveChanges();

            RoadWorksProgram roadWorksProgram = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                Cost = 25000,
                RoadId = road.Id
            };
            context.RoadWorksPrograms.Add(roadWorksProgram);
            context.SaveChanges();

            RoadWorksProgramToEstimate roadWorksProgramToEstimate = new()
            {
                RoadWorksProgramId = roadWorksProgram.Id,
                EstimateId = estimate1.Id
            };
            context.RoadWorksProgramsToEstimates.Add(roadWorksProgramToEstimate);
            context.SaveChanges();

            RoadWorksProgramViewModel roadWorksProgramData = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                Cost = 25000,
                EstimatesId = new()
                {
                    estimate2.Id
                },
                RoadId = road.Id
            };

            // Act
            var result = controller.Put(roadWorksProgram.Id, roadWorksProgramData);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultRoadWorksProgram = JsonConvert.DeserializeObject<RoadWorksProgramEstimatesViewModel>(value.ToString());

            Assert.Equal(roadWorksProgram.Id, resultRoadWorksProgram.Id);
            Assert.Equal(roadWorksProgram.Year, resultRoadWorksProgram.Year);
            Assert.Equal(roadWorksProgram.Month, resultRoadWorksProgram.Month);
            Assert.Equal(roadWorksProgram.Cost, resultRoadWorksProgram.Cost);
            Assert.Equal(roadWorksProgram.RoadId, resultRoadWorksProgram.RoadId);

            Assert.Equal(estimate2.Id, resultRoadWorksProgram.Estimates.First().Id);
            Assert.Equal(estimate2.Name, resultRoadWorksProgram.Estimates.First().Name);
            Assert.Equal(estimate2.LevelOfWorks, resultRoadWorksProgram.Estimates.First().LevelOfWorks);
            Assert.Equal(estimate2.Cost, resultRoadWorksProgram.Estimates.First().Cost);
            Assert.Equal(estimate2.Link, resultRoadWorksProgram.Estimates.First().Link);
            Assert.Equal(estimate2.RoadId, resultRoadWorksProgram.Estimates.First().RoadId);

            roadWorksProgramToEstimate = context.RoadWorksProgramsToEstimates.Last();

            Assert.Equal(roadWorksProgramToEstimate.RoadWorksProgramId, resultRoadWorksProgram.Id);
            Assert.Equal(roadWorksProgramToEstimate.EstimateId, resultRoadWorksProgram.Estimates.First().Id);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void DeleteRoadWorksProgramTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadWorksProgramsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            Estimate estimate = new()
            {
                Name = "Смета 10",
                LevelOfWorks = 0.2,
                Cost = 20000,
                Link = "",
                RoadId = road.Id
            };
            context.Estimates.Add(estimate);
            context.SaveChanges();

            RoadWorksProgram roadWorksProgram = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                Cost = 25000,
                RoadId = road.Id
            };
            context.RoadWorksPrograms.Add(roadWorksProgram);
            context.SaveChanges();

            RoadWorksProgramToEstimate roadWorksProgramToEstimate = new()
            {
                RoadWorksProgramId = roadWorksProgram.Id,
                EstimateId = estimate.Id
            };
            context.RoadWorksProgramsToEstimates.Add(roadWorksProgramToEstimate);
            context.SaveChanges();

            int roadWorksProgramCount = context.RoadWorksPrograms.Count();

            // Act
            var result = controller.Delete(roadWorksProgram.Id);

            // Assert
            Assert.NotNull(result);

            var resultRoadWorksProgramCount = ((ObjectResult)result).Value;

            Assert.Equal(roadWorksProgramCount - 1, resultRoadWorksProgramCount);

            int roadWorksProgramToEstimateCount = context.RoadWorksProgramsToEstimates.Count();

            Assert.Equal(0, roadWorksProgramToEstimateCount);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }
    }
}
