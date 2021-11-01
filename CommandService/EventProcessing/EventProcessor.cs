using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace CommandService.EventProcessing {
    public class EventProcessor : IEventProcessor {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper) {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message) {
            var eventType = DetermineEvent(message);
            switch (eventType) {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage) {
            using (var scope = _scopeFactory.CreateScope()) {
                var repositoy = scope.ServiceProvider.GetService<ICommandRepository>();

                var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

                try {
                    var platform = _mapper.Map<Platform>(platformPublishedDTO);
                    if (!repositoy.ExternalPlatformExist(platform.ExternalID)) {
                        repositoy.CreatePlatform(platform);
                        repositoy.SaveChanges();
                        Console.WriteLine($"--> Platform added");
                    } else {
                        Console.WriteLine($"--> Platform already exist");
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"--> Could not add platform to db: {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage) {
            Console.WriteLine("--> Determining event...");
            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);

            switch (eventType.Event) {
                case "Platform_Published":
                    Console.WriteLine("--> Platform_Published event detected...");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Not determine event");
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType {
        PlatformPublished,
        Undetermined
    }
}
