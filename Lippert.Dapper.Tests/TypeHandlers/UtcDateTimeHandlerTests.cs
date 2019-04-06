using System;
using System.Data;
using System.Linq;
using Dapper;
using Lippert.Dapper.TypeHandlers;
using Moq;
using NUnit.Framework;

namespace Lippert.Dapper.Tests.TypeHandlers
{
	[TestFixture]
	public class UtcDateTimeHandlerTests
	{
		[Test]
		public void TestParsesValue()
		{
			//--Arrange
			var dbValue = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

			//--Act
			var domainValue = new UtcDateTimeHandler().Parse(dbValue);

			//--Assert
			Assert.AreEqual(DateTimeKind.Utc, domainValue.Kind);
		}

		[Test]
		public void TestSetsValue()
		{
			//--Arrange
			var domainValue = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

			//--Mock
			var parameterMock = new Mock<IDbDataParameter>();
			parameterMock.SetupAllProperties();

			//--Act
			new UtcDateTimeHandler().SetValue(parameterMock.Object, domainValue);

			//--Assert
			Assert.AreEqual(domainValue, parameterMock.Object.Value);
			Assert.AreEqual(DateTimeKind.Unspecified, domainValue.Kind);
			Assert.AreEqual(DateTimeKind.Utc, ((DateTime)parameterMock.Object.Value).Kind);
			Assert.AreEqual(domainValue.Ticks, ((DateTime)parameterMock.Object.Value).Ticks);
		}

		[Test]
		public void TestRetrievesDataAsUtc()
		{
			//--Arrange
			var expectedRows = Enumerable.Range(0, 10)
				.Select(x => new FakeQueryResult
				{
					RegisteredDateUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
				}).ToList();

			//--Mock
			var connectionMock = new Mock<IDbConnection>();
			var commandMock = new Mock<IDbCommand>();
			var readerMock = new Mock<IDataReader>();
			connectionMock.Setup(x => x.CreateCommand())
				.Returns(commandMock.Object);
			commandMock.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>()))
				.Returns(readerMock.Object);
			readerMock.Setup(x => x.FieldCount)
				.Returns(1);
			readerMock.Setup(x => x.GetName(0))
				.Returns(nameof(FakeQueryResult.RegisteredDateUtc));
			var rowIndex = -1;
			readerMock.Setup(x => x.Read())
				.Returns(() => ++rowIndex < expectedRows.Count);
			readerMock.Setup(x => x[It.IsAny<int>()])
				.Returns((int i) => expectedRows[rowIndex].RegisteredDateUtc);

			//--Act
			SqlMapper.AddTypeHandler(typeof(DateTime), new UtcDateTimeHandler());
			var rows = connectionMock.Object.Query<FakeQueryResult>("This isn't even valid sql!", commandType: CommandType.StoredProcedure)
				.ToList();

			//--Assert
			Assert.AreEqual(10, rows.Count);
			Assert.IsTrue(rows.All(x => x.RegisteredDateUtc.Kind == DateTimeKind.Utc));
		}


		public class FakeQueryResult
		{
			public DateTime RegisteredDateUtc { get; set; }
		}
	}
}