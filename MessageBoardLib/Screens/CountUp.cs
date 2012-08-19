using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBoardLib.FX;

namespace MessageBoardLib.Screens
{
	/// <summary>
	/// Display a clock on the screen, showing the time to the nearest seccond
	/// </summary>
	public class CountUp : IMsgBoardScreen
	{
		#region Declerations

		MsgBoardAsyncDriver _board;
		double _seconds;
		string _lastDisplayed = "0.0";

		#endregion
		#region Initialization & Instantiation

		public CountUp(MsgBoardAsyncDriver board, double startingSeconds = 0.0)
		{
			_board = board;
			_seconds = startingSeconds;
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
			_seconds += dt;

			if (_seconds > 17.5 && _board != null)
			{
				_board.SetFX(new Inverse(0.4));
				_board = null;
			}

			var displayString = string.Format("{0:0.0}", _seconds);

			if (displayString != _lastDisplayed)
			{
				_lastDisplayed = displayString;
				return true;
			}
			else
			{
				return false;
			}
		}

		public MsgBoardImage GetCurrentScreenImage()
		{
			return MsgBoardText.Render(_lastDisplayed, 21);
		}

		#endregion
	}
}
