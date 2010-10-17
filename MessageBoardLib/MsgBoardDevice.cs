using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HIDLib;
using System.Threading;
using System.Diagnostics;

namespace MessageBoardLib
{
	public class MsgBoardDevice : HIDDevice
	{
		#region Nested Classes

		public class MsgBoardPacket : OutputReport
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="oDev">Device constructing this report</param>
			public MsgBoardPacket(MsgBoardDevice oDev) : base(oDev) { }

			public void SetPacketData(byte[] data)
			{
				if (data.Length > base.BufferLength)
					throw new InvalidOperationException("Data too big!");

				Array.Copy(data, 0, Buffer, 0, data.Length);
			}

			public void ClearBuffer()
			{
				SetPacketData(new byte[base.BufferLength]);
			}

			public byte[] Data
			{
				get { return Buffer; }
			}
		}

		#endregion
		#region Declerations

		/// <summary>
		/// How bright the LED display is
		/// </summary>
		public enum Brightness : byte
		{
			DIM = 0,
			MEDIUM = 1,
			BRIGHT = 2,
		}

		private Brightness _brightness = Brightness.DIM;

		/// <summary>
		/// The orientation of the device, used to correct the output if the device is upside down
		/// </summary>
		public enum Orientation
		{
			NORMAL,
			ROTATED_180,
		}

		private Orientation _orientation = Orientation.NORMAL;

		#endregion
		#region Intialization

		public MsgBoardDevice()
		{ }

		static public MsgBoardDevice Create()
		{
			return FindDevice<MsgBoardDevice>(0x1D34, 0x0013);
		}

		#endregion
		#region Public Methods

		/*
		 *  packet format as viewed on the screen:, bits are inverse 0 = on, 1 = off 
		 *  [0] = brightness, either 0(dim) - 2(bright)
		 *  [1] = row, 0, or 2, or 4 or 6
		 *  
		 * [4b0] [4b1] [4b2] [4b3] [4b4] [4b5] [4b6] [4b7] [3b0] [3b1] [3b2] [3b3] [3b4] [3b5] [3b6] [3b7] [2b0] [2b1] [2b2] [2b3] [2b4]
		 * [7b0] [7b1] [7b2] [7b3] [7b4] [7b5] [7b6] [7b7] [6b0] [6b1] [6b2] [6b3] [6b4] [6b5] [6b6] [6b7] [5b0] [5b1] [5b2] [5b3] [5b4]
		 * 
		 */

		public void ScreenRandom()
		{
			var packet = new MsgBoardPacket(this);
			Random rand = new Random((int)DateTime.Now.Ticks);
			byte[] data = new byte[9];

			for (int i = 0; i < 4; i++)
			{
				data[1] = 0x2;
				data[2] = (byte)(i << 1);

				for (int j = 3; j < data.Length; j++)
					data[j] = (byte)rand.Next(255);

				packet.SetPacketData(data);
				Write(packet);
			}
		}

		public void WriteScreen(MsgBoardImage image)
		{
			if (image.Width != 21)
				throw new ArgumentOutOfRangeException("Image must be the exact correct width");

			MsgBoardPacket packet = new MsgBoardPacket(this);

			// For each of the 4 packet
			for (int i = 0; i < 4; i++)
			{
				packet.ClearBuffer();

				packet.Data[0] = 0x00;
				packet.Data[1] = (byte)_brightness; //brightness
				packet.Data[2] = (byte)(i << 1); //row

				//first row inside packet
				int currentByte = 4;
				int currentBit = 0;
				int currentY = i << 1;
				for (int x = 0; x < 21; x++)
				{
					int imageY, imageX;
					switch(_orientation)
					{
						case Orientation.ROTATED_180:
							imageY = (MsgBoardImage.Height - currentY) - 1;
							imageX = (image.Width - x) - 1;
							break;
						default:
						case Orientation.NORMAL:
							imageY = currentY;
							imageX = x;
							break;
					}

					if (!image.Get(imageY, imageX)) //turn on bit, in order to turn off the LED
					{
						byte turnBitOfff = (byte)(1 << currentBit);
						packet.Data[currentByte + 1] |= turnBitOfff;
					}

					currentBit++;
					if (currentBit == 8)
					{
						currentBit = 0;
						currentByte--;
					}
				}

				////second row inside packet
				if (i < 3) //last packet doesnt have a second row
				{
					currentByte = 7;
					currentBit = 0;
					currentY = currentY + 1;
					for (int x = 0; x < 21; x++)
					{
						int imageY, imageX;
						switch (_orientation)
						{
							case Orientation.ROTATED_180:
								imageY = (MsgBoardImage.Height - currentY) - 1;
								imageX = (image.Width - x) - 1;
								break;
							default:
							case Orientation.NORMAL:
								imageY = currentY;
								imageX = x;
								break;
						}

						if (!image.Get(imageY, imageX)) //turn on bit, in order to turn off the LED
						{
							byte turnBitOfff = (byte)(1 << currentBit);
							packet.Data[currentByte + 1] |= turnBitOfff;
						}

						currentBit++;
						if (currentBit == 8)
						{
							currentBit = 0;
							currentByte--;
						}
					}
				}

				Write(packet);
			}
		}

