using System;
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
		int Execute(IDbConnection connection, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Execute parameterized SQL
		/// </summary>
		/// <returns>Number of rows affected</returns>
		int Execute(IDbTransaction transaction, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a single-row query, returning the data typed as per T
		/// </summary>
		/// <returns>A single row of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		T QuerySingle<T>(IDbConnection connection, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a single-row query, returning the data typed as per T
		/// </summary>
		/// <returns>A single row of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		T QuerySingle<T>(IDbTransaction transaction, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per T
		/// </summary>
		/// <returns>A sequence of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		IEnumerable<T> Query<T>(IDbConnection connection, string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per T
		/// </summary>
		/// <returns>A sequence of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		IEnumerable<T> Query<T>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst and TSecond
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond>> Query<TFirst, TSecond>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst and TSecond
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond>> Query<TFirst, TSecond>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, and TThird
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird>> Query<TFirst, TSecond, TThird>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, and TThird
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird>> Query<TFirst, TSecond, TThird>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, and TFourth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth>> Query<TFirst, TSecond, TThird, TFourth>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, and TFourth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth>> Query<TFirst, TSecond, TThird, TFourth>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, and TFifth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>> Query<TFirst, TSecond, TThird, TFourth, TFifth>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, and TFifth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>> Query<TFirst, TSecond, TThird, TFourth, TFifth>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, and TSixth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, and TSixth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, TSixth, and TSeventh
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, TSixth, and TSeventh
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

		/// <summary>
		/// Execute a command that returns multiple result sets, and access each in turn
		/// </summary>
		IGridReaderWrapper QueryMultiple(IDbConnection connection, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);
		/// <summary>
		/// Execute a command that returns multiple result sets, and access each in turn
		/// </summary>
		IGridReaderWrapper QueryMultiple(IDbTransaction transaction, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null);
	}
}