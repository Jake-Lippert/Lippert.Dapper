using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Dapper.Tests.TestSchema.Contracts;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps.Builders
{
	public class GuidIdentifierMapBuilder : TableMapBuilder<IGuidIdentifier>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap) => tableMap.Map(x => x.Id).Key();
	}
}