using System;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps.ColumnValueProviders
{
	public static class ClaimsProvider
	{
		public static Claims UserClaims { get; } = new Claims();

		public class Claims
		{
			public Guid UserId { get; set; }
		}
	}
}