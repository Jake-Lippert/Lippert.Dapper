using System;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class Employee : User, Contracts.ICreateFields
	{
		public Guid UserId { get; set; }
		public Guid CompanyId { get; set; }
		public Guid CreatedByUserId { get; set; }
		public DateTime CreatedDateUtc { get; set; }
	}
}