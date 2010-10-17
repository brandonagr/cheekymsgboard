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
		void ApplyFX(MsgBoardImage image);
	}

	public class InverseFX : IMsgBoardFX
	{
		#region Declerations

		bool _on = false;
		double _time = 0.0;
		double _limit;


		#endregion
		#region Initialization & Instantiation

		public InverseFX(double limit)
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

		public void ApplyFX(MsgBoardImage image)
		{
			if (_on)
			{
				for (int y = 0; y < MsgBoardImage.Height; y++)
				{
					for (int x = 0; x < image.Width; x++)
					{
						image.Set(y, x, !image.Get(y, x));
					}
				}
			}
		}

		#endregion
	}
}
