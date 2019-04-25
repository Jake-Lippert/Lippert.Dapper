using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Lippert.Core.Collections.Extensions;

namespace Lippert.Dapper.TypeHandlers
{
	/// <summary>
	/// Converts enumerable collections of <typeparamref name="T"/> into table-valued parameters
	/// </summary>
	public class TableValuedParameterHandler<T> : SqlMapper.TypeHandler<IEnumerable<T>>
	{
		/// <summary>
		/// Converts an enumerable collection of <typeparamref name="T"/> into a table-valued parameter
		/// </summary>
		public override void SetValue(IDbDataParameter parameter, IEnumerable<T> value) =>
			parameter.Value = value.ToDataTable();

		public override IEnumerable<T> Parse(object value) => throw new InvalidOperationException();
	}
}