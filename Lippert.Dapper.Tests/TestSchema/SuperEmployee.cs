using System;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class SuperEmployee : Employee
	{
		public Guid EmployeeId { get; set; }
		public string SomeAwesomeField { get; set; } = Guid.NewGuid().ToString();
	}
}