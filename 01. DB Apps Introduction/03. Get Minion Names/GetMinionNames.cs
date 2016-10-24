using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01.DB_Apps_Introduction;

namespace _03.Get_Minion_Names
{
	public static class GetMinionNames
	{
		static void Main(string[] args)
		{
			var connection = DbConfig.Connection;
			var minionId = int.Parse(Console.ReadLine());

			using (connection)
			{
				connection.Open();

				var getVillainNameCommand = new SqlCommand(
					$@"SELECT Name FROM Villains
					WHERE Id = {minionId}", connection);

				var villainNameResult = getVillainNameCommand.ExecuteScalar();

				if (villainNameResult == null)
				{
					Console.WriteLine($"No villain with ID {minionId} exists in the database.");
					return;
				}
				var villainName = villainNameResult.ToString();
				Console.WriteLine($"Villain: {villainName}");

				var getMinionsByVillain = new SqlCommand(
				$@"SELECT Name, Age FROM MinionsVillains mv
				JOIN Minions m ON m.Id = mv.MinionId
				WHERE VillainId = {minionId}", connection);

				var minionsList = new List<string[]>();

				using (var reader = getMinionsByVillain.ExecuteReader())
				{
					while (reader.Read())
					{
						minionsList.Add(new [] {reader[0].ToString(), reader[1].ToString()});
					}
				}

				for (int i = 0; i < minionsList.Count; i++)
				{
					Console.WriteLine($"{i+1}.{minionsList[i][0]} {minionsList[i][1]}");
				}

				connection.Close();
			}
		}
	}
}