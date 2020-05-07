using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Dapper.Tests.TestSchema.Contracts;
using Lippert.Dapper.Tests.TestSchema.TableMaps.ColumnValueProviders;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps.Builders
{
	public class CreateFieldsMapBuilder : TableMapBuilder<ICreateFields>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap)
		{
			tableMap.Map(x => x.CreatedByUserId).Ignore(SqlOperation.Update);
			tableMap.Map(x => x.CreatedDateUtc).Generated();
		}


		public override void SetInsertValues(ICreateFields record)
		{
			record.CreatedByUserId = ClaimsProvider.UserClaims.UserId;
		}
	}
}