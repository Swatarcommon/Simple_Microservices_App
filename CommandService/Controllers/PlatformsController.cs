using System;
using AutoMapper;
using CommandService.Data;
using Microsoft.AspNetCore.Mvc;
using CommandService.DTOs;
using System.Collections.Generic;

namespace CommandService.Controllers {
    [ApiController]
    [Route("api/commands/[controller]")]
    public class PlatformsController : ControllerBase {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms() {
            Console.WriteLine("--> Getting platforms from Command Service...");
            var platformItems = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems));
        }

        [HttpPost]
        public ActionResult CreatePlatform() {

            return Ok("Okay from Command Post Controller");
        }
    }
}