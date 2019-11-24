using System.Collections.Generic;
using GridReader = Dapper.SqlMapper.GridReader;

namespace Lippert.Dapper
{
	public class GridReaderWrapper : Contracts.IGridReaderWrapper
	{
		private readonly GridReader _gridReader;

		internal GridReaderWrapper(GridReader gridReader)
		{
			_gridReader = gridReader;
		}

		/// <summary>
		/// Has the underlying reader been consumed?
		/// </summary>
		public bool IsConsumed => _gridReader.IsConsumed;

		public void Dispose() => _gridReader.Dispose();
		/// <summary>
		/// Read the next grid of results
		/// </summary>
		public IEnumerable<T> Read<T>(bool buffered = true) =>
			_gridReader.Read<T>(buffered);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second)> Read<TFirst, TSecond>(string splitOn = "id", bool buffered = true) =>
			_gridReader.Read((TFirst first, TSecond second) => (first, second), splitOn, buffered);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third)> Read<TFirst, TSecond, TThird>(string splitOn = "id", bool buffered = true) =>
			_gridReader.Read((TFirst first, TSecond second, TThird third) => (first, second, third), splitOn, buffered);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth)> Read<TFirst, TSecond, TThird, TFourth>(string splitOn = "id", bool buffered = true) =>
			_gridReader.Read((TFirst first, TSecond second, TThird third, TFourth fourth) => (first, second, third, fourth), splitOn, buffered);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth)> Read<TFirst, TSecond, TThird, TFourth, TFifth>(string splitOn = "id", bool buffered = true) =>
			_gridReader.Read((TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth) => (first, second, third, fourth, fifth), splitOn, buffered);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth)> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string splitOn = "id", bool buffered = true) =>
			_gridReader.Read((TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth) => (first, second, third, fourth, fifth, sixth), splitOn, buffered);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth, TSeventh seventh)> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string splitOn = "id", bool buffered = true) =>
			_gridReader.Read((TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth, TSeventh seventh) => (first, second, third, fourth, fifth, sixth, seventh), splitOn, buffered);
	}
}