using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib.Screens
{
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
