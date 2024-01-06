using DSS.Controllers.ApiControllers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace DSS.Tests
{
    public class RoadsApiControllerTests
    {
        private readonly Mock<ILogger<RoadsApiController>> _mock = new();
        private readonly IServiceProvider _serviceProvider;

        public RoadsApiControllerTests()
        {
            _serviceProvider = DependencyInjection.InitilizeServices().BuildServiceProvider();
        }

        [Fact]
        public void GetAllRoadsTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadsApiController(context, _mock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void GetRoadByIdTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            // Act
            var result = controller.Get(road.Id);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultRoad = JsonConvert.DeserializeObject<Road>(value.ToString());

            Assert.Equal(road.Id, resultRoad.Id);
            Assert.Equal(road.Number, resultRoad.Number);
            Assert.Equal(road.Priority, resultRoad.Priority);
            Assert.Equal(road.LinkToPassport, resultRoad.LinkToPassport);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void CreateRoadTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadsApiController(context, _mock.Object);

            RoadViewModel roadData = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };

            // Act
            var result = controller.Post(roadData);

            // Assert
            Assert.NotNull(result);

            Road road = context.Roads.Last();
            var roadId = ((ObjectResult)result).Value;

            Assert.Equal(road.Id, roadId);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void UpdateRoadTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            RoadViewModel roadData = new()
            {
                Number = "18 ОП РЗ 18Р-11",
                Priority = 2,
                LinkToPassport = ""
            };

            // Act
            var result = controller.Put(road.Id, roadData);

            // Assert
            Assert.NotNull(result);

            var value = ((ObjectResult)result).Value;
            var resultRoad = JsonConvert.DeserializeObject<Road>(value.ToString());

            Assert.Equal(road.Id, resultRoad.Id);
            Assert.Equal(roadData.Number, resultRoad.Number);
            Assert.Equal(roadData.Priority, resultRoad.Priority);
            Assert.Equal(roadData.LinkToPassport, resultRoad.LinkToPassport);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void DeleteRoadTest()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var controller = new RoadsApiController(context, _mock.Object);

            Road road = new()
            {
                Number = "18 ОП РЗ 18Р-10",
                Priority = 1,
                LinkToPassport = ""
            };
            context.Roads.Add(road);
            context.SaveChanges();

            int roadCount = context.Roads.Count();

            // Act
            var result = controller.Delete(road.Id);

            // Assert
            Assert.NotNull(result);

            var resultRoadCount = ((ObjectResult)result).Value;

            Assert.Equal(roadCount - 1, resultRoadCount);

            var statusCode = ((ObjectResult)result).StatusCode;
            Assert.Equal(200, statusCode);
        }
    }
}
