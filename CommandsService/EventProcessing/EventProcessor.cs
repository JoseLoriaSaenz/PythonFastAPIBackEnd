
#nullable disable

using System.Text.Json;
using AutoMapper;
using CommandsServices.Data;
using CommandsServices.Dtos;
using CommandsServices.Models;

namespace CommandsServices.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine($"--> Determining Event {notificationMessage}");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine($"--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine($"--> Couldn't determine Event Type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string PlatformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(PlatformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);                    

                    if (!repo.ExternalPlatformExist(platform.ExtenralId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();

                        Console.WriteLine($"Platform Added: Id: {platform.Id}, Name: {platform.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Platform {platform.ExtenralId} already exists");
                    }
                }
                catch (Exception e)
                {                    
                    Console.WriteLine($"Could not add platform to DB {e.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}