using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _03.Introduction_to_Entity_Framework
{
	using _03.Introduction_to_Entity_Framework.SoftUniContext;

	static class Exercises
	{
		[STAThread]
		private static void Main()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
			var softUniContext = new SoftUniContext.SoftUniContext();
			var gringottsContext = new GringottsContext.GringottsContext();
			var result = P19_FirstLetter(gringottsContext);

			Console.WriteLine(result);
			Clipboard.SetText(result);
		}

		private static string P19_FirstLetter(GringottsContext.GringottsContext context)
		{
			var result = new StringBuilder();

			var wizardLetters = context.WizzardDeposits
				.Where(a => a.DepositGroup == "Troll Chest")
				.Select(a => a.FirstName.Substring(0, 1))
				.Distinct()
				.OrderBy(a => a)
				.ToArray();

			result.Append(string.Join(Environment.NewLine, wizardLetters));

			return result.ToString();
		}

		private static string P18_FindEmployeesByFirstNameStartingWithSa(SoftUniContext.SoftUniContext context)
		{
			var result = new StringBuilder();

			var employees = context.Employees
				.Where(e => e.FirstName.Substring(0, 2).Equals("SA", StringComparison.OrdinalIgnoreCase))
				.ToArray();

			foreach (var employee in employees)
			{
				result.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:F4})");
			}

			return result.ToString();
		}

		private static string P16_IncreaseSalaries(SoftUniContext.SoftUniContext context)
		{
			var result = new StringBuilder();

			var correctDepartments = new[] {"Engineering", "Tool Design", "Marketing", "Information Services"};

			var employees = context.Employees.Where(e => correctDepartments.Contains(e.Department.Name));

			foreach (var employee in employees)
			{
				employee.Salary *= 1.12m;
				result.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary})");
			}

			context.SaveChanges();

			return result.ToString();
		}

		private static string P15_FindLatest10Projects(SoftUniContext.SoftUniContext context)
		{
			var result = new StringBuilder();

			var projects = context.Projects
				.OrderByDescending(a => a.StartDate)
				.Take(10)
				.OrderBy(a => a.Name)
				.ToArray();

			foreach (var project in projects)
			{
				result.AppendLine($"{project.Name} {project.Description} {project.StartDate} {project.EndDate}");
			}

			return result.ToString();
		}

		private static string P11_DepartmentsWithMoreThan5Employees(SoftUniContext.SoftUniContext context)
		{
			var result = new StringBuilder();

			var departments = context.Departments.Where(a => a.Employees.Count > 5)
				.OrderBy(a => a.Employees.Count).ToArray();

			foreach (var department in departments)
			{
				result.AppendLine($"{department.Name} {department.Manager.FirstName}");
				foreach (var employee in department.Employees)
				{
					result.AppendLine($"{employee.FirstName} {employee.LastName} {employee.JobTitle}");
				}
			}

			return result.ToString();
		}

		private static string P10_EmployeeWithId147(SoftUniContext.SoftUniContext context)
		{
			var result = new StringBuilder();

			var employee = context.Employees.Find(147);

			result.AppendLine($"{employee.FirstName} {employee.LastName} {employee.JobTitle}");
			foreach (var project in employee.Projects.OrderBy(a => a.Name))
			{
				result.AppendLine($"{project.Name}");
			}
			return result.ToString();
		}

		private static string P09_AddressesByTownName(SoftUniContext.SoftUniContext context)
		{
			var result = new StringBuilder();

			var addresses = context.Addresses
				.OrderByDescending(a => a.Employees.Count)
				.ThenBy(a => a.Town.Name).Take(10)
				.ToArray();

			foreach (var address in addresses)
			{
				result.AppendLine($"{address.AddressText}, {address.Town.Name} - {address.Employees.Count} employees");
			}

			return result.ToString();
		}

		private static string P08_FindEmployeesInPeriod(SoftUniContext.SoftUniContext context)
		{
			var employees =
				context.Employees
					.Where(a => a.Projects.Any(p => p.StartDate.Year >= 2001 && p.StartDate.Year <= 2003))
					.Take(30)
					.ToArray();

			var result = new StringBuilder();
			foreach (var employee in employees)
			{
				result.AppendLine($"{employee.FirstName} {employee.LastName} {employee.Manager.FirstName}");
				foreach (var project in employee.Projects)
				{
					result.AppendLine($"--{project.Name} {project.StartDate} {project.EndDate}");
				}
			}

			return result.ToString();
		}

		private static string P07_DeleteProjectById(SoftUniContext.SoftUniContext context)
		{
			var project = context.Projects.Find(2);

			foreach (var employee in project.Employees)
			{
				employee.Projects.Remove(project);
			}

			context.Projects.Remove(project);
			context.SaveChanges();

			var projects = context.Projects.Take(10).Select(p => p.Name).ToArray();
			var result = string.Join(Environment.NewLine, projects);
			return result;
		}

		private static string P06_AddNewAddressAndUpdateEmployee(SoftUniContext.SoftUniContext context)
		{
			var newAddress = new Address()
			{
				AddressText = "Vitoshka 15",
				TownID = 4
			};

			var nakov = context.Employees.FirstOrDefault(a => a.LastName == "Nakov");
			nakov.Address = newAddress;

			context.SaveChanges();

			var employees = context.Employees.OrderByDescending(a => a.AddressID).Take(10).Select(a => a.Address.AddressText);
			var result = string.Join(Environment.NewLine, employees);
			return result;
		}

		private static string P05_EmployeesFromSeattle(SoftUniContext.SoftUniContext context)
		{
			var employees = context.Employees
				.Where(e => e.Department.Name == "Research and Development").OrderBy(e => e.Salary)
				.ThenByDescending(e => e.FirstName)
				.Select(e => new {e.FirstName, e.LastName, e.Department.Name, e.Salary,})
				.ToArray();

			var result = new StringBuilder();
			foreach (var employee in employees)
			{
				result.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:F2}");
			}

			return result.ToString();
		}

		private static string P04_EmployeesWithSalaryOver50000(SoftUniContext.SoftUniContext context)
		{
			var employeesWithHighSalary = context.Employees.Where(e => e.Salary > 50000).Select(e => e.FirstName).ToArray();

			var result = string.Join(Environment.NewLine, employeesWithHighSalary);
			return result;
		}

		private static string P03_GetEmployeesFullInformation(SoftUniContext.SoftUniContext context)
		{
			var employees = context.Employees.ToArray();

			var employeesOutput = new StringBuilder();
			foreach (var employee in employees)
			{
				employeesOutput.AppendLine(
					$"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary}");
			}

			return employeesOutput.ToString();
		}
	}
}