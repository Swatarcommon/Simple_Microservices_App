using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClinet;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepository repository, IMapper mapper,
                                    ICommandDataClient commandDataClient, IMessageBusClient messageBusClient) {
            _repository = repository;
            _mapper = mapper;
            _commandDataClinet = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms() {
            Debug.WriteLine("--> Getting Platforms...");

            var platformItems = _repository.GetPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDTO> GetPlatformById(int id) {
            Debug.WriteLine("--> Getting Platform by id...");
            var platformItem = _repository.GetPlatformById(id);
            if (platformItem != null)
                return Ok(_mapper.Map<PlatformReadDTO>(platformItem));

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDTO>> AddPlatform(PlatformCreateDTO platformCreateDto) {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.AddPlatform(platformModel);

            _repository.SaveChanges();
            var platfromReadDTO = _mapper.Map<PlatformReadDTO>(platformModel);
            //Send sync message
            try {
                await _commandDataClinet.SendPlatformToCommand(platfromReadDTO);
            } catch (Exception ex) {
                Debug.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            //Send async message
            try {
                var platformPublishedDTO = _mapper.Map<PlatformPublishedDTO>(platfromReadDTO);
                platformPublishedDTO.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDTO);
            } catch (Exception ex) {
                Debug.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platfromReadDTO.Id }, platfromReadDTO);
        }
    }
}