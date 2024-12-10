using System.Security.Claims;
using myApi.Helpers;
using myApi.Model.Token;
using myApi.Model.User;
using MySql.Data.MySqlClient;

namespace myApi.Repository
{
    public class UserRepository : AbstractRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) : base(configuration, hostingEnvironment)
        {
        }

        public TokenOutput Authentification(string login, string password)
        {
            MySqlConnection conn = this.OpenDbConnection();

            conn.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT id, Username, pseudo, created_at, email, password FROM USER WHERE Username = '" + login + "'", conn);

            MySqlDataReader dr = cmd.ExecuteReader();

            TokenOutput Token = new TokenOutput();

            string passwordDb = "";

            while (dr.Read())
            {
                Token.User = new UserOutput
                {
                    Id = dr.GetInt32(0),
                    Username = dr.GetString(1),
                    Pseudo = dr.GetString(2),
                    Created_at = dr.GetDateTime(3),
                    Email = dr.GetString(4),
                };

                passwordDb = dr.GetString(5);

            }

            this.CloseDbConnection(conn);

            if (login == Token.User.Username && PasswordManager.CheckPassword(password, passwordDb))
            {
                Token.Token = TokenManager.GenerateToken(_keyString, new List<Claim> { new Claim(ClaimTypes.Name, Token.User.Username) });
                return Token;

            }
            else
            {
                throw new Exception("Not Found");
            }
        }

        public UserOutput CreateUser(UserInput newUser)
        {
            MySqlConnection conn = this.OpenDbConnection();

            conn.Open();

            string password = PasswordManager.HashPassword(newUser.Password);

            MySqlCommand cmd = new MySqlCommand(String.Format(@"
                INSERT INTO `myapi`.`USER` (`Username`, `Pseudo`, `created_at`, `email`, `password`) 
                VALUES ('{0}', '{1}', NOW() , '{2}', '{3}')", newUser.Username, newUser.Pseudo, newUser.Email, password), conn);


            int nbrOfRowAffected = cmd.ExecuteNonQuery();

            UserOutput user = new UserOutput();

            if (nbrOfRowAffected > 0)
            {
                cmd = new MySqlCommand("SELECT id FROM USER ORDER BY ID DESC LIMIT 1", conn);

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    user.Id = dr.GetInt32(0);
                }

                user.Username = newUser.Username;
                user.Pseudo = newUser.Pseudo;
                user.Created_at = DateTime.Now;
                user.Email = newUser.Email;
            }

            this.CloseDbConnection(conn);

            return user;
        }

        public bool DeleterUser(int id)
        {
            MySqlConnection conn = this.OpenDbConnection();

            MySqlCommand cmd = new MySqlCommand("DELETE FROM USER WHERE ID = '" + id + "'", conn);

            conn.Open();

            if (CheckIfUserIdExist(id))
            {
                int result = cmd.ExecuteNonQuery();

                this.CloseDbConnection(conn);

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            } else
            {
                throw new Exception("User doesn't exist");
            }
        }


        public int CountPagerOfUser(string pseudo)
        {
            MySqlConnection conn = this.OpenDbConnection();
            conn.Open();

            string haveWhere = (!string.IsNullOrEmpty(pseudo)) ? " AND pseudo LIKE '%" + pseudo + "%'" : "";

            string query = string.Format("SELECT COUNT(*) FROM USER WHERE 1=1 {0}", haveWhere);

            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader dr = cmd.ExecuteReader();

            int result = 0;

            while(dr.Read())
            {
                result = dr.GetInt32(0);
            }

            return result;
        }
        public List<UserOutput> GetAllUser(string pseudo,int page, int perPage)
        {
            MySqlConnection conn = this.OpenDbConnection();
            conn.Open();

            string query = "";

            if (page > 1)
            {
                int currentPage = (page - 1) * perPage;
                query = string.Format(@"SELECT id, Username, Pseudo, email, created_at
                             FROM USER  
                             {0}", GetUserQuery(pseudo, currentPage, perPage));
            }
            else
            {
                query = string.Format(@"SELECT id, Username, Pseudo, email, created_at
                             FROM USER  
                             {0}", GetUserQuery(pseudo,page,perPage));
            }
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader dr = cmd.ExecuteReader();

            List<UserOutput> resultUser = new List<UserOutput>();

            while (dr.Read())
            {
                UserOutput userTemp = new UserOutput
                {
                    Id = dr.GetInt32(0),
                    Username = dr.GetString(1),
                    Pseudo = dr.GetString(2),
                    Email = dr.GetString(3),
                    Created_at = dr.GetDateTime(4)
                };

                resultUser.Add(userTemp);
            }
            this.CloseDbConnection(conn);

       

            return resultUser;

        }

        public UserOutput GetUser(string id)
        {
            if (CheckIfUserIdExist( Int32.Parse(id)))
                {
                MySqlConnection conn = this.OpenDbConnection();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT id, USERNAME, PSEUDO, EMAIL, created_at FROM USER WHERE ID = '" + id + "'", conn);

                MySqlDataReader dr = cmd.ExecuteReader();

                UserOutput user = new UserOutput();

                while (dr.Read())
                {
                    user.Id = dr.GetInt32(0);
                    user.Username = dr.GetString(1);
                    user.Pseudo = dr.GetString(2);
                    user.Email = dr.GetString(3);
                    user.Created_at = dr.GetDateTime(4);
                }

                this.CloseDbConnection(conn);

                return user;

            } else
            {
                throw new Exception("Utilisateur introuvable");
            }
        }

        public UserOutput UpdateUser(string username, string pseudo, string email, string password, int id)
        {
            if (CheckIfUserIdExist(id))
            {
                MySqlConnection conn = this.OpenDbConnection();
                conn.Open();

                string passwordHash = PasswordManager.HashPassword(password);

                MySqlCommand cmd = new MySqlCommand(String.Format(@"
                UPDATE `myapi`.`USER` 
                SET `Username` =  '{0}', `Pseudo` = '{1}', `email` = '{2}', `password` = '{3}' 
                WHERE `id` = {4}", username, pseudo, email, passwordHash, id), conn);


                int result = cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("SELECT created_at FROM USER WHERE id = "+id, conn);

                MySqlDataReader dr = cmd.ExecuteReader();

                DateTime created_at = new DateTime();
                while(dr.Read())
                {
                    created_at = dr.GetDateTime(0);
                }

                this.CloseDbConnection(conn);

                return new UserOutput
                {
                    Id = id,
                    Username = username,
                    Pseudo = pseudo,
                    Email = email, 
                    Created_at = created_at                    
                };
            } else
            {
                throw new Exception("Not Found");
            }
        }
    }
}
