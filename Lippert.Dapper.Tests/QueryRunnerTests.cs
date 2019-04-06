using System;
using System.Data;
using System.Linq;
using Dapper;
using Lippert.Core.Configuration;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Dapper.Tests.TestSchema;
using Lippert.Dapper.Tests.TestSchema.Contracts;
using Moq;
using NUnit.Framework;

namespace Lippert.Dapper.Tests
{
	[TestFixture]
	public class QueryRunnerTests
	{
		private Guid _currentUserId;

		private IDbConnection _dbConnectionMock;
		private Mock<Contracts.IDapperWrapper> _dapperMock;
		private QueryRunnerUnderTest _queryRunner;

		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = "Lippert";

		private string[] SplitQuery(string query) =>
#if TARGET_FRAMEWORK_NET471
			query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
#else
			query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
#endif

		[SetUp]
		public void SetUp()
		{
			//--Arrange
			TestSchema.TableMaps.ColumnValueProviders.ClaimsProvider.UserClaims.UserId = _currentUserId = Guid.NewGuid();

			//--Mock
			_dbConnectionMock = new Mock<IDbConnection>().Object;
			_dapperMock = new Mock<Contracts.IDapperWrapper>();
			_queryRunner = new QueryRunnerUnderTest(_dapperMock.Object);
		}


