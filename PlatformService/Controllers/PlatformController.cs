using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Entities;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
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
        ///     { "id": "48f6a78e-6ba4-4553-8640-c66ab6906a42", "name": "Dot Net", "publisher": "Microsoft", "cost": "Free"},
        ///     { "id": "a70eb49d-00a2-41df-8c42-d800f3669004", "name": "SQL Server", "publisher": "Microsoft", "cost": "Free" }
        /// ]
        /// ```
        /// </remarks>
        /// <response code="200">The list of platform items was successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PlatformReadDto>))]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platformItem = _platformRepository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        /// <summary>
        /// Retrieves a specific platform item by Id.
        /// </summary>
        /// <param name="id">The unique identifier of the platform item.</param>
        /// <returns>A specific platform</returns>
        /// <response code="200">The platform item was successfully retrieved.</response>
        /// <response code="404">The platform item with the given ID was not found.</response>
        [HttpGet("{id:required}", Name = "GetPlatformById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PlatformReadDto> GetPlatformById([FromRoute] Guid id)
        {
            var platformItem = _platformRepository.GetPlatformById(id);

            if (platformItem == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
        }

        /// <summary>
        /// Creates a new platform item.
        /// </summary>
        ///  /// <remarks>
        /// Example request:
        /// ```http
        /// POST /api/Platform
        /// ```
        /// ```json
        ///     { "name": "Dot Net", "publisher": "Microsoft", "cost": "Free"}
        /// ```
        /// Example response:
        /// ```json
        ///     { "id": "48f6a78e-6ba4-4553-8640-c66ab6906a42", "name": "Dot Net", "publisher": "Microsoft", "cost": "Free"}
        /// ```
        /// </remarks>
        /// <response code="201">The platform item was successufuly created.</response>
        /// <response code="400">There is one or more required fields missing.</response>
        /// <response code="409">The platform you tried creating already exist.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlatformReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto platform)
        {
            var platformAlreadyExists = _platformRepository.GetPlatformByName(platform.Name);

            if (platformAlreadyExists == null)
            {
                var platformModel = _mapper.Map<Platform>(platform);

                _platformRepository.CreatePlatform(platformModel);
                _platformRepository.SaveChanges();

                var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

                // Sending Sync Message
                try
                {
                    await _commandDataClient.SendPlatformToCommand(platformReadDto);
                }
                catch (Exception ex)
                {
                    throw new Exception($"--> Could not send synchronously {ex.Message}");
                }

                // Sending Async Message
                try
                {
                    var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                    platformPublishedDto.Event = "Platform_Published";

                    _messageBusClient.PublishNewPlatform(platformPublishedDto);
                }
                catch (Exception ex)
                {
                    throw new Exception($"--> Could not send asynchronously {ex.Message}");
                }

                return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
            }
            else
            {
                return Conflict("The platform you tried creating already exists!");
            }
        }
    }
}
