#nullable disable

using CommandsServices.Models;
using CommandsServices.SyncDataServices.Grpc;

namespace CommandsServices.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using( var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                Console.WriteLine("--> Retrieving platforms");
                var platforms =  grpcClient.ReturnAllPLatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);

            }
        }

        private static void SeedData(ICommandRepo repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms");

            foreach (var platform in platforms)
            {
                if (!repository.ExternalPlatformExist(platform.ExtenralId))
                {
                    Console.WriteLine($"--> Adding platform: {platform.ExtenralId}, Name: {platform.Name}");
                    repository.CreatePlatform(platform);
                }
                repository.SaveChanges();
            }
        }
    }
}