		[Test]
		public void TestSelectBySingleId([Values(0, 1, 2)] int idParameter)
		{
			//--Arrange
			var id = Guid.NewGuid();
			object key;
			switch (idParameter)
			{
				case 0:
					key = id;
					break;
				case 1:
					key = new { Id = id };
					break;
				case 2:
					key = new GuidIdentifier { Id = id };
					break;
				default:
					throw new Exception();
			}

			//--Mock
			_dapperMock.Setup(x => x.Query<User>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("select [Id]", lines[0]);
					Assert.AreEqual("from [User]", lines[1]);
					Assert.AreEqual("where [Id] = @Id", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(1, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(User.Id));
					Assert.AreEqual(id, dynamicParam.Get<Guid>(nameof(User.Id)));

					return Enumerable.Range(0, 1).Select(x => new User { Id = id });
				});

			//--Act
			var user = _queryRunner.Select<User>(_dbConnectionMock, key);

			//--Assert
			Assert.AreEqual(id, user.Id);
			_dapperMock.Verify(x => x.Query<User>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestSelectByComponentKey([Values(0, 1, 2)] int idParameter)
		{
			//--Arrange
			var clientId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			object key;
			switch (idParameter)
			{
				case 0:
					key = new ClientUserIdentifier(clientId, userId);
					break;
				case 1:
					key = new { ClientId = clientId, UserId = userId };
					break;
				case 2:
					key = new ClientUser { ClientId = clientId, UserId = userId };
					break;
				default:
					throw new Exception();
			}

			//--Mock
			_dapperMock.Setup(x => x.Query<ClientUser>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("select [ClientId], [UserId], [IsActive], [Role]", lines[0]);
					Assert.AreEqual("from [Client_User]", lines[1]);
					Assert.AreEqual("where [ClientId] = @ClientId and [UserId] = @UserId", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(2, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.ClientId));
					Assert.AreEqual(clientId, dynamicParam.Get<Guid>(nameof(ClientUser.ClientId)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.UserId));
					Assert.AreEqual(userId, dynamicParam.Get<Guid>(nameof(ClientUser.UserId)));

					return Enumerable.Range(0, 1).Select(x => new ClientUser { ClientId = clientId, UserId = userId });
				});

			//--Act
			var clientUser = _queryRunner.Select<ClientUser>(_dbConnectionMock, key);

			//--Assert
			Assert.AreEqual(clientId, clientUser.ClientId);
			Assert.AreEqual(userId, clientUser.UserId);
			_dapperMock.Verify(x => x.Query<ClientUser>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestSelectAll()
		{
			//--Mock
			_dapperMock.Setup(x => x.Query<InheritingComponent>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(2, lines.Length);
					Assert.AreEqual("select [Id], [BaseId], [Category], [Cost]", lines[0]);
					Assert.AreEqual("from [InheritingComponent]", lines[1]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(0, dynamicParam.ParameterNames.Count());

					return Enumerable.Range(0, 100).Select(x => new InheritingComponent { Id = Guid.NewGuid(), BaseId = Guid.NewGuid() });
				});

			//--Act
			var inheritingComponents = _queryRunner.Select<InheritingComponent>(_dbConnectionMock).ToList();

			//--Assert
			Assert.AreEqual(100, inheritingComponents.Count);
			_dapperMock.Verify(x => x.Query<InheritingComponent>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}


		[Test]
		public void TestInsertWithGeneratedKey()
		{
			//--Arrange
			var toInsert = new BaseRecord { Name = "Foo" };
			var expectedId = Guid.NewGuid();

			//--Mock
			_dapperMock.Setup(x => x.Query<BaseRecord>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("insert into [BaseRecord]([Name])", lines[0]);
					Assert.AreEqual("ouput inserted.[Id]", lines[1]);
					Assert.AreEqual("values(@Name)", lines[2]);

					var baseRecord = param as BaseRecord;
					Assert.IsNotNull(baseRecord);
					Assert.AreEqual(Guid.Empty, baseRecord.Id);
					Assert.AreEqual("Foo", baseRecord.Name);

					return Enumerable.Range(0, 1).Select(x => new BaseRecord { Id = expectedId });
				});

			//--Act
			_queryRunner.Insert(_dbConnectionMock, toInsert);

			//--Assert
			Assert.AreEqual(expectedId, toInsert.Id);
			_dapperMock.Verify(x => x.Query<BaseRecord>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestInsertAssignedKeyWithGeneratedFields()
		{
			//--Arrange
			var toInsert = new Employee { UserId = Guid.NewGuid(), CompanyId = Guid.NewGuid() };
			var now = DateTime.UtcNow;

			//--Mock
			_dapperMock.Setup(x => x.Query<Employee>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, bool buffered, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("insert into [Employee]([UserId], [CompanyId], [CreatedByUserId])", lines[0]);
					Assert.AreEqual("ouput inserted.[CreatedDateUtc]", lines[1]);
					Assert.AreEqual("values(@UserId, @CompanyId, @CreatedByUserId)", lines[2]);

					var employee = param as Employee;
					Assert.IsNotNull(employee);
					Assert.AreNotEqual(Guid.Empty, employee.UserId);
					Assert.AreNotEqual(Guid.Empty, employee.CompanyId);
					Assert.AreEqual(_currentUserId, employee.CreatedByUserId);
					Assert.AreEqual(default(DateTime), employee.CreatedDateUtc);

					return Enumerable.Range(0, 1).Select(x => new Employee { CreatedDateUtc = now });//--Generated by the database
				});

			//--Act
			_queryRunner.Insert(_dbConnectionMock, toInsert);

			//--Assert
			Assert.AreEqual(now, toInsert.CreatedDateUtc);
			_dapperMock.Verify(x => x.Query<Employee>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestInsertAssignedKey()
		{
			//--Arrange
			var toInsert = new ClientUser { ClientId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsActive = true, Role = "Admin" };

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(2, lines.Length);
					Assert.AreEqual("insert into [Client_User]([ClientId], [UserId], [IsActive], [Role])", lines[0]);
					Assert.AreEqual("values(@ClientId, @UserId, @IsActive, @Role)", lines[1]);

					var clientUser = param as ClientUser;
					Assert.IsNotNull(clientUser);
					Assert.AreNotEqual(Guid.Empty, clientUser.ClientId);
					Assert.AreNotEqual(Guid.Empty, clientUser.UserId);
					Assert.IsTrue(clientUser.IsActive);
					Assert.AreEqual("Admin", clientUser.Role);

					return 1;
				});

			//--Act
			_queryRunner.Insert(_dbConnectionMock, toInsert);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}


		[Test]
		public void TestUpdateFullRecordBySingleId()
		{
			//--Arrange
			var toUpate = new Client { Name = "Some Name", IsActive = false };
			(toUpate as IGuidIdentifier).Id = Guid.NewGuid();
			var now = DateTime.UtcNow;

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client]", lines[0]);
					Assert.AreEqual("set [ModifiedByUserId] = @ModifiedByUserId, [ModifiedDateUtc] = @ModifiedDateUtc, [Name] = @Name, [IsActive] = @IsActive", lines[1]);
					Assert.AreEqual("where [Id] = @Id", lines[2]);

					var client = param as Client;
					Assert.IsNotNull(client);
					Assert.AreNotEqual(Guid.Empty, (client as IGuidIdentifier).Id);
					Assert.IsNotNull(client.Name);
					Assert.AreEqual(_currentUserId, client.ModifiedByUserId);
					Assert.LessOrEqual(now, client.ModifiedDateUtc);
					Assert.GreaterOrEqual(now.AddSeconds(2), client.ModifiedDateUtc);//--A few milliseconds will have passed before the value provider kicks in
					Assert.IsFalse(client.IsActive);

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, toUpate);

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestUpdateFullRecordByComponentKey()
		{
			//--Arrange
			var toUpate = new ClientUser { ClientId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsActive = true, Role = "Doctor" };

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client_User]", lines[0]);
					Assert.AreEqual("set [IsActive] = @IsActive, [Role] = @Role", lines[1]);
					Assert.AreEqual("where [ClientId] = @ClientId and [UserId] = @UserId", lines[2]);

					var clientUser = param as ClientUser;
					Assert.IsNotNull(clientUser);
					Assert.AreNotEqual(Guid.Empty, clientUser.ClientId);
					Assert.AreNotEqual(Guid.Empty, clientUser.UserId);
					Assert.IsTrue(clientUser.IsActive);
					Assert.AreEqual("Doctor", clientUser.Role);

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, toUpate);

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestUpdatePartialRecordBySingleId()
		{
			//--Arrange
			var toUpate = new Client { Name = "Some Name", IsActive = false };
			(toUpate as IGuidIdentifier).Id = Guid.NewGuid();
			var now = DateTime.UtcNow;

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client]", lines[0]);
					Assert.AreEqual("set [IsActive] = @IsActive, [ModifiedByUserId] = @ModifiedByUserId, [ModifiedDateUtc] = @ModifiedDateUtc", lines[1]);
					Assert.AreEqual("where [Id] = @Id", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(4, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(IGuidIdentifier.Id));
					Assert.AreEqual((toUpate as IGuidIdentifier).Id, dynamicParam.Get<Guid>(nameof(IGuidIdentifier.Id)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(Client.IsActive));
					Assert.AreEqual(toUpate.IsActive, dynamicParam.Get<bool>(nameof(Client.IsActive)));

					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(IEditFields.ModifiedByUserId));
					Assert.AreEqual(_currentUserId, dynamicParam.Get<Guid>(nameof(IEditFields.ModifiedByUserId)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(IEditFields.ModifiedDateUtc));
					Assert.LessOrEqual(now, dynamicParam.Get<DateTime>(nameof(IEditFields.ModifiedDateUtc)));
					Assert.GreaterOrEqual(now.AddSeconds(2), dynamicParam.Get<DateTime>(nameof(IEditFields.ModifiedDateUtc)));//--A few milliseconds will have passed before the value provider kicks in

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, new UpdateBuilder<Client>().Set(x => x.IsActive, false).Key(toUpate));

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestUpdatePartialRecordById()
		{
			//--Arrange
			var clientId = Guid.NewGuid();
			var isActive = false;
			var now = DateTime.UtcNow;

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client]", lines[0]);
					Assert.AreEqual("set [IsActive] = @IsActive, [ModifiedByUserId] = @ModifiedByUserId, [ModifiedDateUtc] = @ModifiedDateUtc", lines[1]);
					Assert.AreEqual("where [Id] = @Id", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(4, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(IGuidIdentifier.Id));
					Assert.AreEqual(clientId, dynamicParam.Get<Guid>(nameof(IGuidIdentifier.Id)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(Client.IsActive));
					Assert.AreEqual(isActive, dynamicParam.Get<bool>(nameof(Client.IsActive)));

					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(IEditFields.ModifiedByUserId));
					Assert.AreEqual(_currentUserId, dynamicParam.Get<Guid>(nameof(IEditFields.ModifiedByUserId)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(IEditFields.ModifiedDateUtc));
					Assert.LessOrEqual(now, dynamicParam.Get<DateTime>(nameof(IEditFields.ModifiedDateUtc)));
					Assert.GreaterOrEqual(now.AddSeconds(2), dynamicParam.Get<DateTime>(nameof(IEditFields.ModifiedDateUtc)));//--A few milliseconds will have passed before the value provider kicks in

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, new UpdateBuilder<Client>().Set(x => x.IsActive, isActive).Key(clientId));

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestUpdatePartialRecordByComponentKey()
		{
			//--Arrange
			var toUpate = new ClientUser { ClientId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsActive = true, Role = "Don't Update" };

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client_User]", lines[0]);
					Assert.AreEqual("set [IsActive] = @IsActive", lines[1]);
					Assert.AreEqual("where [ClientId] = @ClientId and [UserId] = @UserId", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(3, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.ClientId));
					Assert.AreEqual(toUpate.ClientId, dynamicParam.Get<Guid>(nameof(ClientUser.ClientId)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.UserId));
					Assert.AreEqual(toUpate.UserId, dynamicParam.Get<Guid>(nameof(ClientUser.UserId)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.IsActive));
					Assert.IsFalse(dynamicParam.Get<bool>(nameof(ClientUser.IsActive)));

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, new UpdateBuilder<ClientUser>().Set(x => x.IsActive, false).Key(toUpate));

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestUpdatePartialRecordByNonKey()
		{
			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client_User]", lines[0]);
					Assert.AreEqual("set [Role] = @Role", lines[1]);
					Assert.AreEqual("where [IsActive] = @IsActive", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(2, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.Role));
					Assert.AreEqual("Update", dynamicParam.Get<string>(nameof(ClientUser.Role)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.IsActive));
					Assert.IsTrue(dynamicParam.Get<bool>(nameof(ClientUser.IsActive)));

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, new UpdateBuilder<ClientUser>().Set(x => x.Role, "Update").Filter(x => x.IsActive, true));

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestUpdateNonKeyFieldByFilteringToSameField()
		{
			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(3, lines.Length);
					Assert.AreEqual("update [Client_User]", lines[0]);
					Assert.AreEqual("set [Role] = @_Role", lines[1]);
					Assert.AreEqual("where [Role] = @Role", lines[2]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(2, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.Role));
					Assert.AreEqual("Old Role", dynamicParam.Get<string>(nameof(ClientUser.Role)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, "_Role");
					Assert.AreEqual("New Role", dynamicParam.Get<string>("_Role"));

					return 1;
				});

			//--Act
			var updates = _queryRunner.Update(_dbConnectionMock, new UpdateBuilder<ClientUser>().Set(x => x.Role, "New Role").Filter(x => x.Role, "Old Role"));

			//--Assert
			Assert.AreEqual(1, updates);
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}


		[Test]
		public void TestDeleteBySingleId([Values(0, 1, 2)] int idParameter)
		{
			//--Arrange
			var id = Guid.NewGuid();
			object key;
			switch (idParameter)
			{
				case 0:
					key = id;
					break;
				case 1:
					key = new { Id = id };
					break;
				case 2:
					key = new GuidIdentifier { Id = id };
					break;
				default:
					throw new Exception();
			}

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(2, lines.Length);
					Assert.AreEqual("delete from [User]", lines[0]);
					Assert.AreEqual("where [Id] = @Id", lines[1]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(1, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(User.Id));
					Assert.AreEqual(id, dynamicParam.Get<Guid>(nameof(User.Id)));

					return 1;
				});

			//--Act
			_queryRunner.Delete<User>(_dbConnectionMock, key);

			//--Assert
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestDeleteByComponentKey([Values(0, 1, 2)] int idParameter)
		{
			//--Arrange
			var clientId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			object key;
			switch (idParameter)
			{
				case 0:
					key = new ClientUserIdentifier(clientId, userId);
					break;
				case 1:
					key = new { ClientId = clientId, UserId = userId };
					break;
				case 2:
					key = new ClientUser { ClientId = clientId, UserId = userId };
					break;
				default:
					throw new Exception();
			}

			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(2, lines.Length);
					Assert.AreEqual("delete from [Client_User]", lines[0]);
					Assert.AreEqual("where [ClientId] = @ClientId and [UserId] = @UserId", lines[1]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(2, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.ClientId));
					Assert.AreEqual(clientId, dynamicParam.Get<Guid>(nameof(ClientUser.ClientId)));
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(ClientUser.UserId));
					Assert.AreEqual(userId, dynamicParam.Get<Guid>(nameof(ClientUser.UserId)));

					return 1;
				});

