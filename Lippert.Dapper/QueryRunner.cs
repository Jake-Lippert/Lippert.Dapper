using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Dapper;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Data.QueryBuilders.Contracts;
using Newtonsoft.Json;

namespace Lippert.Dapper
{
	/// <summary>
	/// Provides Select, Insert, Update, and Delete functionality
	/// </summary>
	public class QueryRunner : Contracts.IQueryRunner
	{
		private readonly SqlServerSelectQueryBuilder _selectQueryBuilder = new SqlServerSelectQueryBuilder();
		private readonly SqlServerInsertQueryBuilder _insertQueryBuilder = new SqlServerInsertQueryBuilder();
		private readonly SqlServerUpdateQueryBuilder _updateQueryBuilder = new SqlServerUpdateQueryBuilder();
		private readonly SqlServerDeleteQueryBuilder _deleteQueryBuilder = new SqlServerDeleteQueryBuilder();
		private readonly SqlServerMergeQueryBuilder _mergeQueryBuilder = new SqlServerMergeQueryBuilder();
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
			BuilderSelect(transaction.Connection, new ValuedPredicateBuilder<T>(), transaction, buffered, commandTimeout);
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
			BuilderSelect(transaction.Connection, predicateBuilder(new ValuedPredicateBuilder<T>()), transaction, buffered, commandTimeout);
		/// <summary>
		/// Builds and runs a query to select records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Filtered records from the table represented by <typeparamref name="T"/></returns>
		private IEnumerable<T> BuilderSelect<T>(IDbConnection connection, IValuedPredicateBuilder<T> predicateBuilder, IDbTransaction? transaction, bool buffered, int? commandTimeout)
		{
			var dynamicParameters = new DynamicParameters();
			foreach (var column in predicateBuilder.GetFilterColumns())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var sql = _selectQueryBuilder.Select(predicateBuilder);
			return Query<T>(connection, sql, dynamicParameters, transaction, buffered, commandTimeout);
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
			Insert(transaction.Connection, record, transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to insert the specified record.  All value provided columns will be set before insert; all sql-generated column values will be set upon insert.
		/// </summary>
		private void Insert<T>(IDbConnection connection, T record, IDbTransaction? transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyInsertValues(record);

			var tableMap = SqlServerQueryBuilder.GetTableMap<T>();
			var sql = _insertQueryBuilder.Insert(tableMap);
			if (tableMap.GeneratedColumns.Any())
			{
				var output = Query<T>(connection, sql, record, transaction, true, commandTimeout).Single();
				foreach (var generatedProperty in tableMap.GeneratedColumns.Select(c => c.Property))
				{
					generatedProperty.SetValue(record, generatedProperty.GetValue(output));
				}
			}
			else
			{
				Execute(connection, sql, record, transaction, commandTimeout);
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
			Update(transaction.Connection, record, transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the specified record.  All value provided columns will be set before update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		private int Update<T>(IDbConnection connection, T record, IDbTransaction? transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyUpdateValues(record);

			var sql = _updateQueryBuilder.Update<T>();
			return Execute(connection, sql, record, transaction, commandTimeout);
		}

		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Update<T>(IDbConnection connection, Func<ValuedUpdateBuilder<T>, ValuedUpdateBuilder<T>> updateBuilder, int? commandTimeout = null)
			where T : new() => BuilderUpdate(connection, updateBuilder(new ValuedUpdateBuilder<T>()), null, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		public int Update<T>(IDbTransaction transaction, Func<ValuedUpdateBuilder<T>, ValuedUpdateBuilder<T>> updateBuilder, int? commandTimeout = null)
			where T : new() => BuilderUpdate(transaction.Connection, updateBuilder(new ValuedUpdateBuilder<T>()), transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to update the matching records.  All value provided columns will be included in update.
		/// </summary>
		/// <returns>Number of rows affected</returns>
		private int BuilderUpdate<T>(IDbConnection connection, IValuedUpdateBuilder<T> updateBuilder, IDbTransaction? transaction, int? commandTimeout)
			where T : new()
		{
			//--Create a new T and record the original property values...
			var record = new T();
			var originalValues = updateBuilder.TableMap.UpdateColumns.ToDictionary(x => x.Property, x => x.Property.GetValue(record));

			//--Set properties using value providers
			ColumnValueProvider.ApplyUpdateValues(record);

			//--...and then figure out which values changed;
			//	Set the values for update which have changed
			foreach (var (property, original, current) in originalValues.Select(x => (x.Key, x.Value, x.Key.GetValue(record))))
			{
				if (!Equals(original, current))
				{
					updateBuilder.Set(property, current);
				}
			}

			//--Add where clause parameters/values
			var dynamicParameters = new DynamicParameters();
			foreach (var column in updateBuilder.GetFilterColumns())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			//--Add set clause parameters/values
			var underscoreRequired = updateBuilder.GetFilterColumns().Select(fc => fc.ColumnName)
				.Intersect(updateBuilder.GetSetColumns().Select(sc => sc.ColumnName))
				.Any();
			foreach (var column in updateBuilder.GetSetColumns())
			{
				dynamicParameters.Add(underscoreRequired ? $"_{column.ColumnName}" : column.ColumnName, column.Value);
			}
			
			var sql = _updateQueryBuilder.Update(updateBuilder);
			return Execute(connection, sql, dynamicParameters, transaction, commandTimeout);
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
			BuilderDelete(transaction.Connection, predicateBuilder(new ValuedPredicateBuilder<T>()), transaction, commandTimeout);
		/// <summary>
		/// Builds and runs a query to delete records from the table represented by <typeparamref name="T"/> which match the specified filter
		/// </summary>
		/// <param name="predicateBuilder">Specifies the predicate for the query's where clause</param>
		/// <returns>Number of rows affected</returns>
		private int BuilderDelete<T>(IDbConnection connection, IValuedPredicateBuilder<T> predicateBuilder, IDbTransaction? transaction, int? commandTimeout)
		{
			var dynamicParameters = new DynamicParameters();
			foreach (var column in predicateBuilder.GetFilterColumns())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var sql = _deleteQueryBuilder.Delete(predicateBuilder);
			return Execute(connection, sql, dynamicParameters, transaction, commandTimeout);
		}

		/// <summary>
		/// Builds and runs a query to merge records for the table represented by <typeparamref name="T"/>
		/// </summary>
		public void Merge<T>(IDbConnection connection, IEnumerable<T> records, SqlOperation mergeOperations, bool buffered = true, int? commandTimeout = null, bool useJson = false) =>
			Merge(connection, records, null, mergeOperations, buffered, commandTimeout, useJson);
		/// <summary>
		/// Builds and runs a query to merge records for the table represented by <typeparamref name="T"/>
		/// </summary>
		public void Merge<T>(IDbTransaction transaction, IEnumerable<T> records, SqlOperation mergeOperations, bool buffered = true, int? commandTimeout = null, bool useJson = false) =>
			Merge(transaction.Connection, records, transaction, mergeOperations, buffered, commandTimeout, useJson);
		/// <summary>
		/// Builds and runs a query to merge records for the table represented by <typeparamref name="T"/>
		/// </summary>
		private void Merge<T>(IDbConnection connection, IEnumerable<T> records, IDbTransaction? transaction, SqlOperation mergeOperations, bool buffered, int? commandTimeout, bool useJson)
		{
			var workingRecords = records.Select(record =>
			{
				//--Set properties using value providers
				if (mergeOperations.HasFlag(SqlOperation.Update))
				{
					ColumnValueProvider.ApplyUpdateValues(record);
				}
				if (mergeOperations.HasFlag(SqlOperation.Insert))
				{
					ColumnValueProvider.ApplyInsertValues(record);
				}

				return record;
			}).ToList();

			string sql, serialized;
			var tableMap = SqlServerQueryBuilder.GetTableMap<T>();
			if (useJson)
			{
				sql = _mergeQueryBuilder.Merge(converter: out var converter, mergeOperations, tableMap);
				serialized = JsonConvert.SerializeObject(workingRecords, converter);
			}
			else
			{
				sql = _mergeQueryBuilder.Merge(aliases: out var aliases, mergeOperations, tableMap, useJson: useJson);

				var toSerialize = new XElement("_");
				foreach (var (wr, index) in workingRecords.Indexed())
				{
					var xmlRecord = new XElement("_", new XAttribute("_", index));
					foreach (var (key, value) in aliases.Select(a => (a.Value, a.Key.GetValue(wr))))
					{
						//--Don't serialize null values
						if (value != null)
						{
							xmlRecord.Add(new XAttribute($"_{key}", value));
						}
					}

					toSerialize.Add(xmlRecord);
				}
				serialized = toSerialize.ToString();
			}

			if (mergeOperations.HasFlag(SqlOperation.Insert))
			{
				var output = (transaction switch
				{
					IDbTransaction trans => _dapperWrapper.Query<SqlServerMergeQueryBuilder.RecordMergeCorrelation, T>(trans, sql, new { serialized }, buffered, SqlServerQueryBuilder.SplitOn, commandTimeout),
					_ => _dapperWrapper.Query<SqlServerMergeQueryBuilder.RecordMergeCorrelation, T>(connection, sql, new { serialized }, buffered, SqlServerQueryBuilder.SplitOn, commandTimeout)
				}).ToDictionary(x => x.Item1.CorrelationIndex, x => (x.Item1.Action, Record: x.Item2));

				foreach (var (record, index) in workingRecords.Indexed())
				{
					if (output.TryGetValue(index, out var recordAction) && recordAction is ("INSERT", { } insertedRecord))
					{
						foreach (var generatedProperty in tableMap.GeneratedColumns.Select(c => c.Property))
						{
							generatedProperty.SetValue(record, generatedProperty.GetValue(insertedRecord));
						}
					}
				}
			}
			else if (transaction is IDbTransaction trans)
			{
				_dapperWrapper.Execute(trans, sql, new { serialized }, commandTimeout);
			}
			else
			{
				_dapperWrapper.Execute(connection, sql, new { serialized }, commandTimeout);
			}
		}

		private int Execute(IDbConnection connection, string sql, object? param, IDbTransaction? transaction, int? commandTimeout) => transaction switch
		{
			IDbTransaction trans => _dapperWrapper.Execute(trans, sql, param, commandTimeout, CommandType.Text),
			_ => _dapperWrapper.Execute(connection, sql, param, commandTimeout, CommandType.Text)
		};
		private IEnumerable<T> Query<T>(IDbConnection connection, string sql, object? param, IDbTransaction? transaction, bool buffered, int? commandTimeout) => transaction switch
		{
			IDbTransaction trans => _dapperWrapper.Query<T>(trans, sql, param, buffered, commandTimeout, CommandType.Text),
			_ => _dapperWrapper.Query<T>(connection, sql, param, buffered, commandTimeout, CommandType.Text)
		};
	}
}