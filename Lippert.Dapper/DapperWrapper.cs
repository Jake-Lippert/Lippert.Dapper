using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Lippert.Dapper
{
	/// <summary>
	/// Provides access to Dapper in a testable manner
	/// </summary>
	public class DapperWrapper : Contracts.IDapperWrapper
	{
		/// <summary>
		/// Execute parameterized SQL
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Execute(IDbConnection connection, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Execute(sql, param, null, commandTimeout, commandType);
		/// <summary>
		/// Execute parameterized SQL
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Execute(IDbTransaction transaction, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Execute(sql, param, transaction, commandTimeout, commandType);

		/// <summary>
		/// Executes a single-row query, returning the data typed as per T
		/// </summary>
		/// <returns>A single row of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		public T QuerySingle<T>(IDbConnection connection, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null) =>
			connection.QuerySingle<T>(sql, param, null, commandTimeout, commandType);
		/// <summary>
		/// Executes a single-row query, returning the data typed as per T
		/// </summary>
		/// <returns>A single row of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		public T QuerySingle<T>(IDbTransaction transaction, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per T
		/// </summary>
		/// <returns>A sequence of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		public IEnumerable<T> Query<T>(IDbConnection connection, string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query<T>(sql, param, null, buffered, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per T
		/// </summary>
		/// <returns>A sequence of data of the supplied type;
		/// if a basic type (int, string, etc) is queried then the data from the first column in assumed,
		/// otherwise an instance is created per row, and a direct column-name===member-name mapping is assumed (case insensitive).</returns>
		public IEnumerable<T> Query<T>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst and TSecond
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond>> Query<TFirst, TSecond>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query(sql, (TFirst first, TSecond second) => Tuple.Create(first, second), param, null, buffered, splitOn, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst and TSecond
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond>> Query<TFirst, TSecond>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query(sql, (TFirst first, TSecond second) => Tuple.Create(first, second), param, transaction, buffered, splitOn, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, and TThird
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird>> Query<TFirst, TSecond, TThird>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query(sql, (TFirst first, TSecond second, TThird third) => Tuple.Create(first, second, third), param, null, buffered, splitOn, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, and TThird
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird>> Query<TFirst, TSecond, TThird>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query(sql, (TFirst first, TSecond second, TThird third) => Tuple.Create(first, second, third), param, transaction, buffered, splitOn, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, and TFourth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth>> Query<TFirst, TSecond, TThird, TFourth>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth) => Tuple.Create(first, second, third, fourth), param, null, buffered, splitOn, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, and TFourth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth>> Query<TFirst, TSecond, TThird, TFourth>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth) => Tuple.Create(first, second, third, fourth), param, transaction, buffered, splitOn, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, and TFifth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>> Query<TFirst, TSecond, TThird, TFourth, TFifth>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth) => Tuple.Create(first, second, third, fourth, fifth), param, null, buffered, splitOn, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, and TFifth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>> Query<TFirst, TSecond, TThird, TFourth, TFifth>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth) => Tuple.Create(first, second, third, fourth, fifth), param, transaction, buffered, splitOn, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, and TSixth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth) => Tuple.Create(first, second, third, fourth, fifth, sixth), param, null, buffered, splitOn, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, and TSixth
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth) => Tuple.Create(first, second, third, fourth, fifth, sixth), param, transaction, buffered, splitOn, commandTimeout, commandType);

		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, TSixth, and TSeventh
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IDbConnection connection, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth, TSeventh seventh) => Tuple.Create(first, second, third, fourth, fifth, sixth, seventh), param, null, buffered, splitOn, commandTimeout, commandType);
		/// <summary>
		/// Executes a query, returning the data typed as per TFirst, TSecond, TThird, TFourth, TFifth, TSixth, and TSeventh
		/// </summary>
		/// <returns>A sequence of data of the supplied types</returns>
		public IEnumerable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IDbTransaction transaction, string sql, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
			transaction.Connection.Query(sql, (TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth, TSeventh seventh) => Tuple.Create(first, second, third, fourth, fifth, sixth, seventh), param, transaction, buffered, splitOn, commandTimeout, commandType);

		/// <summary>
		/// Execute a command that returns multiple result sets, and access each in turn
		/// </summary>
		public Contracts.IGridReaderWrapper QueryMultiple(IDbConnection connection, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null) =>
			new GridReaderWrapper(connection.QueryMultiple(sql, param, null, commandTimeout, commandType));
		/// <summary>
		/// Execute a command that returns multiple result sets, and access each in turn
		/// </summary>
		public Contracts.IGridReaderWrapper QueryMultiple(IDbTransaction transaction, string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null) =>
			new GridReaderWrapper(transaction.Connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType));
	}
}