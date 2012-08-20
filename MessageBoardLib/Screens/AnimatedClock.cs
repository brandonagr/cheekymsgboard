using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoardLib.Screens
{
	/// <summary>
	/// Display a clock on the screen, when the digits change they are animated
	/// </summary>
	public class AnimatedClock : IMsgBoardScreen
	{
		#region Nested Classes

		/// <summary>
		/// Collection of the digits to be displayed
		/// </summary>
		private struct DisplayDigits : IEquatable<DisplayDigits>
		{
			int[] _digits;

			/// <summary>
			/// Create DisplayDigits for the given time
			/// </summary>
			/// <param name="time"></param>
			public DisplayDigits(DateTime time)
			{
				int firstHourDigit = time.Hour / 10;
				int firstMinuteDigit = time.Minute / 10;

				_digits = new int[4]
				{
					firstHourDigit,
					time.Hour - (firstHourDigit * 10),
					firstMinuteDigit,
					time.Minute - (firstMinuteDigit * 10)
				};
			}

			/// <summary>
			/// Get the character
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public char this[int index]
			{
				get
				{
					return (char)(_digits[index] + (int)'0');
				}
			}

			public override bool Equals(object obj)
			{
				if (obj is DisplayDigits)
					return Equals((DisplayDigits)obj);
				else
					return false;
			}

			public override int GetHashCode()
			{
				return _digits[0] + _digits[1] * 10 + _digits[2] * 100 + _digits[3] * 1000;
			}

			public bool Equals(DisplayDigits other)
			{
				return _digits[0] == other._digits[0] &&
					_digits[1] == other._digits[1] &&
					_digits[2] == other._digits[2] &&
					_digits[3] == other._digits[3];
			}
		}

		#endregion

		#region Declerations

		/// <summary>
		/// Number of seconds for a digit to move
		/// </summary>
		const double AnimationTime = 4.0;

		/// <summary>
		/// The speed at which the characters move
		/// </summary>
		const double AnimationSpeed = 8.0 / AnimationTime;

		/// <summary>
		/// If the clock is currently animating
		/// </summary>
		bool _animating = false;

		/// <summary>
		/// The amount of time that has been animated so far
		/// </summary>
		double _animationProgress = 0;

		/// <summary>
		/// The pixel offset of the current animation
		/// </summary>
		int _previousYOffset = 0;


		/// <summary>
		/// The digits currently being displayed
		/// </summary>
		DisplayDigits _currentDisplay;

		/// <summary>
		/// The digits that will be shown after the current animation finishes
		/// </summary>
		DisplayDigits _nextDisplay;

		/// <summary>
		/// MsgBoardImage 
		/// </summary>
		MsgBoardImage _currentImage;

		#endregion
		
		#region Initialization & Instantiation

		/// <summary>
		/// Default Constructor
		/// </summary>
		public AnimatedClock()
		{
			_currentDisplay = new DisplayDigits(DateTime.Now);
			_nextDisplay = _currentDisplay;

			_currentImage = new MsgBoardImage(21);
			_currentImage.Set(2, 10, true);
			_currentImage.Set(4, 10, true);
			RenderToCurrentImage();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Render the DisplayDigits to the _currentImage
		/// </summary>
		private void RenderToCurrentImage()
		{
			for (int digit = 0; digit < 4; digit++)
			{
				MsgBoardText.Render(_currentDisplay[digit], 0, GetXOffsetOf(digit), 4, _currentImage);
			}
		}

		int[] _offsets = new int[4] { 0, 5, 12, 17 };
		private int GetXOffsetOf(int digit)
		{
			return _offsets[digit];
		}

		#endregion

		#region Implement IMsgBoardScreen Methods

		/// <summary>
		/// Advance a frame of animation forward in time
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public bool Advance(double dt)
		{
			if (!_animating)
			{
				// see if we can start animating
				_nextDisplay = new DisplayDigits(DateTime.Now.AddSeconds(AnimationTime));

				if (!_nextDisplay.Equals(_currentDisplay))
				{
					_animating = true;
					_animationProgress = 0;
					_previousYOffset = 0;
				}
			}

			if (_animating)
			{
				_animationProgress += AnimationSpeed * dt;

				// need to draw a new frame
				if ((int)_animationProgress > _previousYOffset)
				{
					_previousYOffset = (int)_animationProgress;

					if (_previousYOffset > 8)
					{
						_previousYOffset = 8;
						_animating = false;
						_currentDisplay = _nextDisplay;
					}

					for (int digit = 0; digit < 4; digit++)
					{
						if (_currentDisplay[digit] == _nextDisplay[digit])
							continue;

						int x = GetXOffsetOf(digit);
						_currentImage.Clear(x, x + 3);

						MsgBoardText.Render(_currentDisplay[digit], -_previousYOffset, x, 4, _currentImage);
						MsgBoardText.Render(_nextDisplay[digit], -_previousYOffset + 8, x, 4, _currentImage);
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Return the current screen to display
		/// </summary>
		/// <returns></returns>
		public MsgBoardImage GetCurrentScreenImage()
		{
			return _currentImage;
		}

		#endregion
	}
}
