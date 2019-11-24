using System.Collections.Generic;

namespace Lippert.Dapper.Contracts
{
	public interface IGridReaderWrapper : System.IDisposable
	{
		/// <summary>
		/// Has the underlying reader been consumed?
		/// </summary>
		public bool IsConsumed { get; }

		/// <summary>
		/// Read the next grid of results
		/// </summary>
		public IEnumerable<T> Read<T>(bool buffered = true);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second)> Read<TFirst, TSecond>(string splitOn = "id", bool buffered = true);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third)> Read<TFirst, TSecond, TThird>(string splitOn = "id", bool buffered = true);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth)> Read<TFirst, TSecond, TThird, TFourth>(string splitOn = "id", bool buffered = true);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth)> Read<TFirst, TSecond, TThird, TFourth, TFifth>(string splitOn = "id", bool buffered = true);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth)> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string splitOn = "id", bool buffered = true);
		/// <summary>
		/// Read multiple objects from a single record set on the grid
		/// </summary>
		public IEnumerable<(TFirst first, TSecond second, TThird third, TFourth fourth, TFifth fifth, TSixth sixth, TSeventh seventh)> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string splitOn = "id", bool buffered = true);
	}
}