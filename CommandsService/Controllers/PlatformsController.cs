using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository commandRepository, IMapper mapper)
        { 
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of platform items.
        /// </summary>
        ///  /// <remarks>
        /// Example request:
        /// ```http
        /// GET /api/Platform
        /// ```
        /// Example response:
        /// ```json
        /// [
        ///     { "id": "48f6a78e-6ba4-4553-8640-c66ab6906a42", "name": "Dot Net" },
        ///     { "id": "a70eb49d-00a2-41df-8c42-d800f3669004", "name": "SQL Server" }
        /// ]
        /// ```
        /// </remarks>
        /// <response code="200">The list of platform items was successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PlatformReadDto>))]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() {
            var platformItems = _commandRepository.GetAllPlatforms();
            
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        /// <summary>
        /// Test communication between services
        /// </summary>
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound Post # Command Service");

            return Ok("Inbound test of from Platforms Controller");
        }
    }
}
