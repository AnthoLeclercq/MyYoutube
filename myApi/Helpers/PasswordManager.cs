namespace myApi.Helpers
{
    using BCrypt.Net;
    public class PasswordManager
    {
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password);
        }

        public static bool CheckPassword(string password, string hashPassword)
        {
            return BCrypt.Verify(password, hashPassword);
        }
    }
}
