using System;

namespace Lippert.Dapper.Tests.TestSchema
{
	public class ClientUser
    {
		public Guid ClientId { get; set; }
		public Guid UserId { get; set; }
		public bool IsActive { get; set; }
		public string Role { get; set; } = string.Empty;
	}
}