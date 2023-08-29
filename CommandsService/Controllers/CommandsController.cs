using AutoMapper;
using CommandsServices.Data;
using CommandsServices.Dtos;
using CommandsServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsServices.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands(int platformId)
        {
            Console.WriteLine($"--> Getting all commands for platform : {platformId}");

            if (!_repo.PlatformExits(platformId))
               return NotFound($"Platform: {platformId} doesn't exits!");
              
            var commands = _repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name ="GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {   
            Console.WriteLine($"--> Getting command {commandId} for platform : {platformId}");

            if (!_repo.PlatformExits(platformId))
               return NotFound($"Platform: {platformId} doesn't exits!");

            var command = _repo.GetCommand(platformId, commandId);

            if (command == null)
                return NotFound($"Command: {commandId} doesn't exits!");

            return Ok(_mapper.Map<CommandReadDto>(command));
      
        }

        [HttpPost]
        public ActionResult<CommandReadDto> PostCommand(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Creating command for platform : {platformId}");

            if (!_repo.PlatformExits(platformId))
               return NotFound($"Platform: {platformId} doesn't exits!");

            var newCommand = _mapper.Map<Command>(commandDto);
            _repo.CreateCommand(platformId, newCommand);
            _repo.SaveChanges();

            return Ok(_mapper.Map<CommandReadDto>(newCommand));

        }

    }
}