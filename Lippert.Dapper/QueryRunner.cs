using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders;

namespace Lippert.Dapper
{
	public class QueryRunner : Contracts.IQueryRunner
	{
		private readonly SqlServerQueryBuilder _queryBuilder = new SqlServerQueryBuilder();
		private readonly Contracts.IDapperWrapper _dapperWrapper;

		public QueryRunner()
			: this(new DapperWrapper()) { }
		public QueryRunner(Contracts.IDapperWrapper dapperWrapper) => _dapperWrapper = dapperWrapper;


		public T Select<T>(IDbConnection connection, object key, bool buffered = true, int? commandTimeout = null) =>
			Select(connection, new PredicateBuilder<T>().Key(key), null, buffered, commandTimeout).SingleOrDefault();
		public T Select<T>(IDbTransaction transaction, object key, bool buffered = true, int? commandTimeout = null) =>
			Select(null, new PredicateBuilder<T>().Key(key), transaction, buffered, commandTimeout).SingleOrDefault();

		public IEnumerable<T> Select<T>(IDbConnection connection, bool buffered = true, int? commandTimeout = null) =>
			Select(connection, new PredicateBuilder<T>(), null, buffered, commandTimeout);
		public IEnumerable<T> Select<T>(IDbTransaction transaction, bool buffered = true, int? commandTimeout = null) =>
			Select(null, new PredicateBuilder<T>(), transaction, buffered, commandTimeout);

		public IEnumerable<T> Select<T>(IDbConnection connection, PredicateBuilder<T> selectBuilder, bool buffered = true, int? commandTimeout = null) =>
			Select(connection, selectBuilder, null, buffered, commandTimeout);
		public IEnumerable<T> Select<T>(IDbTransaction transaction, PredicateBuilder<T> selectBuilder, bool buffered = true, int? commandTimeout = null) =>
			Select(null, selectBuilder, transaction, buffered, commandTimeout);
		private IEnumerable<T> Select<T>(IDbConnection connection, PredicateBuilder<T> selectBuilder, IDbTransaction transaction, bool buffered, int? commandTimeout)
		{
			var dynamicParameters = new DynamicParameters();
			foreach (var column in selectBuilder.GetFilterColumns(false).OfType<IValuedColumnMap>())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var sql = _queryBuilder.Select(selectBuilder);
			return _dapperWrapper.Query<T>(transaction?.Connection ?? connection, sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text);
		}


		public void Insert<T>(IDbConnection connection, T record, int? commandTimeout = null) =>
			Insert(connection, record, null, commandTimeout);
		public void Insert<T>(IDbTransaction transaction, T record, int? commandTimeout = null) =>
			Insert(null, record, transaction, commandTimeout);
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


		public int Update<T>(IDbConnection connection, T record, int? commandTimeout = null) =>
			Update(connection, record, null, commandTimeout);
		public int Update<T>(IDbTransaction transaction, T record, int? commandTimeout = null) =>
			Update(null, record, transaction, commandTimeout);
		private int Update<T>(IDbConnection connection, T record, IDbTransaction transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyUpdateValues(record);

			var sql = _queryBuilder.Update<T>();
			return _dapperWrapper.Execute(transaction?.Connection ?? connection, sql, record, transaction, commandTimeout, CommandType.Text);
		}

		public int Update<T>(IDbConnection connection, UpdateBuilder<T> updateBuilder, int? commandTimeout = null) =>
			Update(connection, updateBuilder, null, commandTimeout);
		public int Update<T>(IDbTransaction transaction, UpdateBuilder<T> updateBuilder, int? commandTimeout = null) =>
			Update(null, updateBuilder, transaction, commandTimeout);
		private int Update<T>(IDbConnection connection, UpdateBuilder<T> updateBuilder, IDbTransaction transaction, int? commandTimeout)
		{
			//--Set properties using value providers
			ColumnValueProvider.ApplyUpdateBuilderValues(updateBuilder);

			var dynamicParameters = new DynamicParameters();
			foreach (var column in updateBuilder.GetFilterColumns(true).OfType<IValuedColumnMap>())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var underscoreRequired = updateBuilder.UnderscoreRequired;
			foreach (var column in updateBuilder.SetColumns.OfType<IValuedColumnMap>())
			{
				dynamicParameters.Add(underscoreRequired ? $"_{column.ColumnName}" : column.ColumnName, column.Value);
			}
			
			var sql = _queryBuilder.Update(updateBuilder);
			return _dapperWrapper.Execute(transaction?.Connection ?? connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
		}

		public int Delete<T>(IDbConnection connection, object key, int? commandTimeout = null) =>
			Delete(connection, new PredicateBuilder<T>().Key(key), null, commandTimeout);
		public int Delete<T>(IDbTransaction transaction, object key, int? commandTimeout = null) =>
			Delete(null, new PredicateBuilder<T>().Key(key), transaction, commandTimeout);

		public int Delete<T>(IDbConnection connection, PredicateBuilder<T> predicateBuilder, int? commandTimeout = null) =>
			Delete(connection, predicateBuilder, null, commandTimeout);
		public int Delete<T>(IDbTransaction transaction, PredicateBuilder<T> predicateBuilder, int? commandTimeout = null) =>
			Delete(null, predicateBuilder, transaction, commandTimeout);
		private int Delete<T>(IDbConnection connection, PredicateBuilder<T> predicateBuilder, IDbTransaction transaction, int? commandTimeout)
		{
			var dynamicParameters = new DynamicParameters();
			foreach (var column in predicateBuilder.GetFilterColumns(true).OfType<IValuedColumnMap>())
			{
				dynamicParameters.Add(column.ColumnName, column.Value);
			}

			var sql = _queryBuilder.Delete(predicateBuilder);
			return _dapperWrapper.Execute(transaction?.Connection ?? connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
		}
	}
}