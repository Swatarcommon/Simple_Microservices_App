using CommandService.Models;
using CommandService.SynDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CommandService.Data {
    public static class PrepDb {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder) {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) {
                var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
            }
        }

        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms) {
            Console.WriteLine("--> Seeding new platforms to command service...");
            foreach (var platform in platforms) {
                if (!repository.PlatformExist(platform.ExternalID)) {
                    repository.CreatePlatform(platform);
                }
                repository.SaveChanges();
            }
        }
    }
}
