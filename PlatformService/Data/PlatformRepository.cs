using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data {
	public class PlatformRepository : IPlatformRepository {
		private readonly AppDbContext _context;

		public PlatformRepository(AppDbContext context) {
			_context = context;
		}

		public bool SaveChanges() {
			return _context.SaveChanges() >= 0;
		}

		public IEnumerable<Platform> GetPlatforms() {
			return _context.Platforms;
		}

		public Platform GetPlatformById(int id) {
			return _context.Platforms.FirstOrDefault(p => p.Id == id);
		}

		public void AddPlatform(Platform platform) {
			if (platform == null) {
				throw new ArgumentNullException(nameof(platform));
			}

			_context.Platforms.Add(platform);
		}
	}
}