using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data {
	public interface IPlatformRepository {
		bool SaveChanges();
		IEnumerable<Platform> GetPlatforms();
		Platform GetPlatformById(int id);
		void AddPlatform(Platform platform);

	}
}