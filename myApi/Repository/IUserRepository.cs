using myApi.Model.Token;
using myApi.Model.User;

namespace myApi.Repository
{
    public interface IUserRepository
    {
        UserOutput CreateUser(UserInput newUser);

        TokenOutput Authentification(string login, string password);

        bool DeleterUser(int id);

        UserOutput UpdateUser(string username, string pseudo, string email, string password, int id);

        List<UserOutput> GetAllUser(string Username, int page, int perPage);

        UserOutput GetUser(string id);

        int CountPagerOfUser(string pseudo);
    }
}