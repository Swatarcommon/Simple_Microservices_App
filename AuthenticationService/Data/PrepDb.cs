using AuthenticationService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AuthenticationService.Data {
    public static class PrepDb {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder) {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) {
                //var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
                //var platforms = grpcClient.ReturnAllPlatforms();

                var users = new List<User>() {
                    new User(){ Email = "swatarcommon_0@gmail.com", Password="134576290", ExternalID=1 },
                    new User(){ Email = "swatarcommon_1@gmail.com", Password="134576290", ExternalID=2 },
                    new User(){ Email = "swatarcommon_2@gmail.com", Password="134576290", ExternalID=3 },
                    new User(){  Email = "swatarcommon_3@gmail.com", Password="134576290", ExternalID=4 },
                    };

                SeedData(serviceScope.ServiceProvider.GetService<IUserRepository>(), users);
            }
        }

        private static void SeedData(IUserRepository repository, IEnumerable<User> users) {
            Console.WriteLine("--> Seeding new user to authentication service...");
            foreach (var user in users) {
                if (!repository.UserExist(user.ExternalID)) {
                    repository.CreateUser(user);
                }
                repository.SaveChanges();
            }
        }
    }
}
