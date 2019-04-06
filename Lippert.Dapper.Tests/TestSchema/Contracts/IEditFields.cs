using System;

namespace Lippert.Dapper.Tests.TestSchema.Contracts
{
	public interface IEditFields
	{
		Guid ModifiedByUserId { get; set; }
		DateTime ModifiedDateUtc { get; set; }
	}
}