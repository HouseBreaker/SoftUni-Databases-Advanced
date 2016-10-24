using System;
using System.Data;
using System.Data.SqlClient;
using _01.DB_Apps_Introduction;

namespace _06.Remove_Villain
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var villainId = int.Parse(Console.ReadLine());

			using (var connection = DbConfig.Connection)
			{
				connection.Open();
				try
				{
					var villainNameCommand = new SqlCommand("SELECT Name FROM Villains WHERE Id = @villainId", connection);
					villainNameCommand.Parameters.AddWithValue("@villainId", villainId);
					var villainName = villainNameCommand.ExecuteScalar();

					if (villainName == null)
					{
						throw new ArgumentException("No such villain was found");
					}

					var transaction = connection.BeginTransaction();

					var deleteMinionsCommand = new SqlCommand
					{
						CommandText = @"DELETE FROM MinionsVillains
										WHERE VillainId = @villainId",
						Connection = connection,
						Transaction = transaction
					};
					deleteMinionsCommand.Parameters.AddWithValue("@villainId", villainId);

					var releasedMinionCount = deleteMinionsCommand.ExecuteNonQuery();

					var deleteVillainCommand = new SqlCommand
					{
						CommandText = @"DELETE FROM Villains
										WHERE Id = @villainId",
						Connection = connection,
						Transaction = transaction
					};
					deleteVillainCommand.Parameters.AddWithValue("@villainId", villainId);

					deleteVillainCommand.ExecuteNonQuery();

					Console.WriteLine($"{villainName} was deleted");
					Console.WriteLine("{0} minion{1} released",
						releasedMinionCount,
						releasedMinionCount != 1 ? "s" : string.Empty);
#if DEBUG
					transaction.Rollback();
#else
					transaction.Commit();
#endif
				}
				catch (ArgumentException ex)
				{
					Console.WriteLine(ex.Message);
				}
				catch (SqlException ex)
				{
					Console.WriteLine($"{ex.Message}");
				}
			}
		}
	}
}