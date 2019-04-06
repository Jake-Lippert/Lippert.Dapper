using Lippert.Core.Data;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps
{
	public class EmployeeMap : TableMap<Employee>
	{
		public EmployeeMap()
		{
			Map(x => x.UserId).Key(false);
			AutoMap();
		}
	}
}