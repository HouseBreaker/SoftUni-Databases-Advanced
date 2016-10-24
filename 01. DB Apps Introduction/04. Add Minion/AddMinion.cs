using System;
using System.Data.SqlClient;
using _01.DB_Apps_Introduction;

namespace _04.Add_Minion
{
	public static class AddMinion
	{
		public static void Main(string[] args)
		{
			var connection = DbConfig.Connection;

			var minionTokens = Console.ReadLine().Split();
			var villainTokens = Console.ReadLine().Split();

			var minionName = minionTokens[1];
			var minionAge = int.Parse(minionTokens[2]);
			var minionTown = minionTokens[3];

			var villainName = villainTokens[1];

			using (connection)
			{
				connection.Open();

				var addTownAffectedRows = AddTownIfNonexistent(connection, minionTown);
				switch (addTownAffectedRows)
				{
					case 1:
						Console.WriteLine($"Town {minionTown} was added to the database.");
						break;
					case 0:
						Console.WriteLine("Error on adding town!");
						break;
				}

				var addVillainAffectedRows = AddVillainIfNonexistant(connection, villainName);
				switch (addVillainAffectedRows)
				{
					case 1:
						Console.WriteLine($"Villain {villainName} was added to the database.");
						break;
					case 0:
						Console.WriteLine("Error on adding villain!");
						break;
				}

				var addMinionAffectedRows = AddMinionIfNonexistant(connection, minionName, minionAge, minionTown);
				switch (addMinionAffectedRows)
				{
					case 1:
						Console.WriteLine($"Minion {minionName} was added to the database.");
						break;
					case 0:
						Console.WriteLine("Error on adding minion!");
						break;
				}

				var assignMinionAffectedRows = AssignMinionToVillain(connection, minionName, villainName);
				if (assignMinionAffectedRows == 1)
				{
					Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
				}
				else
				{
					Console.WriteLine($"Error adding minion ");
				}
				connection.Close();
			}
		}

		private static int AssignMinionToVillain(SqlConnection connection, string minionName, string villainName)
		{
			var minionIdCommand = new SqlCommand(@"SELECT Id FROM Minions WHERE Name = @minionName", connection);
			minionIdCommand.Parameters.AddWithValue("@minionName", minionName);
			var minionId = (int)minionIdCommand.ExecuteScalar();

			var villainIdCommand = new SqlCommand(@"SELECT Id FROM Villains WHERE Name = @villainName", connection);
			villainIdCommand.Parameters.AddWithValue("@villainName", villainName);
			var villainId = (int)villainIdCommand.ExecuteScalar();

			var assignMinionToVillainCommand = new SqlCommand(@"INSERT INTO MinionsVillains VALUES (@minionId, @villainId)", connection);
			assignMinionToVillainCommand.Parameters.AddWithValue("@minionId", minionId);
			assignMinionToVillainCommand.Parameters.AddWithValue("@villainId", villainId);
			var affectedRows = assignMinionToVillainCommand.ExecuteNonQuery();

			return affectedRows;
		}

		private static int AddMinionIfNonexistant(SqlConnection connection, string minionName, int minionAge,
			string minionTown)
		{
			var villainExistsCommand = new SqlCommand(@"SELECT Id FROM Minions WHERE Name = @minionName", connection);
			villainExistsCommand.Parameters.AddWithValue("@minionName", minionName);

			var resultRows = villainExistsCommand.ExecuteScalar();
			if (resultRows == null)
			{
				var minionTownIdCommand = new SqlCommand(@"SELECT Id FROM Towns WHERE Name = @minionTown", connection);
				minionTownIdCommand.Parameters.AddWithValue("@minionTown", minionTown);
				var minionTownId = minionTownIdCommand.ExecuteScalar();

				var insertMinion = new SqlCommand("INSERT INTO Minions VALUES (@minionName, @minionAge, @minionTownId)",
					connection);
				insertMinion.Parameters.AddWithValue("@minionName", minionName);
				insertMinion.Parameters.AddWithValue("@minionAge", minionAge);
				insertMinion.Parameters.AddWithValue("@minionTownId", minionTownId);

				var affectedRows = insertMinion.ExecuteNonQuery();

				return affectedRows;
			}

			return -1;
		}

		private static int AddVillainIfNonexistant(SqlConnection connection, string villainName)
		{
			var villainExistsCommand = new SqlCommand(@"SELECT Id FROM Villains WHERE Name = @villainName", connection);
			villainExistsCommand.Parameters.AddWithValue("@villainName", villainName);

			var villainExistsResult = villainExistsCommand.ExecuteScalar();
			if (villainExistsResult == null)
			{
				var insertTownCommand = new SqlCommand("INSERT INTO Villains VALUES (@villainName, 'evil')", connection);
				insertTownCommand.Parameters.AddWithValue("@villainName", villainName);
				var affectedRows = insertTownCommand.ExecuteNonQuery();

				return affectedRows;
			}

			return -1;
		}

		private static int AddTownIfNonexistent(SqlConnection connection, string minionTown)
		{
			var townExistsCommand = new SqlCommand(@"SELECT Id FROM Towns WHERE Name = @townName", connection);
			townExistsCommand.Parameters.AddWithValue("@townName", minionTown);

			if (townExistsCommand.ExecuteScalar() == null)
			{
				var insertTownCommand = new SqlCommand("INSERT INTO Towns VALUES (@townName, NULL)", connection);
				insertTownCommand.Parameters.AddWithValue("@townName", minionTown);
				var affectedRows = insertTownCommand.ExecuteNonQuery();

				return affectedRows;
			}

			return -1;
		}
	}
}