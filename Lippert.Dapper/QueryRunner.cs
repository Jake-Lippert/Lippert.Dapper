using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Lippert.Core.Data;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Dapper
{
	/// <summary>
	/// Provides Select, Insert, Update, and Delete functionality
	/// </summary>
	public class QueryRunner : Contracts.IQueryRunner
	{
		private readonly SqlServerQueryBuilder _queryBuilder = new SqlServerQueryBuilder();
		private readonly Contracts.IDapperWrapper _dapperWrapper;

		/// <summary>
		/// Constructs a QueryRunner with a default concrete DapperWrapper
		/// </summary>
		public QueryRunner()
			: this(new DapperWrapper()) { }
		/// <summary>
		/// Constructs a testable QueryRunner with a specified instance of the IDapperWrapper interface
		/// </summary>
		public QueryRunner(Contracts.IDapperWrapper dapperWrapper) => _dapperWrapper = dapperWrapper;


		/// <summary>
		/// Builds and runs a query to select all records from the table represented by <typeparamref name="T"/>
		/// </summary>
		/// <returns>All records from the table represented by <typeparamref name="T"/></returns>
		public IEnumerable<T> Select<T>(IDbConnection connection, bool buffered = true, int? commandTimeout = null) =>
			BuilderSelect(connection, new ValuedPredicateBuilder<T>(), null, buffered, commandTimeout);
		/// <summary>
		/// Builds and runs a query to select all records from the table represented by <typeparamref name="T"/>
		/// </summary>
		/// <returns>All records from the table represented by <typeparamref name="T"/></returns>
		public IEnumerable<T> Select<T>(IDbTransaction transaction, bool buffered = true, int? commandTimeout = null) =>
			BuilderSelect(null, new ValuedPredicateBuilder<T>(), transaction, buffered, commandTimeout);
		/// <summary>
		/// Builds and runs a query to select records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Filtered records from the table represented by <typeparamref name="T"/></returns>
		public IEnumerable<T> Select<T>(IDbConnection connection, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, bool buffered = true, int? commandTimeout = null) =>
			BuilderSelect(connection, predicateBuilder(new ValuedPredicateBuilder<T>()), null, buffered, commandTimeout);
		/// <summary>
		/// Builds and runs a query to select records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Filtered records from the table represented by <typeparamref name="T"/></returns>
		public IEnumerable<T> Select<T>(IDbTransaction transaction, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, bool buffered = true, int? commandTimeout = null) =>
			BuilderSelect(null, predicateBuilder(new ValuedPredicateBuilder<T>()), transaction, buffered, commandTimeout);
		/// <summary>
		/// Builds and runs a query to select records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Filtered records from the table represented by <typeparamref name="T"/></returns>
		private IEnumerable<T> BuilderSelect<T>(IDbConnection connection, IValuedPredicateBuilder<T> predicateBuilder, IDbTransaction transaction, bool buffered, int? commandTimeout)
		{
			var dynamicParameters = new DynamicParameters();
			foreach (var column in predicateBuilder.GetFilterColumns())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var sql = _queryBuilder.Select(predicateBuilder);
			return _dapperWrapper.Query<T>(transaction?.Connection ?? connection, sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text);
		}


		/// <summary>
		/// Builds and runs a query to insert the specified record.  All value provided columns will be set before insert; all sql-generated column values will be set upon insert.
		/// </summary>
		public void Insert<T>(IDbConnection connection, T record, int? commandTimeout = null) =>
			Insert(connection, record, null, commandTimeout);
		/// <summary>
		/// Builds and runs a query to insert the specified record.  All value provided columns will be set before insert; all sql-generated column values will be set upon insert.
		/// </summary>
		public void Insert<T>(IDbTransaction transaction, T record, int? commandTimeout = null) =>
			Insert(null, record, transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to insert the specified record.  All value provided columns will be set before insert; all sql-generated column values will be set upon insert.
		/// </summary>
		private void Insert<T>(IDbConnection connection, T record, IDbTransaction transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyInsertValues(record);

			var tableMap = SqlServerQueryBuilder.GetTableMap<T>();
			var sql = _queryBuilder.Insert(tableMap);
			if (tableMap.GeneratedColumns.Any())
			{
				var output = _dapperWrapper.Query<T>(transaction?.Connection ?? connection, sql, record, transaction, commandTimeout: commandTimeout, commandType: CommandType.Text).Single();
				foreach (var generatedProperty in tableMap.GeneratedColumns.Select(c => c.Property))
				{
					generatedProperty.SetValue(record, generatedProperty.GetValue(output));
				}
			}
			else
			{
				_dapperWrapper.Execute(transaction?.Connection ?? connection, sql, record, transaction, commandTimeout: commandTimeout, commandType: CommandType.Text);
			}
		}


		/// <summary>
		/// Builds and runs a query to update the specified record.  All value provided columns will be set before update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Update<T>(IDbConnection connection, T record, int? commandTimeout = null) =>
			Update(connection, record, null, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the specified record.  All value provided columns will be set before update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Update<T>(IDbTransaction transaction, T record, int? commandTimeout = null) =>
			Update(null, record, transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the specified record.  All value provided columns will be set before update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		private int Update<T>(IDbConnection connection, T record, IDbTransaction transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyUpdateValues(record);

			var sql = _queryBuilder.Update<T>();
			return _dapperWrapper.Execute(transaction?.Connection ?? connection, sql, record, transaction, commandTimeout, CommandType.Text);
		}

		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Update<T>(IDbConnection connection, Func<ValuedUpdateBuilder<T>, ValuedUpdateBuilder<T>> updateBuilder, int? commandTimeout = null) =>
			BuilderUpdate(connection, updateBuilder(new ValuedUpdateBuilder<T>()), null, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Update<T>(IDbTransaction transaction, Func<ValuedUpdateBuilder<T>, ValuedUpdateBuilder<T>> updateBuilder, int? commandTimeout = null) =>
			BuilderUpdate(null, updateBuilder(new ValuedUpdateBuilder<T>()), transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		private int BuilderUpdate<T>(IDbConnection connection, IValuedUpdateBuilder<T> updateBuilder, IDbTransaction transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyUpdateBuilderValues(updateBuilder);

			var dynamicParameters = new DynamicParameters();
			foreach (var column in updateBuilder.GetFilterColumns())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var underscoreRequired = updateBuilder.GetFilterColumns().Select(fc => fc.ColumnName)
				.Intersect(updateBuilder.GetSetColumns().Select(sc => sc.ColumnName))
				.Any();
			foreach (var column in updateBuilder.GetSetColumns())
			{
				dynamicParameters.Add(underscoreRequired ? $"_{column.ColumnName}" : column.ColumnName, column.Value);
			}
			
			var sql = _queryBuilder.Update(updateBuilder);
			return _dapperWrapper.Execute(transaction?.Connection ?? connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
		}

		/// <summary>
		/// Builds and runs a query to delete records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Number of rows affected</returns>
		public int Delete<T>(IDbConnection connection, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, int? commandTimeout = null) =>
			BuilderDelete(connection, predicateBuilder(new ValuedPredicateBuilder<T>()), null, commandTimeout);
		/// <summary>
		/// Builds and runs a query to delete records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Number of rows affected</returns>
		public int Delete<T>(IDbTransaction transaction, Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>> predicateBuilder, int? commandTimeout = null) =>
			BuilderDelete(null, predicateBuilder(new ValuedPredicateBuilder<T>()), transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to delete records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Number of rows affected</returns>
		private int BuilderDelete<T>(IDbConnection connection, IValuedPredicateBuilder<T> predicateBuilder, IDbTransaction transaction, int? commandTimeout)
		{
			var dynamicParameters = new DynamicParameters();
			foreach (var column in predicateBuilder.GetFilterColumns())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var sql = _queryBuilder.Delete(predicateBuilder);
			return _dapperWrapper.Execute(transaction?.Connection ?? connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
		}
	}
}