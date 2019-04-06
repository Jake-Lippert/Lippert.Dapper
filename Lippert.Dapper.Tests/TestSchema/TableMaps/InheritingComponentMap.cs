using Lippert.Core.Data;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps
{
	public class InheritingComponentMap : TableMap<InheritingComponent>
    {
		public InheritingComponentMap()
		{
			Map(x => x.BaseId).Ignore(SqlOperation.Update);
			AutoMap(x => x.Category, x => x.Cost);
		}
    }
}