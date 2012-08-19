using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib.Screens
{
	/// <summary>
	/// Displays a dot that changes position occasionally
	/// </summary>
	public class RandomDot : IMsgBoardScreen
	{
		#region Declerations

		int _lastPosX, _lastPosY;

		double _changePosCounter = 0.0;

		#endregion
		#region Initialization & Instantiation

		public RandomDot()
		{
			GenerateRandomPosition();
		}

		#endregion
		#region Private Methods

		private void GenerateRandomPosition()
		{
			Random rand = new Random((int)DateTime.Now.Ticks);//.NextDouble() * 360.0;

			_lastPosX = rand.Next(21);
			_lastPosY = rand.Next(7);
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
			_changePosCounter += dt;

			if (_changePosCounter > 5.0)
			{
				GenerateRandomPosition();
				_changePosCounter = 0.0;

				return true;
			}
			else
				return false;
		}

		public MsgBoardImage GetCurrentScreenImage()
		{
			MsgBoardImage image = new MsgBoardImage(21);

			image.Set(_lastPosY, _lastPosX, true);

			return image;
		}

		#endregion
	}
}
