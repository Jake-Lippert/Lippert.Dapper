using System.Collections.Generic;
using System.Data;

namespace Lippert.Dapper.Contracts
{
	/// <summary>
	/// Provides access to Dapper in a testable manner
	/// </summary>
	public interface IDapperWrapper
	{
		/// <summary>
		/// Execute parameterized SQL
		/// </summary>
		/// <returns>Number of rows affected</returns>
		int Execute(IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per T
		/// </summary>
		/// <returns>A sequence of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		IEnumerable<T> Query<T>(IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
	}
}