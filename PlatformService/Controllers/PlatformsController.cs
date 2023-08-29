using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper, 
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            Console.WriteLine("Getting All Platforms...");
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_repository.GetAllPlatforms()));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine(string.Format("Getting Platform by Id --> {0}...", id));
            var platform = _repository.GetPlatformById(id);

            if (platform == null)
                return NotFound(string.Format("Platform {0} resource doesn't exists", id));

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
        {
            Console.WriteLine("Creating new platform...");
            
            var model = _mapper.Map<Platform>(platform);
            _repository.CreatePlatform(model);
            _repository.SaveChanges();

            var newPlatform = _mapper.Map<PlatformReadDto>(model);

            //Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(newPlatform);
            }
            catch (Exception ex)
            {                
                Console.WriteLine($"--> Couldn't send message synchronously: {ex.Message}");
            }

            //Send Async Message
            try
            {
                var platformPublihedDto = _mapper.Map<PlatformPublishedDto>(newPlatform);
                platformPublihedDto.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublihedDto);

            }
            catch (Exception ex)
            {                
                Console.WriteLine($"--> Couldn't send message asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = newPlatform.Id }, newPlatform);
        }
    }
}