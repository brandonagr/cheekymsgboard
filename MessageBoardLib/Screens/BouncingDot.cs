using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib.Screens
{
	/// <summary>
	/// Displays a dot that bounces around the screen
	/// </summary>
	public class BouncingDot : IMsgBoardScreen
	{
		#region Declerations

		int _lastPosX, _lastPosY;
		double _curX, _curY;
		double _speedX, _speedY;

		double _changeDirCounter = 0.0;

		#endregion
		#region Initialization & Instantiation

		public BouncingDot()
		{
			_lastPosY = int.MinValue;
			_lastPosX = int.MinValue;

			_curY = 5.0;
			_curX = 5.0;

			GenerateRandomDirection();
		}

		#endregion
		#region Private Methods

		private void GenerateRandomDirection(double speed = 15.0)//0.125)
		{
			double dir = new Random((int)DateTime.Now.Ticks).NextDouble() * 360.0;

			_speedX = Math.Cos(dir) * speed;
			_speedY = Math.Sin(dir) * speed;
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
			_changeDirCounter += dt;

			if (_changeDirCounter > 90.0)
			{
				GenerateRandomDirection();
				_changeDirCounter = 0.0;
			}

			_curX += _speedX * dt;
			_curY += _speedY * dt;

			if (_curX < 0.0)
			{
				_curX = 0.0;
				if (_speedX < 0)
					_speedX = -_speedX;
			}
			else if (_curX >= 21.0)
			{
				_curX = 20.99999;
				if (_speedX > 0)
					_speedX = -_speedX;
			}

			if (_curY < 0.0)
			{
				_curY = 0.0;
				if (_speedY < 0)
					_speedY = -_speedY;
			}
			else if (_curY >= 7.0)
			{
				_curY = 6.99999;
				if (_speedY > 0)
					_speedY = -_speedY;
			}

			int drawnX = (int)Math.Floor(_curX);
			int drawnY = (int)Math.Floor(_curY);

			return (drawnX != _lastPosX || drawnY != _lastPosY);
		}

		public MsgBoardImage GetCurrentScreenImage()
		{
			MsgBoardImage image = new MsgBoardImage(21);

			image.Set((int)Math.Floor(_curY), (int)Math.Floor(_curX), true);

			return image;
		}

		#endregion
	}
}
