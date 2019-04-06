using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Lippert.Dapper
{
	public class DapperWrapper : Contracts.IDapperWrapper
	{
		public int Execute(IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
			conn.Execute(sql, param, transaction, commandTimeout, commandType);
		public IEnumerable<T> Query<T>(IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) =>
			conn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
	}
}