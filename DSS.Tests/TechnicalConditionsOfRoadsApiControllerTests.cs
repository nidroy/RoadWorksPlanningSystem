namespace DSS.Tests
{
    public class TechnicalConditionsOfRoadsApiControllerTests
    {
        private readonly Mock<ILogger<ApiController>> _mock = new();
        private readonly IServiceProvider _serviceProvider;

        public TechnicalConditionsOfRoadsApiControllerTests()
        {
            _serviceProvider = DependencyInjection.InitilizeServices().BuildServiceProvider();
        }

        [Fact]
        public void ReadAllTechnicalConditionsOfRoadsTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new TechnicalConditionsOfRoadsApiController(context, _mock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void ReadTechnicalConditionOfRoadByIdTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new TechnicalConditionsOfRoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            TechnicalConditionOfRoad technicalConditionOfRoad = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                TechnicalCondition = 2.5,
                RoadId = road.Id
            };
            context.TechnicalConditionsOfRoads.Add(technicalConditionOfRoad);
            context.SaveChanges();

            // Act
            var result = controller.Get(technicalConditionOfRoad.Id);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultTechnicalConditionOfRoad = JsonConvert.DeserializeObject<TechnicalConditionOfRoad>(value.ToString());

            Assert.Equal(technicalConditionOfRoad.Id, resultTechnicalConditionOfRoad.Id);
            Assert.Equal(technicalConditionOfRoad.Year, resultTechnicalConditionOfRoad.Year);
            Assert.Equal(technicalConditionOfRoad.Month, resultTechnicalConditionOfRoad.Month);
            Assert.Equal(technicalConditionOfRoad.TechnicalCondition, resultTechnicalConditionOfRoad.TechnicalCondition);
            Assert.Equal(technicalConditionOfRoad.RoadId, resultTechnicalConditionOfRoad.RoadId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void CreateTechnicalConditionOfRoadTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new TechnicalConditionsOfRoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            TechnicalConditionOfRoadViewModel technicalConditionOfRoadData = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                TechnicalCondition = 2.5,
                RoadId = road.Id
            };

            // Act
            var result = controller.Post(technicalConditionOfRoadData);

            // Assert
            Assert.NotNull(result);

            TechnicalConditionOfRoad technicalConditionOfRoad = context.TechnicalConditionsOfRoads.Last();
            var technicalConditionOfRoadId = ((ObjectResult)result).Value;

            Assert.Equal(technicalConditionOfRoad.Id, technicalConditionOfRoadId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void UpdateTechnicalConditionOfRoadTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new TechnicalConditionsOfRoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            TechnicalConditionOfRoad technicalConditionOfRoad = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                TechnicalCondition = 2.5,
                RoadId = road.Id
            };
            context.TechnicalConditionsOfRoads.Add(technicalConditionOfRoad);
            context.SaveChanges();

            TechnicalConditionOfRoadViewModel technicalConditionOfRoadData = new()
            {
                Year = 2002,
                Month = "Октябрь",
                TechnicalCondition = 4,
                RoadId = road.Id
            };

            // Act
            var result = controller.Put(technicalConditionOfRoad.Id, technicalConditionOfRoadData);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultTechnicalConditionOfRoad = JsonConvert.DeserializeObject<TechnicalConditionOfRoad>(value.ToString());

            Assert.Equal(technicalConditionOfRoad.Id, resultTechnicalConditionOfRoad.Id);
            Assert.Equal(technicalConditionOfRoadData.Year, resultTechnicalConditionOfRoad.Year);
            Assert.Equal(technicalConditionOfRoadData.Month, resultTechnicalConditionOfRoad.Month);
            Assert.Equal(technicalConditionOfRoadData.TechnicalCondition, resultTechnicalConditionOfRoad.TechnicalCondition);
            Assert.Equal(technicalConditionOfRoadData.RoadId, resultTechnicalConditionOfRoad.RoadId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void DeleteTechnicalConditionOfRoadTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new TechnicalConditionsOfRoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            TechnicalConditionOfRoad technicalConditionOfRoad = new()
            {
                Year = 2001,
                Month = "Сентябрь",
                TechnicalCondition = 2.5,
                RoadId = road.Id
            };
            context.TechnicalConditionsOfRoads.Add(technicalConditionOfRoad);
            context.SaveChanges();

            int technicalConditionOfRoadCount = context.TechnicalConditionsOfRoads.Count();

            // Act
            var result = controller.Delete(technicalConditionOfRoad.Id);

            // Assert
            Assert.NotNull(result);

            var resultTechnicalConditionOfRoadCount = ((ObjectResult)result).Value;

            Assert.Equal(technicalConditionOfRoadCount - 1, resultTechnicalConditionOfRoadCount);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }
    }
}
