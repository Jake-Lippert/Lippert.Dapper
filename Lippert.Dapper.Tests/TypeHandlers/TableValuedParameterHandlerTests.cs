using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Lippert.Core.Collections.Extensions;
using Lippert.Dapper.TypeHandlers;
using Moq;
using NUnit.Framework;

namespace Lippert.Dapper.Tests.TypeHandlers
{
	[TestFixture]
	public class TableValuedParameterHandlerTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			SqlMapper.AddTypeHandler(typeof(List<TVPTestModel>), (SqlMapper.ITypeHandler)Activator.CreateInstance(typeof(TableValuedParameterHandler<TVPTestModel>)));
			SqlMapper.AddTypeHandler(typeof(TVPTestModel[]), (SqlMapper.ITypeHandler)Activator.CreateInstance(typeof(TableValuedParameterHandler<TVPTestModel>)));
		}

		[Test]
		public void TestSetsValue()
		{
			//--Arrange
			var domainCollection = Enumerable.Range(0, 10)
				.Select(x => BuildDomainModel())
				.ToList();

			//--Mock
			var parameterMock = new Mock<IDbDataParameter>();
			parameterMock.SetupAllProperties();

			//--Act
			new TableValuedParameterHandler<TVPTestModel>().SetValue(parameterMock.Object, domainCollection);

			//--Assert
			Assert.IsInstanceOf<DataTable>(parameterMock.Object.Value);
			var dataTable = (DataTable)parameterMock.Object.Value;
			Assert.AreEqual(8, dataTable.Columns.Count);
			Assert.AreEqual(domainCollection.Count, dataTable.Rows.Count);
			foreach (var (domain, index) in domainCollection.Indexed())
			{
				Assert.AreEqual(domain.Guid, dataTable.Rows[index][nameof(TVPTestModel.Guid)]);
			}
		}

		[Test]
		public void TestHandlerFoundForList()
		{
			//--Arrange
			var domainCollection = Enumerable.Range(0, 10)
				.Select(x => BuildDomainModel())
				.ToList();

			//--Act
#pragma warning disable CS0618 // Type or member is obsolete
			var dbType = SqlMapper.LookupDbType(domainCollection.GetType(), nameof(domainCollection), true, out var handler);
#pragma warning restore CS0618 // Type or member is obsolete

			//--Assert
			Assert.AreEqual(typeof(TableValuedParameterHandler<TVPTestModel>), handler.GetType());
			Assert.AreEqual(DbType.Object, dbType);
		}

		[Test]
		public void TestHandlerFoundForArray()
		{
			//--Arrange
			var domainCollection = Enumerable.Range(0, 10)
				.Select(x => BuildDomainModel())
				.ToArray();

			//--Act
#pragma warning disable CS0618 // Type or member is obsolete
			var dbType = SqlMapper.LookupDbType(domainCollection.GetType(), "n/a", true, out var handler);
#pragma warning restore CS0618 // Type or member is obsolete

			//--Assert
			Assert.AreEqual(typeof(TableValuedParameterHandler<TVPTestModel>), handler.GetType());
			Assert.AreEqual(DbType.Object, dbType);
		}

		[Test]
		public void TestCantGetValue()
		{
			//--Arrange
			var domainCollection = Enumerable.Range(0, 10)
				.Select(x => BuildDomainModel())
				.ToList();

			//--Mock
			var parameterMock = new Mock<IDbDataParameter>();
			parameterMock.SetupAllProperties();

			//--Act
			var tvpHandler = new TableValuedParameterHandler<TVPTestModel>();
			Assert.Throws<InvalidOperationException>(() => tvpHandler.Parse(parameterMock.Object));
		}

		private TVPTestModel BuildDomainModel() => new TVPTestModel
		{
			Guid = Guid.NewGuid()
		};


		public class TVPTestModel
		{
			public bool Boolean { get; set; }
			public byte Byte { get; set; }
			public short Short { get; set; }
			public int Int { get; set; }
			public long Long { get; set; }
			public Guid Guid { get; set; }
			public string String { get; set; }
			public DateTime DateTime { get; set; }
		}
	}
}