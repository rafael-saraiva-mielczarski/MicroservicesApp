using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Entities;


namespace PlatformServiceUnitTesting.Controllers
{
    public class PlatformControllerTest
    {
        [Fact]
        public void GetAllPlatforms_ShouldReturnAllPlatforms()


        {
            // Arrange
            var mockRepo = new Mock<IPlatformRepository>();
            var mockMapper = new Mock<IMapper>();

            var platforms = GetTestPlatforms();

            var platformDtos = platforms.Select(p => new PlatformReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Publisher = p.Publisher,
                Cost = p.Cost
            }).ToList();

            mockRepo.Setup(repo => repo.GetAllPlatforms()).Returns(platforms);
            mockMapper.Setup(m => m.Map<IEnumerable<PlatformReadDto>>(platforms)).Returns(platformDtos);

            var controller = new PlatformController(mockRepo.Object, mockMapper.Object);


            // Act
            var result = controller.GetPlatforms().Result as OkObjectResult;
            var returnValue = result.Value as IEnumerable<PlatformReadDto>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(platformDtos.Count, returnValue.Count());
        }



        private List<Platform> GetTestPlatforms()
        {
            var testPlatforms = new List<Platform>();
            testPlatforms.Add(new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" });
            testPlatforms.Add(new Platform { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" });
            testPlatforms.Add(new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundations", Cost = "Free" });

            return testPlatforms;
        }
    
    }
}
