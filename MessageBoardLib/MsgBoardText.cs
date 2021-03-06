﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace MessageBoardLib
{
	/// <summary>
	/// Handles rendering a string into a 
	/// </summary>
	public static class MsgBoardText
	{
		#region Declerations

		/// <summary>
		/// Font bitmap
		/// </summary>
		static Bitmap _imageData = null;

		/// <summary>
		/// Lookup of supported characters, tuple is posX in font bitmap, Width of character
		/// </summary>
		static Dictionary<char, Tuple<int, int>> _imageMetaData = null;

		#endregion
		#region Initialization & Instaitation

		static MsgBoardText()
		{
			_imageData = (Bitmap)Image.FromFile(@"Fonts\med_font.bmp");
			_imageMetaData = new Dictionary<char, Tuple<int, int>>();

			using (StreamReader reader = new StreamReader(File.OpenRead(@"Fonts\med_font_map.txt")))
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
		/// Render a single character at the specified location
		/// </summary>
		/// <param name="character"></param>
		/// <param name="y"></param>
		/// <param name="x"></param>
		/// <param name="forceWidth">If non zero, aligns the character to the right of the specified width</param>
		/// <param name="image"></param>
		internal static void Render(char character, int y, int x, int forceWidth, MsgBoardImage image)
		{
			Tuple<int, int> charData;
			if (!_imageMetaData.TryGetValue(character, out charData))
				return;

			if (forceWidth != 0)
			{
				x += forceWidth - charData.Item2;
			}

			RenderCharacter(x, charData, image, y);
		}

		/// <summary>
		/// Generate an Image from a given string text
		/// </summary>
		/// <param name="text"></param>
		/// <param name="forceWidth">If greater than 0, force the width of the produced image to be this width, aligning the text to the left</param>
		/// <returns></returns>
		public static MsgBoardImage Render(string text, int forceWidth = 0)
		{
			if (string.IsNullOrEmpty(text))
				throw new ArgumentOutOfRangeException("There is nothing to render!");

			// First iterate over string finding out the width of each character to render
			int totalWidth = 0;
			List<Tuple<int, int>> renderText = new List<Tuple<int, int>>();
			foreach (char c in text)
			{
				Tuple<int, int> posData;
				if (_imageMetaData.TryGetValue(c, out posData))
				{
					totalWidth += posData.Item2 + 1;
					renderText.Add(posData);
				}
			}

			MsgBoardImage image = new MsgBoardImage( ((forceWidth > 0) ? forceWidth: totalWidth) );

			int curPos = 0;
			foreach (var posData in renderText)
			{
				if (!RenderCharacter(curPos, posData, image))
					break;

				curPos += posData.Item2 + 1;
			}

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xPos"></param>
		/// <param name="charData"></param>
		/// <param name="image"></param>
		/// <returns>False if no more characters can fit in this image</returns>
		private static bool RenderCharacter(int xPos, Tuple<int, int> charData, MsgBoardImage image, int yOffset = 0)
		{
			lock (_imageData)
			{
				for (int y = 0; y < 7; y++)
				{
					int renderY = y + yOffset;

					if (renderY < 0 || renderY > 6)
						continue;

					for (int xOffset = 0; xOffset < charData.Item2; xOffset++)
					{
						if (image.Width <= xPos + xOffset)
							return false;

						Color pixel = _imageData.GetPixel(charData.Item1 + xOffset, y);
						bool pixelOn = (pixel.R == 0 && pixel.G == 0 && pixel.B == 0);
						image.Set(renderY, xPos + xOffset, pixelOn);
					}
				}
			}

			return true;
		}

		#endregion
	}
}
