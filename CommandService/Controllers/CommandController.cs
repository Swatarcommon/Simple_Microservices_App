using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CommandService.Controllers {
    [ApiController]
    [Route("api/commands/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTO>> GetCommandsForPlatform(int platformId) {
            Console.WriteLine("--> Getting Commands for Platform...");
            if (!_repository.PlatformExist(platformId)) {
                return NotFound();
            }
            var commands = _repository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDTO> GetCommandForPlatform(int platformId, int commandId) {
            Console.WriteLine("--> Getting Command for Platform...");
            if (!_repository.PlatformExist(platformId)) {
                return NotFound();
            }
            var command = _repository.GetCommand(platformId, commandId);

            if (command == null) {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDTO>(command));
        }


        [HttpPost]
        public ActionResult<CommandReadDTO> CreateCommandForPlatform(int platformId, CommandCreateDTO commandDTO) {
            Console.WriteLine("--> Create Command for Platform...");
            if (!_repository.PlatformExist(platformId)) {
                return NotFound();
            }
            var command = _mapper.Map<Command>(commandDTO);

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();
            var commandReadDTO = _mapper.Map<CommandReadDTO>(command);
            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDTO.Id }, commandReadDTO);
        }
    }
}
