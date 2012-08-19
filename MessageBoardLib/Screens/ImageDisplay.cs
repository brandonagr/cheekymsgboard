using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib.Screens
{
	/// <summary>
	/// A view of an image that doesn't move, just clips it to the available space
	/// </summary>
	public class ImageDisplay : IMsgBoardScreen
	{
		#region Declarations

		bool displayedOnce = false;
		MsgBoardImage _displayImage;

		#endregion
		#region Initialization & Instantiation

		public ImageDisplay(MsgBoardImage sourceImage)
		{
			_displayImage = new MsgBoardImage(21);

			for (int y = 0; y < MsgBoardImage.Height; y++)
			{
				for (int x = 0; x < _displayImage.Width; x++)
				{
					if (x < 0 || x >= sourceImage.Width)
						_displayImage.Set(y, x, false);
					else
						_displayImage.Set(y, x, sourceImage.Get(y, x));
				}
			}
		}

		#endregion
		#region IMsgBoardScreen Members

		public bool Advance(double dt)
		{
			if (displayedOnce)
			{
				return false;
			}
			else
			{
				displayedOnce = true;
				return true;
			}
		}

		public MsgBoardImage GetCurrentScreenImage()
		{
			return _displayImage;
		}

		#endregion
	}
}
