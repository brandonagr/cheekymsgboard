using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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
}
