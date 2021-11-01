using CommandService.Models;
using System.Collections.Generic;

namespace CommandService.SynDataServices.Grpc {
    public interface IPlatformDataClient {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}