		#endregion
		#region Properties


		public Brightness BrightnessLevel
		{
			get { return _brightness; }
			set { _brightness = value; }
		}

		public Orientation DeviceOrientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}

		#endregion
		#region Overrides

		/// <summary>
		/// Dispose.
		/// </summary>
		/// <param name="bDisposing">True if object is being disposed - else is being finalised</param>
		protected override void Dispose(bool bDisposing)
		{
			if (bDisposing)
			{
				//turn stuff off?
			}
			base.Dispose(bDisposing);
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public class MsgBoardAsyncDriver
	{
		#region Declerations

		private MsgBoardDevice _device = null;

		private IMsgBoardScreen _screen = null; //The screen, produces images that are displayed on the device
		private MsgBoardImage _image = null; //The current image that is being displayed on the device
		private IMsgBoardFX _fx = null; //Any post processing FX to apply to the current _image

		volatile private int _stopThreadFlag = 0;
		private Thread _backgroundThread = null;

		#endregion
		#region Initialization & Instantiation

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <param name="scrollSpeed">Columns per second</param>
		public MsgBoardAsyncDriver(IMsgBoardScreen screen)
		{
			_device = MsgBoardDevice.Create();

			if (_device == null)
				throw new ApplicationException("Unable to find Message board");

			//_fx = new InverseFX(1.75);

			_screen = screen;
			_image = _screen.GetCurrentScreenImage();
		}

		#endregion
		#region Public Methods

		public void SetFX(IMsgBoardFX fx)
		{
			_fx = fx;
		}

		public void SetSource(IMsgBoardScreen screen)
		{
			if (screen == null && _backgroundThread != null)
			{
				StopDriver();
				_screen = null;
			}

			_screen = screen;
			_image = _screen.GetCurrentScreenImage();
		}

		public void SetBrightness(MsgBoardDevice.Brightness level)
		{
			_device.BrightnessLevel = level;
		}

		public void BeginDriver()
		{
			if (_device == null || _backgroundThread != null || _screen == null)
				return;

			_stopThreadFlag = 0;
			_backgroundThread = new Thread(new ThreadStart(this.Driver))
			{
				IsBackground = true,
				Name = "MsgBoard Render Thread",
				Priority = ThreadPriority.AboveNormal,
			};
			_backgroundThread.Start();
		}

		public void StopDriver()
		{
			if (_device == null)
				return;

			_stopThreadFlag = 1;
			_backgroundThread.Join();
			_backgroundThread = null;
		}

		#endregion
		#region Private Thread

		private void Driver()
		{
			Stopwatch sw = Stopwatch.StartNew();
			long curMs = sw.ElapsedTicks;

			try
			{
				while (true)
				{
					if (_stopThreadFlag != 0)
						return;

					long ms = sw.ElapsedMilliseconds;
					double dt = (ms - curMs) * 0.001;
					curMs = ms;

					if (_fx != null)
						_fx.Advance(dt);

					if (_screen.Advance(dt))
					{
						_image = _screen.GetCurrentScreenImage();

						if (_fx != null)
							_fx.ApplyFX(_image, _device);
					}
					_device.WriteScreen(_image);

					Thread.Sleep(30);
				}
			}
			catch (HIDDeviceException ex)
			{
				_backgroundThread = null;
				_device = null;
			}
		}

		#endregion
	}
}
