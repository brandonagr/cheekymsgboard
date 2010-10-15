using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HIDLib;

namespace ConsoleApplication1
{
	class MessageBoardDevice : HIDDevice
	{
		public class MessageBoardPacket : OutputReport
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="oDev">Device constructing this report</param>
			public MessageBoardPacket(MessageBoardDevice oDev) : base(oDev) { }

			public void SetPacketData(byte[] data)
			{
				if (data.Length > base.BufferLength)
					throw new InvalidOperationException("Data too big!");

				Array.Copy(data, 0, Buffer, 1, data.Length);
			}
		}


		public MessageBoardDevice()
		{ }

		static public MessageBoardDevice Create()
		{
			return FindDevice<MessageBoardDevice>(0x1D34, 0x0013);
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
			var packet = new MessageBoardPacket(this);
			Random rand = new Random((int)DateTime.Now.Ticks);
			byte[] data = new byte[8];

			for (int i = 0; i < 4; i++)
			{
				data[0] = 0x2;
				data[1] = (byte)(i << 1);

				for (int j = 2; j < data.Length; j++)
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
