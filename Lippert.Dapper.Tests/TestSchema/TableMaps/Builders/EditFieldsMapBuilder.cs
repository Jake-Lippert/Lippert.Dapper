using System;
using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Dapper.Tests.TestSchema.Contracts;
using Lippert.Dapper.Tests.TestSchema.TableMaps.ColumnValueProviders;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps.Builders
{
	public class EditFieldsMapBuilder : TableMapBuilder<IEditFields>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap)
		{
			tableMap.Map(x => x.ModifiedByUserId);
			tableMap.Map(x => x.ModifiedDateUtc).Generated(allowUpdates: true);
		}
		

		public override void SetInsertValues(IEditFields record)
		{
			record.ModifiedByUserId = ClaimsProvider.UserClaims.UserId;
		}

		public override void SetUpdateValues(IEditFields record)
		{
			record.ModifiedByUserId = ClaimsProvider.UserClaims.UserId;
			record.ModifiedDateUtc = DateTime.UtcNow;
		}
	}
}