			//--Act
			_queryRunner.Delete<ClientUser>(_dbConnectionMock, key);

			//--Assert
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}

		[Test]
		public void TestDeleteBySingleNonKey([Values(0, 1, 2)] int idParameter)
		{
			//--Mock
			_dapperMock.Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()))
				.Returns((IDbConnection conn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType) =>
				{
					//--Assert
					Console.WriteLine(sql);
					var lines = SplitQuery(sql);
					Assert.AreEqual(2, lines.Length);
					Assert.AreEqual("delete from [InheritingComponent]", lines[0]);
					Assert.AreEqual("where [Category] = @Category", lines[1]);

					var dynamicParam = param as DynamicParameters;
					Assert.IsNotNull(dynamicParam);
					Assert.AreEqual(1, dynamicParam.ParameterNames.Count());
					CollectionAssert.Contains(dynamicParam.ParameterNames, nameof(InheritingComponent.Category));
					Assert.AreEqual("Delete Me", dynamicParam.Get<string>(nameof(InheritingComponent.Category)));

					return 1;
				});

			//--Act
			_queryRunner.Delete(_dbConnectionMock, new PredicateBuilder<InheritingComponent>().Filter(x => x.Category, "Delete Me"));

			//--Assert
			_dapperMock.Verify(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType>()));
		}



		public class QueryRunnerUnderTest : QueryRunner
		{
			public QueryRunnerUnderTest(Contracts.IDapperWrapper dapperWrapper)
			{
				DapperWrapper = dapperWrapper;
			}
		}

		public class GuidIdentifier
		{
			public Guid Id { get; set; }
		}
		public struct ClientUserIdentifier
		{
			public ClientUserIdentifier(Guid clientId, Guid userId)
			{
				ClientId = clientId;
				UserId = userId;
			}

			public Guid ClientId { get; }
			public Guid UserId { get; }
		}
	}
}