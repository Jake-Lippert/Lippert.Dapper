using System;
using System.Collections.Generic;
using System.Reflection;
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
		

		public override List<(PropertyInfo column, object? value)> GetInsertValues() => new List<(PropertyInfo column, object? value)>
		{
			SetValue(x => x.ModifiedByUserId, ClaimsProvider.UserClaims.UserId)
		};

		public override List<(PropertyInfo column, object? value)> GetUpdateValues() => new List<(PropertyInfo column, object? value)>
		{
			SetValue(x => x.ModifiedByUserId, ClaimsProvider.UserClaims.UserId),
			SetValue(x => x.ModifiedDateUtc, DateTime.UtcNow)
		};
	}
}