using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace MessageBoardLib
{
	public class MsgBoardImage
	{
		#region Declerations

		bool[,] _data;
		int _width;
		public const int Height = 7;

		#endregion
		#region Initialization & Instantiation

		public MsgBoardImage(int width)
		{
			_width = width;
			_data = new bool[Height, width];
		}

		public MsgBoardImage(string text)
		{

		}

		#endregion
		#region Public Methods

		/// <summary>
		/// Clear all of the bits in this range
		/// </summary>
		/// <param name="beginXInclusive"></param>
		/// <param name="endXInclusive"></param>
		public void Clear(int beginXInclusive, int endXInclusive)
		{
			for (int x = beginXInclusive; x <= endXInclusive; x++)
			{
				_data[0, x] = false;
				_data[1, x] = false;
				_data[2, x] = false;
				_data[3, x] = false;
				_data[4, x] = false;
				_data[5, x] = false;
				_data[6, x] = false;
			}
		}

		public void Set(int y, int x, bool on)
		{
			if (y < 0 || y >= Height || x < 0 || x > _width)
				throw new ArgumentOutOfRangeException("Invalid location");

			_data[y, x] = on;
		}

		public bool Get(int y, int x)
		{
			if (y < 0 || y >= Height || x < 0 || x >= _width)
				throw new ArgumentOutOfRangeException("Invalid location");

			return _data[y,x];
		}

		public void WriteToConsole()
		{
			StringBuilder builder = new StringBuilder();

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					builder.Append(_data[y, x] ? 'X' : '_');
				}
				builder.AppendLine();
			}

			Console.Write(builder.ToString());
		}

		#endregion
		#region Properties

		public int Width
		{
			get { return _width; }
		}

		#endregion
	}
}
