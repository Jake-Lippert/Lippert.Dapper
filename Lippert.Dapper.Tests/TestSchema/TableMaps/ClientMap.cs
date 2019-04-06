using Lippert.Core.Data;

namespace Lippert.Dapper.Tests.TestSchema.TableMaps
{
	public class ClientMap : TableMap<Client>
	{
		public ClientMap()
		{
			AutoMap();
		}
	}
}