using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib
{
	/// <summary>
	/// A post processing effect applied to a screen size image before it is shown on the screen
	/// </summary>
	public interface IMsgBoardFX
	{
		void Advance(double dt);
		void ApplyFX(MsgBoardImage image, MsgBoardDevice device);
	}
}
