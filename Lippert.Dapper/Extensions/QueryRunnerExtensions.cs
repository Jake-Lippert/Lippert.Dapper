using System.Data;
using System.Linq;
using Lippert.Core.Data;
using Lippert.Dapper.Contracts;

namespace Lippert.Dapper.Extensions
{
	public static class QueryRunnerExtensions
	{
		/// <summary>
		/// Builds and runs a query to select the record from the table represented by <typeparamref name="T"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <returns>The record from the table represented by <typeparamref name="T"/> which matches the specified key</returns>
		public static T Get<T>(this IQueryRunner queryRunner, IDbConnection connection, object key) => queryRunner.Select<T>(connection, x => x.Key(key)).Single();
		/// <summary>
		/// Builds and runs a query to select the record from the table represented by <typeparamref name="T"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <returns>The record from the table represented by <typeparamref name="T"/> which matches the specified key</returns>
		public static T Get<T>(this IQueryRunner queryRunner, IDbTransaction transaction, object key) => queryRunner.Select<T>(transaction, x => x.Key(key)).Single();

		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <returns>The record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key</returns>
		public static T1 Get<T1, T2>(this IQueryRunner queryRunner, IDbConnection connection, object key)
			where T1 : class
			where T2 : class, T1
		{
			var baseRecord = queryRunner.Get<T1>(connection, key);
			if (queryRunner.Select<T2>(connection, x => x.Key(key)).SingleOrDefault() is { } inheritingRecord)
			{
				foreach (var property in typeof(T1).GetProperties())
				{
					property.SetValue(inheritingRecord, property.GetValue(baseRecord));
				}

				return inheritingRecord;
			}

			return baseRecord;
		}
		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <returns>The record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key</returns>
		public static T1 Get<T1, T2>(this IQueryRunner queryRunner, IDbTransaction transaction, object key)
			where T1 : class
			where T2 : class, T1
		{
			var baseRecord = queryRunner.Get<T1>(transaction, key);
			if (queryRunner.Select<T2>(transaction, x => x.Key(key)).SingleOrDefault() is { } inheritingRecord)
			{
				foreach (var property in typeof(T1).GetProperties())
				{
					property.SetValue(inheritingRecord, property.GetValue(baseRecord));
				}

				return inheritingRecord;
			}

			return baseRecord;
		}

		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <returns>The record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key</returns>
		public static T1 Get<T1, T2, T3>(this IQueryRunner queryRunner, IDbConnection connection, object key)
			where T1 : class
			where T2 : class, T1
			where T3 : class, T2
		{
			var baseRecord = queryRunner.Get<T1, T2>(connection, key);
			if (queryRunner.Select<T3>(connection, x => x.Key(key)).SingleOrDefault() is { } inheritingRecord)
			{
				foreach (var property in typeof(T2).GetProperties())
				{
					property.SetValue(inheritingRecord, property.GetValue(baseRecord));
				}

				return inheritingRecord;
			}

			return baseRecord;
		}
		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <returns>The record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key</returns>
		public static T1 Get<T1, T2, T3>(this IQueryRunner queryRunner, IDbTransaction transaction, object key)
			where T1 : class
			where T2 : class, T1
			where T3 : class, T2
		{
			var baseRecord = queryRunner.Get<T1, T2>(transaction, key);
			if (queryRunner.Select<T3>(transaction, x => x.Key(key)).SingleOrDefault() is { } inheritingRecord)
			{
				foreach (var property in typeof(T2).GetProperties())
				{
					property.SetValue(inheritingRecord, property.GetValue(baseRecord));
				}

				return inheritingRecord;
			}

			return baseRecord;
		}


		/// <summary>
		/// Builds and runs a query to select the record from the table represented by <typeparamref name="T"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <param name="value">The record from the table represented by <typeparamref name="T"/> which matches the specified key</param>
		/// <returns>Was the matching record able to be found?</returns>
		public static bool TryGet<T>(this IQueryRunner queryRunner, IDbConnection connection, object key, out T? value)
			where T : class
		{
			try
			{
				value = queryRunner.Get<T>(connection, key);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}
		/// <summary>
		/// Builds and runs a query to select the record from the table represented by <typeparamref name="T"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <param name="value">The record from the table represented by <typeparamref name="T"/> which matches the specified key</param>
		/// <returns>Was the matching record able to be found?</returns>
		public static bool TryGet<T>(this IQueryRunner queryRunner, IDbTransaction transaction, object key, out T? value)
			where T : class
		{
			try
			{
				value = queryRunner.Get<T>(transaction, key);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}

		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <param name="value">The record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key</param>
		/// <returns>Was the matching record able to be found?</returns>
		public static bool TryGet<T1, T2>(this IQueryRunner queryRunner, IDbConnection connection, object key, out T1? value)
			where T1 : class
			where T2 : class, T1
		{
			try
			{
				value = queryRunner.Get<T1, T2>(connection, key);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}
		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <param name="value">The record from the table(s) represented by <typeparamref name="T1"/> and <typeparamref name="T2"/> which matches the specified key</param>
		/// <returns>Was the matching record able to be found?</returns>
		public static bool TryGet<T1, T2>(this IQueryRunner queryRunner, IDbTransaction transaction, object key, out T1? value)
			where T1 : class
			where T2 : class, T1
		{
			try
			{
				value = queryRunner.Get<T1, T2>(transaction, key);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}

		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <param name="value">The record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key</param>
		/// <returns>Was the matching record able to be found?</returns>
		public static bool TryGet<T1, T2, T3>(this IQueryRunner queryRunner, IDbConnection connection, object key, out T1? value)
			where T1 : class
			where T2 : class, T1
			where T3 : class, T2
		{
			try
			{
				value = queryRunner.Get<T1, T2, T3>(connection, key);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}
		/// <summary>
		/// Builds and runs a query to select the record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key
		/// </summary>
		/// <param name="key">Specifies the key for the query's where clause</param>
		/// <param name="value">The record from the table(s) represented by <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/> which matches the specified key</param>
		/// <returns>Was the matching record able to be found?</returns>
		public static bool TryGet<T1, T2, T3>(this IQueryRunner queryRunner, IDbTransaction transaction, object key, out T1? value)
			where T1 : class
			where T2 : class, T1
			where T3 : class, T2
		{
			try
			{
				value = queryRunner.Get<T1, T2, T3>(transaction, key);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}


		/// <summary>
		/// Builds and runs a query to upsert the record into the table represented by <typeparamref name="T"/> using a sql merge
		/// </summary>
		public static void Upsert<T>(this IQueryRunner queryRunner, IDbConnection connection, T record, bool buffered = true, int? commandTimeout = null, bool useJson = false) =>
			queryRunner.Merge(connection, new[] { record }, SqlOperation.Insert | SqlOperation.Update, buffered, commandTimeout, useJson);
		/// <summary>
		/// Builds and runs a query to upsert the record into the table represented by <typeparamref name="T"/> using a sql merge
		/// </summary>
		public static void Upsert<T>(this IQueryRunner queryRunner, IDbTransaction transaction, T record, bool buffered = true, int? commandTimeout = null, bool useJson = false) =>
			queryRunner.Merge(transaction, new[] { record }, SqlOperation.Insert | SqlOperation.Update, buffered, commandTimeout, useJson);
	}
}