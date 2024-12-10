﻿using myApi.Model.Video;
using MySql.Data.MySqlClient;
using RestSharp;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;


namespace myApi.Repository

{
	public class VideoRepository : AbstractRepository, IVideoRepository
	{
		public VideoRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) : base(configuration, hostingEnvironment)
		{
		}

		public async Task<VideoOutput> UploadVideo(int id, string sourceName, IFormFile video)
		{
			if (video != null && video.Length > 0)
			{
				if (!CheckIfUserIdExist(id))
					throw new Exception("L'utilisateur n'existe pas");

				MySqlConnection conn = this.OpenDbConnection();
				conn.Open();

				DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
				string unixTime = dto.ToUnixTimeSeconds().ToString();


				MySqlCommand cmd = new MySqlCommand(String.Format(@"
                INSERT INTO `myapi`.`video` (`name`, `user_id`, `source`, `created_at`, `duration`) 
                VALUES ('{0}', {1}, '{2}', NOW(), {3})", sourceName, id, "/video/" + unixTime + "_" + id + "_" + video.FileName, 10), conn);

				int nbrRowAffected = cmd.ExecuteNonQuery();

				if (nbrRowAffected > 0)
				{

					string hash = "";
					string source = unixTime + "_" + id + "_" + video.FileName;
					source = source.Substring(0, 10);
					using (SHA1 sha1Hash = SHA1.Create())
					{
						//From String to byte array
						byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
						byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
						hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

						Console.WriteLine("The SHA1 hash of " + source + " is: " + hash);
					}

					string testSave = Path.Combine("wwwroot", "video", hash);
					Directory.CreateDirectory(testSave);

					string filePath = Path.Combine(testSave, unixTime + "_" + id + "_" + video.FileName.Replace(" ", "-"));
					using (Stream fileStream = new FileStream(filePath, FileMode.Create))
					{
						await video.CopyToAsync(fileStream);
					}


					cmd = new MySqlCommand("SELECT vid.id, vid.source, vid.created_at, vid.`view`, vid.enabled, USER.id AS user_id , USER.Username, USER.Pseudo, USER.created_at, USER.email FROM video vid LEFT JOIN USER ON vid.user_id = USER.id ORDER BY ID DESC LIMIT 1", conn);

					MySqlDataReader dr = cmd.ExecuteReader();

					VideoOutput vid = new VideoOutput();

					while (dr.Read())
					{
						vid.Id = dr.GetString(0);
						vid.Source = dr.GetString(1);
						vid.Created_at = dr.GetDateTime(2);
						vid.Views = dr.GetInt32(3);
						vid.Enabled = dr.GetBoolean(4);
						vid.User = new Model.User.UserOutput
						{
							Id = dr.GetInt32(5),
							Username = dr.GetString(6),
							Pseudo = dr.GetString(7),
							Created_at = dr.GetDateTime(8),
							Email = dr.GetString(9)
						};
					}


					this.CloseDbConnection(conn);

					var client = new RestClient("http://encoding/");
					var request = new RestRequest("uploadVideo.php", Method.Post);
					request.AlwaysMultipartFormData = true;
					request.AddHeader("Content-Type", "multipart/form-data");
					request.AddFile("file", filePath);
					request.AddJsonBody(new { id = id}); // Anonymous type object is converted to Json body

					RestResponse response = client.Execute(request);
					Console.WriteLine("Errors sending to encoding : " + response.ErrorMessage);

					return vid;
				}
				else
				{
					throw new Exception("Impossible de sauvegarder les données dans la base");
				}
			}
			else
			{
				throw new Exception("Le fichier est vide ou n'existe pas.");
			}
		}

		public async Task<VideoOutput> SaveEncodedVideo(int id, string sourceName, IFormFile video)
		{
			if (video != null && video.Length > 0)
			{
				if (!CheckIfUserIdExist(id))
					throw new Exception("L'utilisateur " + id + " n'existe pas");

				MySqlConnection conn = this.OpenDbConnection();
				conn.Open();

				MySqlCommand cmd = new MySqlCommand(String.Format(@"
                INSERT INTO `myapi`.`video` (`name`, `user_id`, `source`, `created_at`, `duration`) 
                VALUES ('{0}', {1}, '{2}', NOW(), {3})", sourceName, id, "/video/" + video.FileName, 10), conn);

				int nbrRowAffected = cmd.ExecuteNonQuery();

				if (nbrRowAffected > 0)
				{

					string hash = "";
					string source = video.FileName.Substring(63);
					source = source.Substring(0, 10);
					using (SHA1 sha1Hash = SHA1.Create())
					{
						//From String to byte array
						byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
						byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
						hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

						Console.WriteLine("The SHA1 hash of " + source + " is: " + hash);
					}

					string testSave = Path.Combine("wwwroot", "video", hash);
					            
					// Directory.CreateDirectory(testSave);

					string filePath = Path.Combine(testSave, video.FileName.Substring(63)); // 63 is lenght of full path form encoding container 
					using (Stream fileStream = new FileStream(filePath, FileMode.Create))
					{
						await video.CopyToAsync(fileStream);
					}


					cmd = new MySqlCommand("SELECT vid.id, vid.source, vid.created_at, vid.`view`, vid.enabled, USER.id AS user_id , USER.Username, USER.Pseudo, USER.created_at, USER.email FROM video vid LEFT JOIN USER ON vid.user_id = USER.id ORDER BY ID DESC LIMIT 1", conn);

					MySqlDataReader dr = cmd.ExecuteReader();

					VideoOutput vid = new VideoOutput();

					while (dr.Read())
					{
						vid.Id = dr.GetString(0);
						vid.Source = dr.GetString(1);
						vid.Created_at = dr.GetDateTime(2);
						vid.Views = dr.GetInt32(3);
						vid.Enabled = dr.GetBoolean(4);
						vid.User = new Model.User.UserOutput
						{
							Id = dr.GetInt32(5),
							Username = dr.GetString(6),
							Pseudo = dr.GetString(7),
							Created_at = dr.GetDateTime(8),
							Email = dr.GetString(9)
						};
					}

					this.CloseDbConnection(conn);

					return vid;
				}
				else
				{
					throw new Exception("Impossible de sauvegarder les données dans la base");
				}
			}
			else
			{
				throw new Exception("Le fichier est vide ou n'existe pas.");
			}
		}


		public int CountPagerOfVideo(string name, int user)
		{
			MySqlConnection conn = this.OpenDbConnection();
			conn.Open();

			string haveName = (!string.IsNullOrEmpty(name)) ? " AND name LIKE '%" + name + "%'" : "";
			string haveUser = (!string.IsNullOrEmpty(name)) ? " AND user_id = " + user : "";

			string query = string.Format("SELECT COUNT(*) FROM video WHERE 1=1 {0} {1}", haveName, haveUser);

			MySqlCommand cmd = new MySqlCommand(query, conn);

			MySqlDataReader dr = cmd.ExecuteReader();

			int result = 0;

			while (dr.Read())
			{
				result = dr.GetInt32(0);
			}

			return result;
		}
		public List<VideoOutput> GetVideo(string name, int user, int duration, int page, int perPage)
		{

			MySqlConnection conn = this.OpenDbConnection();

			conn.Open();

			string conditionQuery = GetVideoQuery(name, user, duration, page, perPage);
			string query = "";

			if (page > 1)
			{
				int currentPage = (page - 1) * perPage;
				query = string.Format(@"SELECT vid.id, vid.source, vid.created_at, vid.`view`, vid.enabled, USER.id AS user_id , USER.Username, USER.Pseudo, USER.created_at, USER.email 
                                            FROM video vid 
                                            LEFT JOIN USER ON vid.user_id = USER.id   
                                            {0}", conditionQuery);
			}
			else
			{
				query = string.Format(@"SELECT vid.id, vid.source, vid.created_at, vid.`view`, vid.enabled, USER.id AS user_id , USER.Username, USER.Pseudo, USER.created_at, USER.email 
                                            FROM video vid 
                                            LEFT JOIN USER ON vid.user_id = USER.id   
                                            {0}", conditionQuery);
			}

			MySqlCommand cmd = new MySqlCommand(query, conn);

			MySqlDataReader dr = cmd.ExecuteReader();

			List<VideoOutput> listVideo = new List<VideoOutput>();

			while (dr.Read())
			{
				VideoOutput vid = new VideoOutput
				{
					Id = dr.GetString(0),
					Source = dr.GetString(1),
					Created_at = dr.GetDateTime(2),
					Views = dr.GetInt32(3),
					Enabled = dr.GetBoolean(4),
					User = new Model.User.UserOutput
					{
						Id = dr.GetInt32(5),
						Username = dr.GetString(6),
						Pseudo = dr.GetString(7),
						Created_at = dr.GetDateTime(8),
						Email = dr.GetString(9)
					}
				};
				listVideo.Add(vid);
			}


			return listVideo;
		}

		public VideoOutput UpdateVideo(int id, string name, int user)
		{
			MySqlConnection conn = this.OpenDbConnection();

			conn.Open();

			if (CheckIfUserIdExist(user))
			{
				string query = string.Format(@"UPDATE video SET name = '{0}', user_id = {1} WHERE id = {2}", name, user, id);

				MySqlCommand cmd = new MySqlCommand(query, conn);

				int result = cmd.ExecuteNonQuery();

				if (result > 0)
				{
					query = @"SELECT vid.id, vid.source, vid.created_at, vid.`view`, vid.enabled, USER.id AS user_id , USER.Username, USER.Pseudo, USER.created_at, USER.email 
                                FROM video vid 
                                LEFT JOIN USER 
                                ON vid.user_id = USER.id
                                WHERE vid.id = " + id;

					cmd = new MySqlCommand(query, conn);

					MySqlDataReader dr = cmd.ExecuteReader();

					VideoOutput vid = new VideoOutput();

					while (dr.Read())
					{
						vid.Id = dr.GetString(0);
						vid.Source = dr.GetString(1);
						vid.Created_at = dr.GetDateTime(2);
						vid.Views = dr.GetInt32(3);
						vid.Enabled = dr.GetBoolean(4);
						vid.User = new Model.User.UserOutput
						{
							Id = dr.GetInt32(5),
							Username = dr.GetString(6),
							Pseudo = dr.GetString(7),
							Created_at = dr.GetDateTime(8),
							Email = dr.GetString(9)
						};
					}

					this.CloseDbConnection(conn);
					return vid;

				}
				else
				{
					throw new ArgumentException("Not Found");
				}
			}
			else
			{
				throw new ArgumentException("User Not Found");
			}
		}

		public bool DeleteVideo(int id)
		{
			MySqlConnection conn = this.OpenDbConnection();

			conn.Open();

			MySqlCommand cmd = new MySqlCommand("SELECT source FROM video WHERE id = " + id, conn);
			MySqlDataReader dr = cmd.ExecuteReader();


			string fileSource = "";
			while (dr.Read())
			{
				fileSource = dr.GetString(0);
			}

			string file = _rootPath.Replace('\\', '/') + fileSource;
			if (File.Exists(file))
			{
				File.Delete(_rootPath.Replace('\\', '/') + fileSource);
			}
			dr.Close();

			cmd = new MySqlCommand("DELETE FROM video WHERE id = " + id, conn);
			int result = cmd.ExecuteNonQuery();

			if (result > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
