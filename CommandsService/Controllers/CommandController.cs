using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("/api/c/platforms/{platformId:required}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of command items from a platform.
        /// </summary>
        ///  /// <remarks>
        /// Example request:
        /// ```http
        /// GET /api/c/platforms/b6d4c520-f927-4c37-aa5c-3a6fa688c9f8/commands
        /// ```
        /// Example response:
        /// ```json
        /// [
        ///     { "id": "48f6a78e-6ba4-4553-8640-c66ab6906a42", "howTo": "I dont know", "CommandLine": "dont know", "platformId": "b6d4c520-f927-4c37-aa5c-3a6fa688c9f8" },
        ///     { "id": "a70eb49d-00a2-41df-8c42-d800f3669004", "howTo": "Click on the button show all containers", "CommandLine": "docker ps", "platformId": "b6d4c520-f927-4c37-aa5c-3a6fa688c9f8" }
        /// ]
        /// ```
        /// </remarks>
        /// <response code="200">The list of commands items from the platform was successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CommandReadDto>))]
        public ActionResult<IEnumerable<Command>> GetAllCommandsFromPlatform([FromRoute]Guid platformId) {

            var commands = _commandRepository.GetCommandsForPlatform(platformId);
            
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        /// <summary>
        /// Retrieves a command item from a platform.
        /// </summary>
        ///  /// <remarks>
        /// Example request:
        /// ```http
        /// GET /api/c/platforms/b6d4c520-f927-4c37-aa5c-3a6fa688c9f8/commands/48f6a78e-6ba4-4553-8640-c66ab6906a42
        /// ```
        /// Example response:
        /// ```json
        ///   { "id": "48f6a78e-6ba4-4553-8640-c66ab6906a42", "howTo": "I dont know", "CommandLine": "dont know", "platformId": "b6d4c520-f927-4c37-aa5c-3a6fa688c9f8" },
        /// ```
        /// </remarks>
        /// <response code="200">The commands item from the platform was successfully retrieved.</response>
        /// <response code="404">The command item specified was not found.</response>
        [HttpGet("/{commandId:required}", Name = "GetCommandForPlatform")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommandReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CommandReadDto> GetCommandForPlatform([FromRoute] Guid platformId, [FromRoute] Guid commandId)
        {
            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound("The platform specified was not found.");
            }

            var command = _commandRepository.GetCommand(platformId, commandId);

            if (command == null)
                return NotFound("The command specified was not found.");

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        /// <summary>
        /// Creates a new command item for a platform.
        /// </summary>
        ///  /// <remarks>
        /// Example request:
        /// ```http
        /// POST /api/c/platforms/b6d4c520-f927-4c37-aa5c-3a6fa688c9f8/commands/
        /// ```
        /// ```json
        ///     { "howTo": "I dont know", "commandLine": "dk" }
        /// ```
        /// Example response:
        /// ```json
        ///     { "id": "48f6a78e-6ba4-4553-8640-c66ab6906a42", "howTo": "I dont know", "commandLine": "dk", "platformId": "b6d4c520-f927-4c37-aa5c-3a6fa688c9f8" },
        /// ```
        /// </remarks>
        /// <response code="201">The command item was successufuly created.</response>
        /// <response code="400">There is one or more required fields missing.</response>
        /// <response code="404">The platform specified was not found.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommandReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CommandCreateDto> CreateCommand([FromRoute] Guid platformId, [FromBody] CommandCreateDto commandCreateDto) {

            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound("The platform specified was not found.");
            }

            var command = _mapper.Map<Command>(commandCreateDto);

            _commandRepository.CreateCommand(command.PlatformId, command);
            _commandRepository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = commandReadDto.PlatformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}
