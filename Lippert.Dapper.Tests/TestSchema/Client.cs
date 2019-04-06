using System;
using Lippert.Dapper.Tests.TestSchema.Contracts;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class Client : IGuidIdentifier, ICreateEditFields
	{
		Guid IGuidIdentifier.Id { get; set; }

		public Guid CreatedByUserId { get; set; }
		public DateTime CreatedDateUtc { get; set; }
		public Guid ModifiedByUserId { get; set; }
		public DateTime ModifiedDateUtc { get; set; }

		public string Name { get; set; }
		public bool IsActive { get; set; }
	}
}