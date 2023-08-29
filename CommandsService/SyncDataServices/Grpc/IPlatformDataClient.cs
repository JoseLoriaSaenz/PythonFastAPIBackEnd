using CommandsServices.Models;

namespace CommandsServices.SyncDataServices.Grpc
{   
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPLatforms();
    }
}