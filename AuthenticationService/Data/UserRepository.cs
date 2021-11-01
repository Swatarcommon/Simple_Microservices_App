using AuthenticationService.Models;
using System.Collections.Generic;
using System.Linq;

namespace AuthenticationService.Data {
    public class UserRepository : IUserRepository {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) {
            _context = context;
        }

        public void CreateUser(User user) {
            _context.Users.Add(user);
        }

        public bool ExternalUserExist(int externalUserId) => _context.Users.Any(u => u.ExternalID == externalUserId);

        public IEnumerable<User> GetAllUsers() {
            return _context.Users;
        }

        public User GetUser(int externalUserId) {
            return _context.Users.FirstOrDefault(u => u.ExternalID == externalUserId);
        }

        public bool SaveChanges() {
            return _context.SaveChanges() >= 0;
        }

        public bool UserExist(int userId) => _context.Users.Any(u => u.Id == userId);
    }
}
