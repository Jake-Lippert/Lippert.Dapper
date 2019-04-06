using System;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class User : Contracts.IGuidIdentifier
	{
		public Guid Id { get; set; }
	}
}