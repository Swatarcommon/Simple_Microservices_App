using AuthenticationService.Models;
using System.Collections.Generic;

namespace AuthenticationService.Data {
    public interface IUserRepository {
        bool SaveChanges();

        //Users
        IEnumerable<User> GetAllUsers();
        User GetUser(int userId);
        void CreateUser(User user);
        bool UserExist(int userId);
        bool ExternalUserExist(int externalPlatformId);
    }
}
