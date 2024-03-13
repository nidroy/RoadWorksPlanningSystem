namespace DSS.Tests
{
    public class EstimatesApiControllerTests
    {
        private readonly Mock<ILogger<ApiController>> _mock = new();
        private readonly IServiceProvider _serviceProvider;

        public EstimatesApiControllerTests()
        {
            _serviceProvider = DependencyInjection.InitilizeServices().BuildServiceProvider();
        }

        [Fact]
        public void ReadAllEstimatesTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new EstimatesApiController(context, _mock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void ReadEstimateByIdTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new EstimatesApiController(context, _mock.Object);

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

            // Act
            var result = controller.Get(estimate.Id);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultEstimate = JsonConvert.DeserializeObject<Estimate>(value.ToString());

            Assert.Equal(estimate.Id, resultEstimate.Id);
            Assert.Equal(estimate.Name, resultEstimate.Name);
            Assert.Equal(estimate.LevelOfWorks, resultEstimate.LevelOfWorks);
            Assert.Equal(estimate.Cost, resultEstimate.Cost);
            Assert.Equal(estimate.Link, resultEstimate.Link);
            Assert.Equal(estimate.RoadId, resultEstimate.RoadId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void CreateEstimateTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new EstimatesApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            EstimateViewModel estimateData = new()
            {
                Name = "Смета 10",
                LevelOfWorks = 0.2,
                Cost = 20000,
                Link = "",
                RoadId = road.Id,
            };

            // Act
            var result = controller.Post(estimateData);

            // Assert
            Assert.NotNull(result);

            Estimate estimate = context.Estimates.Last();
            var estimateId = ((ObjectResult)result).Value;

            Assert.Equal(estimate.Id, estimateId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void UpdateEstimateTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new EstimatesApiController(context, _mock.Object);

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

            EstimateViewModel estimateData = new()
            {
                Name = "Смета 11",
                LevelOfWorks = 0.4,
                Cost = 45000,
                Link = "",
                RoadId = road.Id
            };

            // Act
            var result = controller.Put(estimate.Id, estimateData);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultEstimate = JsonConvert.DeserializeObject<Estimate>(value.ToString());

            Assert.Equal(estimate.Id, resultEstimate.Id);
            Assert.Equal(estimateData.Name, resultEstimate.Name);
            Assert.Equal(estimateData.LevelOfWorks, resultEstimate.LevelOfWorks);
            Assert.Equal(estimateData.Cost, resultEstimate.Cost);
            Assert.Equal(estimateData.Link, resultEstimate.Link);
            Assert.Equal(estimateData.RoadId, resultEstimate.RoadId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void DeleteEstimateTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new EstimatesApiController(context, _mock.Object);

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

            int estimateCount = context.Estimates.Count();

            // Act
            var result = controller.Delete(estimate.Id);

            // Assert
            Assert.NotNull(result);

            var resultEstimateCount = ((ObjectResult)result).Value;

            Assert.Equal(estimateCount - 1, resultEstimateCount);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }
    }
}
