using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using _01.DB_Apps_Introduction;

namespace _05.Change_Town_Names_Casing
{
	public static class ChangeTownNamesCasing
	{
		public static void Main(string[] args)
		{
			Console.Write("Enter a country: ");
			var country = Console.ReadLine();
			var connection = DbConfig.Connection;

			var transaction = connection.BeginTransaction();

			var changeTownNamesCommand = new SqlCommand
			{
				CommandText = @"UPDATE Towns
								SET Name = UPPER(Name)
								OUTPUT inserted.Name
								WHERE Country = @country AND Name != UPPER(Name)",
				Connection = connection,
				Transaction = transaction
			};
			changeTownNamesCommand.Parameters.AddWithValue("@country", country);

			var changedTownNames = new List<string>();
			connection.Open();


			using (var result = changeTownNamesCommand.ExecuteReader())
			{
				if (result.HasRows)
				{
					while (result.Read())
					{
						changedTownNames.Add(result[0].ToString());
					}
#if DEBUG
					transaction.Rollback();
#else
					transaction.Commit();
#endif
					Console.WriteLine($"{result.RecordsAffected} town names were affected.");
					Console.WriteLine($"[{string.Join(", ", changedTownNames)}]");
				}
				else
				{
					Console.WriteLine("No town names were affected.");
				}
			}
		}
	}
}