using System;
using System.Data;
using Dapper;
using Lippert.Core.Extensions;

namespace Lippert.Dapper.TypeHandlers
{
	/// <summary>
	/// Specifies DateTimes as UTC when passing as SQL parameter or parsing results
	/// </summary>
	public class UtcDateTimeHandler : SqlMapper.TypeHandler<DateTime>
	{
		/// <summary>
		/// Specifies DateTime parameter value as UTC
		/// </summary>
		public override void SetValue(IDbDataParameter parameter, DateTime value) =>
			parameter.Value = value.SpecifyKind(DateTimeKind.Utc);

		/// <summary>
		/// Specifies parsed DateTime value as UTC
		/// </summary>
		public override DateTime Parse(object value) => ((DateTime)value).SpecifyKind(DateTimeKind.Utc);
	}
}