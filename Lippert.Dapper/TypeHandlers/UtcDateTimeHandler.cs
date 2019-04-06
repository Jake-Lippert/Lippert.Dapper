using System;
using System.Data;
using Dapper;
using Lippert.Core.Extensions;

namespace Lippert.Dapper.TypeHandlers
{
	public class UtcDateTimeHandler : SqlMapper.TypeHandler<DateTime>
	{
		public override void SetValue(IDbDataParameter parameter, DateTime value) =>
			parameter.Value = value.SpecifyKind(DateTimeKind.Utc);

		public override DateTime Parse(object value) => ((DateTime)value).SpecifyKind(DateTimeKind.Utc);
	}
}