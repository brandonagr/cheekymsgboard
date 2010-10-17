using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib
{
	public interface IMsgBoardScreen
	{
		/// <summary>
		/// In case the screen needs to known about time changes
		/// </summary>
		/// <param name="dt">Number of seconds since this method was last called</param>
		/// <returns>True if a new Image needs to be rendered, false if the previous image is still valid and can be used to refresh the screen</returns>
		bool Advance(double dt);

		/// <summary>
		/// Returns a 21x7 image that needs
		/// </summary>
		/// <returns></returns>
		MsgBoardImage GetCurrentScreenImage();
	}


	/// <summary>
	/// A view of an image that is displayed on the MessageBoard itself, this handles scrolling a large image across the viewable surface
	/// </summary>
	public class ImageScroller : IMsgBoardScreen
	{
		#region Enums

		/// <summary>
		/// How it handles scrolling
		/// </summary>
		public enum ScrollType
		{
			NO_WRAP,
			WRAP,
		}

		#endregion
		#region Declerations

		MsgBoardImage _sourceImage;

		double _currentXOffset;

		double _scrollSpeed;
		ScrollType _scrollType;

		#endregion
		#region Initialization & Instantiation

		public ImageScroller(MsgBoardImage image, double scrollSpeed, ScrollType scrollType)
		{
			_sourceImage = image;
			_currentXOffset = -21.0;
			_scrollSpeed = scrollSpeed;
			_scrollType = scrollType;
		}

		#endregion
		#region Implement IMsgBoardScreen Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public bool Advance(double dt)
		{
			int currentX = (int)_currentXOffset;

			_currentXOffset += _scrollSpeed * dt;

			int newX = (int)_currentXOffset;

			if (_scrollType == ScrollType.WRAP && newX >= (_sourceImage.Width))
			{
				_currentXOffset = -21.0;
				newX = 0;
			}

			return (currentX != newX) && (newX < (_sourceImage.Width));
		}

		public MsgBoardImage GetCurrentScreenImage()
		{
			MsgBoardImage image = new MsgBoardImage(21);
			int currentOffset = (int)_currentXOffset;

			for (int y = 0; y < MsgBoardImage.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					int curX = x + currentOffset;

					if (curX < 0 || curX >= _sourceImage.Width)
						image.Set(y, x, false);
					else
						image.Set(y, x, _sourceImage.Get(y, curX));
				}
			}

			return image;
		}

		#endregion
	}

	/// <summary>
	/// Display a clock on the screen, showing the time to the nearest seccond
	/// </summary>
	public class Clock : IMsgBoardScreen
	{
		#region Declerations

		DateTime _lastTimeDisplayed;

		#endregion
		#region Initialization & Instantiation

		public Clock()
		{
			_lastTimeDisplayed = DateTime.MinValue;
		}

		#endregion
		#region Implement IMsgBoardScreen Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public bool Advance(double dt)
		{
			DateTime currentTime = DateTime.Now;

			return ((currentTime - _lastTimeDisplayed).TotalMinutes > 1.0);
		}

		public MsgBoardImage GetCurrentScreenImage()
		{
			_lastTimeDisplayed = DateTime.Now;

			string timeToShow = _lastTimeDisplayed.ToString("HH:mm");

			return MsgBoardText.Render(timeToShow, 21);
		}

		#endregion
	}
}
