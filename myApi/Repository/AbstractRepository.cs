using MySql.Data.MySqlClient;

namespace myApi.Repository
{
    public class AbstractRepository
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnvironment;
        public string _connectionString;
        public string _keyString;
        public string _rootPath;


        public AbstractRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Database");
            _keyString = _configuration["Jwt:Key"];
            _hostingEnvironment = hostingEnvironment;
            _rootPath = _hostingEnvironment.WebRootPath;
        }

        public MySqlConnection OpenDbConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public void CloseDbConnection(MySqlConnection dbConn)
        {
            dbConn.Close();
        }

        public bool CheckIfUserIdExist(int id)
        {
            MySqlConnection conn = this.OpenDbConnection();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM USER WHERE ID = " + id, conn);

            MySqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                if (dr.GetInt32(0) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            conn.Close();
            return false;
        }

        public string GetUserQuery(string pseudo, int page, int perPage)
        {
            string query = "WHERE 1=1";

            if (!string.IsNullOrEmpty(pseudo))
                query += " AND pseudo LIKE '%" + pseudo + "%'";
            if (page > 0)
                query += " LIMIT " + perPage;
            if (perPage > 0 && page > 1)
                query += " OFFSET " + page;

            return query;
        }

        public string GetVideoQuery(string name, int user, int duration, int page, int perPage)
        {
            string query = "WHERE 1=1";
                                        //    throw new Exception("coucou");


            if (!string.IsNullOrEmpty(name))
                query += " AND vid.name LIKE '%" + name + "%'";
            if (user > 0)
                query += " AND user_id = " + user;
            if (duration > 0)
                query += " AND vid.duration = " + duration;
            if (page > 0)
                query += " LIMIT " + perPage;
            if (perPage > 0 && page > 1)
                query += " OFFSET " + page;

            return query;
        }
    }
}
