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

	public static class MsgBoardText
	{
		#region Declerations

		private static Bitmap _imageData = null;
		private static Dictionary<char, Tuple<int, int>> _imageMetaData = null;

		#endregion
		#region Initialization & Instaitation

		private static void Initialize()
		{
			if (_imageData != null)
				return;

			_imageData = (Bitmap)Image.FromFile(@"Fonts\small_font.bmp");
			_imageMetaData = new Dictionary<char,Tuple<int,int>>();

			using (StreamReader reader = new StreamReader(File.OpenRead(@"Fonts\small_font_map.txt")))
			{
				reader.ReadLine(); //eat header line

				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();

					string[] parts = line.Split('\t');

					if (parts.Length != 3 || parts[0].Length != 1)
						throw new InvalidOperationException("Invalid Font Map!");

					_imageMetaData.Add(parts[0][0], Tuple.Create(int.Parse(parts[1]), int.Parse(parts[2])));
				}
			}
		}

		#endregion
		#region Public Methods

		/// <summary>
		/// Generate an Image from a given string text
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static MsgBoardImage Render(string text)
		{
			Initialize();

			if (string.IsNullOrEmpty(text))
				throw new ArgumentOutOfRangeException("There is nothing to render!");

			// First iterate over string finding out the width of each character to render
			int totalWidth = 0;
			List<Tuple<int, int>> renderText = new List<Tuple<int, int>>();
			foreach (char c in text)
			{
				Tuple<int,int> posData;
				if (_imageMetaData.TryGetValue(c, out posData))
				{
					totalWidth += posData.Item2 + 1;
					renderText.Add(posData);
				}
			}

			MsgBoardImage image = new MsgBoardImage(totalWidth);

			int curPos = 1;
			foreach (var posData in renderText)
			{
				RenderCharacter(curPos, posData, image);

				curPos += posData.Item2 + 1;
			}

			return image;
		}

		private static void RenderCharacter(int xPos, Tuple<int,int> charData, MsgBoardImage image)
		{
			for (int y = 0; y < 6; y++)
			{
				for (int xOffset = 0; xOffset < charData.Item2; xOffset++)
				{
					Color pixel = _imageData.GetPixel(charData.Item1 + xOffset, y);
					bool pixelOn = (pixel.R == 0 && pixel.G == 0 && pixel.B == 0);
					image.Set(y+1, xPos + xOffset, pixelOn);
				}
			}
		}

		#endregion
	}
}
