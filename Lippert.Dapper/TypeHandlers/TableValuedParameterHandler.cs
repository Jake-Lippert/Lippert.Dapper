using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Lippert.Core.Collections.Extensions;

namespace Lippert.Dapper.TypeHandlers
{
	public class TableValuedParameterHandler<T> : SqlMapper.TypeHandler<IEnumerable<T>>
	{
		public override void SetValue(IDbDataParameter parameter, IEnumerable<T> value) =>
			parameter.Value = value.ToDataTable();

		public override IEnumerable<T> Parse(object value) => throw new InvalidOperationException();
	}
}