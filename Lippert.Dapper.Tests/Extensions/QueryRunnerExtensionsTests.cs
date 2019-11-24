using System;
using System.Data;
using System.Linq;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Dapper.Contracts;
using Lippert.Dapper.Extensions;
using Lippert.Dapper.Tests.TestSchema;
using Moq;
using NUnit.Framework;

namespace Lippert.Dapper.Tests.Extensions
{
	[TestFixture]
	public class QueryRunnerExtensionsTests
	{
		[Test]
		public void TestGetsByKey([Range(0, 2)] int resultsToReturn, [Values(true, false)] bool useTry, [Values(true, false)] bool useTransaction)
		{
			//--Arrange
			var id = Guid.NewGuid();

			//--Mock
			var connectionMock = new Mock<IDbConnection>();
			var transactionMock = new Mock<IDbTransaction>();
			var queryRunnerMock = new Mock<IQueryRunner>();
			queryRunnerMock.Setup(x => x.Select<User>(It.IsAny<IDbConnection>(), It.IsAny<Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
				.Returns((IDbConnection connection, Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
					.Select(x => new User { Id = id }));
			queryRunnerMock.Setup(x => x.Select<User>(It.IsAny<IDbTransaction>(), It.IsAny<Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
				.Returns((IDbTransaction transaction, Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
					.Select(x => new User { Id = id }));

			//--Act/Assert
			User? user;
			if (resultsToReturn == 1)
			{
				if (useTry)
				{
					Assert.IsTrue(useTransaction ?
						queryRunnerMock.Object.TryGet<User>(transactionMock.Object, id, out user) :
						queryRunnerMock.Object.TryGet<User>(connectionMock.Object, id, out user));
				}
				else
				{
					user = useTransaction ?
						queryRunnerMock.Object.Get<User>(transactionMock.Object, id) :
						queryRunnerMock.Object.Get<User>(connectionMock.Object, id);
				}

				Assert.IsInstanceOf<User>(user);
				Assert.AreEqual(id, user!.Id);
			}
			else if (useTry)
			{
				Assert.IsFalse(useTransaction ?
					queryRunnerMock.Object.TryGet<User>(transactionMock.Object, id, out user) :
					queryRunnerMock.Object.TryGet<User>(connectionMock.Object, id, out user));
				Assert.IsNull(user);
			}
			else
			{
				Assert.Throws<InvalidOperationException>(() => queryRunnerMock.Object.Get<User>(connectionMock.Object, id));
			}
		}

		[Test]
		public void TestGetsByKeyWithPossibleInheritance([Range(0, 2)] int resultsToReturn, [Values(true, false)] bool useTry, [Values(true, false)] bool useTransaction, [Range(0, 1)] int inheritanceLevel)
		{
			//--Arrange
			var id = Guid.NewGuid();

			//--Mock
			var connectionMock = new Mock<IDbConnection>();
			var transactionMock = new Mock<IDbTransaction>();
			var queryRunnerMock = new Mock<IQueryRunner>();
			queryRunnerMock.Setup(x => x.Select<User>(It.IsAny<IDbConnection>(), It.IsAny<Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
				.Returns((IDbConnection connection, Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
					.Select(x => new User { Id = id }));
			queryRunnerMock.Setup(x => x.Select<User>(It.IsAny<IDbTransaction>(), It.IsAny<Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
				.Returns((IDbTransaction transaction, Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
					.Select(x => new User { Id = id }));
			if (inheritanceLevel > 0)
			{
				queryRunnerMock.Setup(x => x.Select<Employee>(It.IsAny<IDbConnection>(), It.IsAny<Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
					.Returns((IDbConnection connection, Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
						.Select(x => new Employee { UserId = id }));
				queryRunnerMock.Setup(x => x.Select<Employee>(It.IsAny<IDbTransaction>(), It.IsAny<Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
					.Returns((IDbTransaction transaction, Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
						.Select(x => new Employee { UserId = id }));
			}

			//--Act/Assert
			User? user;
			if (resultsToReturn == 1)
			{
				if (useTry)
				{
					Assert.IsTrue(useTransaction ?
						queryRunnerMock.Object.TryGet<User, Employee>(transactionMock.Object, id, out user) :
						queryRunnerMock.Object.TryGet<User, Employee>(connectionMock.Object, id, out user));
				}
				else
				{
					user = useTransaction ?
						queryRunnerMock.Object.Get<User, Employee>(transactionMock.Object, id) :
						queryRunnerMock.Object.Get<User, Employee>(connectionMock.Object, id);
				}

				Assert.IsInstanceOf<User>(user);
				Assert.AreEqual(id, user!.Id);
				if (inheritanceLevel > 0)
				{
					Assert.IsInstanceOf<Employee>(user);
					Assert.AreEqual(user!.Id, ((Employee)user).UserId);
				}
				else
				{
					Assert.IsNotInstanceOf<Employee>(user);
				}
			}
			else if (useTry)
			{
				Assert.IsFalse(useTransaction ?
					queryRunnerMock.Object.TryGet<User, Employee>(transactionMock.Object, id, out user) :
					queryRunnerMock.Object.TryGet<User, Employee>(connectionMock.Object, id, out user));
				Assert.IsNull(user);
			}
			else
			{
				Assert.Throws<InvalidOperationException>(() => queryRunnerMock.Object.Get<User, Employee>(connectionMock.Object, id));
			}
		}

		[Test]
		public void TestGetsByKeyWithPossibleDoubleInheritance([Range(0, 2)] int resultsToReturn, [Values(true, false)] bool useTry, [Values(true, false)] bool useTransaction, [Range(0, 2)] int inheritanceLevel)
		{
			//--Arrange
			var id = Guid.NewGuid();

			//--Mock
			var connectionMock = new Mock<IDbConnection>();
			var transactionMock = new Mock<IDbTransaction>();
			var queryRunnerMock = new Mock<IQueryRunner>();
			queryRunnerMock.Setup(x => x.Select<User>(It.IsAny<IDbConnection>(), It.IsAny<Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
				.Returns((IDbConnection connection, Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
					.Select(x => new User { Id = id }));
			queryRunnerMock.Setup(x => x.Select<User>(It.IsAny<IDbTransaction>(), It.IsAny<Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
				.Returns((IDbTransaction transaction, Func<ValuedPredicateBuilder<User>, ValuedPredicateBuilder<User>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
					.Select(x => new User { Id = id }));
			if (inheritanceLevel > 0)
			{
				queryRunnerMock.Setup(x => x.Select<Employee>(It.IsAny<IDbConnection>(), It.IsAny<Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
					.Returns((IDbConnection connection, Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
						.Select(x => new Employee { UserId = id }));
				queryRunnerMock.Setup(x => x.Select<Employee>(It.IsAny<IDbTransaction>(), It.IsAny<Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
					.Returns((IDbTransaction transaction, Func<ValuedPredicateBuilder<Employee>, ValuedPredicateBuilder<Employee>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
						.Select(x => new Employee { UserId = id }));
				if (inheritanceLevel > 1)
				{
					queryRunnerMock.Setup(x => x.Select<SuperEmployee>(It.IsAny<IDbConnection>(), It.IsAny<Func<ValuedPredicateBuilder<SuperEmployee>, ValuedPredicateBuilder<SuperEmployee>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
						.Returns((IDbConnection connection, Func<ValuedPredicateBuilder<SuperEmployee>, ValuedPredicateBuilder<SuperEmployee>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
							.Select(x => new SuperEmployee { EmployeeId = id }));
					queryRunnerMock.Setup(x => x.Select<SuperEmployee>(It.IsAny<IDbTransaction>(), It.IsAny<Func<ValuedPredicateBuilder<SuperEmployee>, ValuedPredicateBuilder<SuperEmployee>>>(), It.IsAny<bool>(), It.IsAny<int?>()))
						.Returns((IDbTransaction transaction, Func<ValuedPredicateBuilder<SuperEmployee>, ValuedPredicateBuilder<SuperEmployee>> predicateBuilder, bool buffered, int? commandTimeout) => Enumerable.Range(0, resultsToReturn)
							.Select(x => new SuperEmployee { EmployeeId = id }));
				}
			}

			//--Act/Assert
			User? user;
			if (resultsToReturn == 1)
			{
				if (useTry)
				{
					Assert.IsTrue(useTransaction ?
						queryRunnerMock.Object.TryGet<User, Employee, SuperEmployee>(transactionMock.Object, id, out user) :
						queryRunnerMock.Object.TryGet<User, Employee, SuperEmployee>(connectionMock.Object, id, out user));
				}
				else
				{
					user = useTransaction ?
						queryRunnerMock.Object.Get<User, Employee, SuperEmployee>(transactionMock.Object, id) :
						queryRunnerMock.Object.Get<User, Employee, SuperEmployee>(connectionMock.Object, id);
				}

				Assert.IsInstanceOf<User>(user);
				Assert.AreEqual(id, user!.Id);
				if (inheritanceLevel > 0)
				{
					Assert.IsInstanceOf<Employee>(user);
					Assert.AreEqual(user!.Id, ((Employee)user).UserId);
					if (inheritanceLevel > 1)
					{
						Assert.IsInstanceOf<SuperEmployee>(user);
						Assert.AreEqual(user!.Id, ((SuperEmployee)user).UserId);
					}
					else
					{
						Assert.IsNotInstanceOf<SuperEmployee>(user);
					}
				}
				else
				{
					Assert.IsNotInstanceOf<Employee>(user);
				}
			}
			else if (useTry)
			{
				Assert.IsFalse(useTransaction ?
					queryRunnerMock.Object.TryGet<User, Employee, SuperEmployee>(transactionMock.Object, id, out user) :
					queryRunnerMock.Object.TryGet<User, Employee, SuperEmployee>(connectionMock.Object, id, out user));
				Assert.IsNull(user);
			}
			else
			{
				Assert.Throws<InvalidOperationException>(() => queryRunnerMock.Object.Get<User, Employee, SuperEmployee>(connectionMock.Object, id));
			}
		}
	}
}