using System;
using Lippert.Dapper.Tests.TestSchema.Contracts;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class InheritingComponent : BaseRecord, IGuidIdentifier
	{
		public new Guid Id { get; set; }
		public Guid BaseId
		{
			get => base.Id;
			set => base.Id = value;
		}
		public string Category { get; set; } = string.Empty;
		public decimal Cost { get; set; }
	}
}