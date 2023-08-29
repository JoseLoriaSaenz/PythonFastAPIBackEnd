using AutoMapper;
using CommandsServices.Data;
using CommandsServices.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsServices.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repository, IMapper mapper) {

            _repo = repository;
            _mapper = mapper;
         }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("Inbound test of from Platforms Controller");
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine(" --> Getting Platforms from Command Service");

            var platforms = _repo.GetAllPlatforms();

            return  Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }     
    }
}