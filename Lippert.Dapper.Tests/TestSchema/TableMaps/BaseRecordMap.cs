using Lippert.Core.Data;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps
{
	public class BaseRecordMap : TableMap<BaseRecord>
    {
		public BaseRecordMap()
		{
			AutoMap();
		}
    }
}