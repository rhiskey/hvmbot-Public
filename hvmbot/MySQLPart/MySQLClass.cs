using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Abstractions.Authorization;

namespace vkMCBot.Mysql
{
    class MySQLClass
    {
		public static bool InsertVKID(long? vkid, string genres)
		{
			bool isSuccess;
			try
			{
				MySqlCommand command = new MySqlCommand(); ;
				MySqlDataAdapter adapter = new MySqlDataAdapter();
				string sql = "";
				MySqlConnection connection = DBUtills.GetDBConnection();
				command.Connection = connection;
				command.Connection.Open();
				sql = "INSERT INTO Parser.SubsBot (UserId, Genres) VALUES ('"+vkid+"', '"+genres+"');";
				command = new MySqlCommand(sql, connection);

				adapter.InsertCommand = new MySqlCommand(sql, connection);
				adapter.InsertCommand.ExecuteNonQuery();

				Console.WriteLine(String.Format("Inserted in DB VKID: {0}, Genres: {1}",vkid, genres));
				command.Dispose();
				connection.Close();

				isSuccess = true;
			}
			catch (Exception e) { Console.WriteLine(e); isSuccess = false; }
			return isSuccess;
		}

		public static List<string> GetUserSubscribtion(long? vkid)
		{
			List<string> genres = new List<string>();


			return genres; 
		}

		public static bool SetUserSubscription(long? vkid, string genre)
		{
			bool isSuccess = false;


			return isSuccess;
		}

		public static bool RemoveVKID(long? vkid)
		{
			bool isSuccess;
			try
			{
				MySqlCommand command = new MySqlCommand(); ;
				MySqlDataAdapter adapter = new MySqlDataAdapter();
				string sql = "";
				MySqlConnection connection = DBUtills.GetDBConnection();
				command.Connection = connection;
				command.Connection.Open();

				sql ="UPDATE authme.authme SET vkid= NULL WHERE  vkid=\'" + vkid + "\';";
				command = new MySqlCommand(sql, connection);

				adapter.InsertCommand = new MySqlCommand(sql, connection);
				adapter.InsertCommand.ExecuteNonQuery();
				Console.WriteLine(String.Format("Removed FROM DB VKID: {0}", vkid));
				command.Dispose();
				connection.Close();

				isSuccess = true;
			}
			catch (Exception e) { Console.WriteLine(e); isSuccess = false; }
			return isSuccess;
		}

		public static List<long?> GetVKID()
		{
			List<long?> idList = new List<long?>();
			try
			{
				MySqlCommand command = new MySqlCommand(); ;
				string sql = "";
				MySqlConnection connection = DBUtills.GetDBConnection();
				command.Connection = connection;
				command.Connection.Open();
				sql = "SELECT * FROM authme.authme ORDER BY vkid ASC LIMIT 1000;";
				command = new MySqlCommand(sql, connection);
				MySqlDataReader reader = command.ExecuteReader();

				while (reader.Read())  //foreach vkID from SQL
				{
					long? vkID=0;
	
					var vkIDObject = reader.GetValue(19);
					if (!(vkIDObject is DBNull))
						vkID = Convert.ToInt64(vkIDObject);
	
					if (vkID != 0)
						idList.Add(vkID);
				}
				reader.Close();

				command.Dispose();
				connection.Close();
			}
			catch (Exception e) { Console.WriteLine(e); }

			return idList;

		}

		public static List<string> GetNicknames()
		{
			List<string> nickList = new List<string>();
			try
			{
				MySqlCommand command = new MySqlCommand(); ;
				string sql = "";
				MySqlConnection connection = DBUtills.GetDBConnection();
				command.Connection = connection;
				command.Connection.Open();
				
				sql = "SELECT * FROM authme.authme ORDER BY username DESC LIMIT 1000;";
				command = new MySqlCommand(sql, connection);
				MySqlDataReader reader = command.ExecuteReader();

				while (reader.Read())  //foreach vkID from SQL
				{
					var nick = reader.GetValue(1).ToString(); 
					var isLogged = Convert.ToInt32(reader.GetValue(15));

					if (nick != null /*&& isLogged==1*/)
						nickList.Add(nick);
				}
				reader.Close();

				command.Dispose();
				connection.Close();
			}
			catch (Exception e) { Console.WriteLine(e); }

			return nickList;

		}

		public static bool IsVKLinked(long? vkIDtoCheck)
		{
			bool isLinked = false;
			try
			{
				MySqlCommand command = new MySqlCommand(); ;
				string sql = "";
				MySqlConnection connection = DBUtills.GetDBConnection();
				command.Connection = connection;
				command.Connection.Open();

				sql = "SELECT * FROM authme.authme ORDER BY vkid ASC LIMIT 1000;";
				command = new MySqlCommand(sql, connection);
				MySqlDataReader reader = command.ExecuteReader();

				while (reader.Read())  //foreach vkID from SQL
				{
					long? vkID = 0;
					
					var vkIDObject = reader.GetValue(19); //"vkid"
					if (!(vkIDObject is DBNull))
					{
						//Ищем совпадение. Если в столбце vkid есть совпадение с vkIDtoCheck то акк привязан
						vkID = Convert.ToInt64(vkIDObject);
						if (vkIDtoCheck == vkID)
							isLinked = true;
						else
							isLinked = false;
					}

	
				}
				reader.Close();
				//command.Connection.Close();
				command.Dispose();
				connection.Close();
			}
			catch (Exception e) { Console.WriteLine(e); }

			return isLinked;
		}
		
		public static string GetNickFromID(long? vkid)
		{
			string playerName = null;
			try
			{
				MySqlCommand command = new MySqlCommand(); ;
				string sql = "";
				MySqlConnection connection = DBUtills.GetDBConnection();
				command.Connection = connection;
				command.Connection.Open();

				sql = "SELECT username FROM authme.authme WHERE vkid = \'"+ vkid + "\'; ";
				command = new MySqlCommand(sql, connection);
				MySqlDataReader reader = command.ExecuteReader();

				while (reader.Read())  //foreach vkID from SQL
				{

					var username = reader.GetValue(0);
					if (!(username is DBNull))
						playerName = username.ToString();

				}
				reader.Close();

				command.Dispose();
				connection.Close();
			}
			catch (Exception e) { Console.WriteLine(e); playerName = null; }

			return playerName;
		}

	}
}
