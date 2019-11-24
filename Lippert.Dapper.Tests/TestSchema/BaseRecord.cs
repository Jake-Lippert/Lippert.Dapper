using System;
using Lippert.Dapper.Tests.TestSchema.Contracts;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class BaseRecord : IGuidIdentifier
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}