using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HIDLib;

namespace MessageBoardLib
{
	public class MsgBoardDevice : HIDDevice
	{
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


		public MsgBoardDevice()
		{ }

		static public MsgBoardDevice Create()
		{
			return FindDevice<MsgBoardDevice>(0x1D34, 0x0013);
		}


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

			//static byte[] packet1 = new byte[] { 0x02, 0x00, 0xef, 0xff, 0xfe, 0xef, 0xff, 0xfe };
			//static byte[] packet2 = new byte[] { 0x02, 0x02, 0xef, 0xff, 0xfe, 0xef, 0xff, 0xfe };
			//static byte[] packet3 = new byte[] { 0x02, 0x04, 0xef, 0xff, 0xfe, 0xef, 0xff, 0xfe };
			//static byte[] packet4 = new byte[] { 0x02, 0x06, 0xef, 0xff, 0xfe, 0x00, 0x00, 0x00 };

			//for (int i = 2; i < 8; i++)
			//    packet1[i] = (byte)rand.Next(255);

			//packet.SetPacketData(packet1);
			//Write(packet);

			//for (int i = 2; i < 8; i++)
			//    packet2[i] = (byte)rand.Next(255);

			//packet.SetPacketData(packet2);
			//Write(packet);

			//for (int i = 2; i < 8; i++)
			//    packet3[i] = (byte)rand.Next(255);

			//packet.SetPacketData(packet3);
			//Write(packet);

			//packet4[0] = brightness;
			//for (int i = 2; i < 8; i++)
			//    packet4[i] = (byte)rand.Next(255);

			//packet.SetPacketData(packet4);
			//Write(packet);
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
				packet.Data[1] = 0x01; //brightness
				packet.Data[2] = (byte)(i << 1); //row

				//first row inside packet
				int currentByte = 4;
				int currentBit = 0;
				int currentY = i << 1;
				for (int x = 0; x < 21; x++)
				{
					int rotY = (MsgBoardImage.Height - currentY) - 1;
					int rotX = (image.Width - x) - 1;
					//int rotY = currentY;
					//int rotX = x;

					if (!image.Get(rotY, rotX)) //turn on bit, in order to turn off the LED
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
						int rotY = (MsgBoardImage.Height - currentY) - 1;
						int rotX = (image.Width - x) - 1;
						//int rotY = currentY;
						//int rotX = x;

						if (!image.Get(rotY, rotX)) //turn on bit, in order to turn off the LED
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
	}
}
