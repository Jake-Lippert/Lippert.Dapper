using System;

namespace Lippert.Dapper.Tests.TestSchema.Contracts
{
	public interface ICreateFields
	{
		Guid CreatedByUserId { get; set; }
		DateTime CreatedDateUtc { get; set; }
	}
}