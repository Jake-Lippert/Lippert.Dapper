using System;
using System.Collections.Generic;
using System.Data;
using Lippert.Core.Data.QueryBuilders;

namespace Lippert.Dapper.Contracts
{
	/// <summary>
	/// Provides Select, Insert, Update, and Delete functionality
	/// </summary>
	public interface IQueryRunner
	{
		/// <summary>
		/// Builds and runs a query to select all records from the table represented by <typeparamref name="T"/>
		/// </summary>
		/// <returns>All records from the table represented by <typeparamref name="T"/></returns>
		IEnumerable<T> Select<T>(IDbConnection connection, bool buffered = true, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to select all records from the table represented by <typeparamref name="T"/>
		/// </summary>
		/// <returns>All records from the table represented by <typeparamref name="T"/></returns>
		IEnumerable<T> Select<T>(IDbTransaction transaction, bool buffered = true, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to select records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Filtered records from the table represented by <typeparamref name="T"/></returns>
		IEnumerable<T> Select<T>(IDbConnection connection, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, bool buffered = true, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to select records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Filtered records from the table represented by <typeparamref name="T"/></returns>
		IEnumerable<T> Select<T>(IDbTransaction transaction, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, bool buffered = true, int? commandTimeout = null);

		/// <summary>
		/// Builds and runs a query to insert the specified record.  All value provided columns will be set before insert; all sql-generated column values will be set upon insert.
		/// </summary>
		void Insert<T>(IDbConnection connection, T record, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to insert the specified record.  All value provided columns will be set before insert; all sql-generated column values will be set upon insert.
		/// </summary>
		void Insert<T>(IDbTransaction transaction, T record, int? commandTimeout = null);

		/// <summary>
		/// Builds and runs a query to update the specified record.  All value provided columns will be set before update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		int Update<T>(IDbConnection connection, T record, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to update the specified record.  All value provided columns will be set before update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		int Update<T>(IDbTransaction transaction, T record, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		int Update<T>(IDbConnection connection, Func<ValuedUpdateBuilder<T>, ValuedUpdateBuilder<T>> updateBuilder, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		int Update<T>(IDbTransaction transaction, Func<ValuedUpdateBuilder<T>, ValuedUpdateBuilder<T>> updateBuilder, int? commandTimeout = null);

		/// <summary>
		/// Builds and runs a query to delete records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Number of rows affected</returns>
		int Delete<T>(IDbConnection connection, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, int? commandTimeout = null);
		/// <summary>
		/// Builds and runs a query to delete records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Number of rows affected</returns>
		int Delete<T>(IDbTransaction transaction, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, int? commandTimeout = null);
	}
}