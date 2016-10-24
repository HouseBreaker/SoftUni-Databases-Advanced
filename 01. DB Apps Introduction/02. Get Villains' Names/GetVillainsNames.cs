using System;
using System.Data.SqlClient;
using _01.DB_Apps_Introduction;

namespace _02.Get_Villains__Names
{
	public static class GetVillainsNames
	{
		static void Main(string[] args)
		{
			var connection = DbConfig.Connection;

			using (connection)
			{
				connection.Open();

				var command = new SqlCommand(
					@"SELECT v.Name, COUNT(MinionId) AS c
					FROM Villains v
					JOIN MinionsVillains mv ON v.Id = mv.VillainId
					GROUP BY v.Name
					HAVING COUNT(MinionId) > 1
					ORDER BY c DESC", connection);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Console.WriteLine($"{reader[0]} {reader[1]}");
					}
				}

				connection.Close();
			}
		}
	}
}