using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib.FX
{
	/// <summary>
	/// Cause the screen to invert what is on / off
	/// </summary>
	public class Inverse : IMsgBoardFX
	{
		#region Declerations

		bool _on = false;
		double _time = 0.0;
		double _limit;


		#endregion
		#region Initialization & Instantiation

		public Inverse(double limit)
		{
			_limit = limit;
		}

		#endregion
		#region IMsgBoardFX Members

		public void Advance(double dt)
		{
			_time += dt;

			if (_time > _limit)
			{
				_on = !_on;
				_time -= _limit;
			}
		}

		public void ApplyFX(MsgBoardImage image, MsgBoardDevice device)
		{
			if (_on)
			{
				device.BrightnessLevel = MsgBoardDevice.Brightness.BRIGHT;

				for (int y = 0; y < MsgBoardImage.Height; y++)
				{
					for (int x = 0; x < image.Width; x++)
					{
						image.Set(y, x, !image.Get(y, x));
					}
				}
			}
			else
				device.BrightnessLevel = MsgBoardDevice.Brightness.DIM;
		}

		#endregion
	}
}
