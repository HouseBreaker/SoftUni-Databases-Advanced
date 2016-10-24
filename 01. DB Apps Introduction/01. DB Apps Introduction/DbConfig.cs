using System.Data.SqlClient;

namespace _01.DB_Apps_Introduction
{
	public static class DbConfig
	{
		public static readonly SqlConnection Connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MinionsDB;Integrated Security=True");
	}
}
