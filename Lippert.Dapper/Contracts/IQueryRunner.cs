using System.Collections.Generic;
using System.Data;
using Lippert.Core.Data.QueryBuilders;

namespace Lippert.Dapper.Contracts
{
	public interface IQueryRunner
	{
		T Select<T>(IDbConnection connection, dynamic key, bool buffered = true, int? commandTimeout = null);
		T Select<T>(IDbTransaction transaction, dynamic key, bool buffered = true, int? commandTimeout = null);
		IEnumerable<T> Select<T>(IDbConnection connection, bool buffered = true, int? commandTimeout = null);
		IEnumerable<T> Select<T>(IDbTransaction transaction, bool buffered = true, int? commandTimeout = null);
		IEnumerable<T> Select<T>(IDbConnection connection, PredicateBuilder<T> selectBuilder, bool buffered = true, int? commandTimeout = null);
		IEnumerable<T> Select<T>(IDbTransaction transaction, PredicateBuilder<T> selectBuilder, bool buffered = true, int? commandTimeout = null);

		void Insert<T>(IDbConnection connection, T record, int? commandTimeout = null);
		void Insert<T>(IDbTransaction transaction, T record, int? commandTimeout = null);

		int Update<T>(IDbConnection connection, T record, int? commandTimeout = null);
		int Update<T>(IDbTransaction transaction, T record, int? commandTimeout = null);
		int Update<T>(IDbConnection connection, UpdateBuilder<T> updateBuilder, int? commandTimeout = null);
		int Update<T>(IDbTransaction transaction, UpdateBuilder<T> updateBuilder, int? commandTimeout = null);

		int Delete<T>(IDbConnection connection, dynamic key, int? commandTimeout = null);
		int Delete<T>(IDbTransaction transaction, dynamic key, int? commandTimeout = null);
		int Delete<T>(IDbConnection connection, PredicateBuilder<T> predicateBuilder, int? commandTimeout = null);
		int Delete<T>(IDbTransaction transaction, PredicateBuilder<T> predicateBuilder, int? commandTimeout = null);
	}
}