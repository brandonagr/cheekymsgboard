using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBoardLib;
using System.Threading;
using System.Diagnostics;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			//MsgBoardAsyncDriver msgBoard = new MsgBoardAsyncDriver(new ImageScroller(MsgBoardText.Render("ERRORS"), 20.0, ImageScroller.ScrollType.WRAP));
			MsgBoardAsyncDriver msgBoard = new MsgBoardAsyncDriver(new Clock());

			msgBoard.SetBrightness(MsgBoardDevice.Brightness.DIM);

			msgBoard.BeginDriver();


			//while (true)
			//{
			//    Thread.Sleep(1000);
			//    msgBoard.SetBrightness(MsgBoardDevice.Brightness.BRIGHT);
			//    Thread.Sleep(1000);
			//    msgBoard.SetBrightness(MsgBoardDevice.Brightness.DIM);
			//}

			//Thread.Sleep(5000);


			//msgBoard.StopDriver();
			//msgBoard.SetBrightness(MsgBoardDevice.Brightness.DIM);

			//Thread.Sleep(1000);





			//_device = MsgBoardDevice.Create();
			//if (_device == null)
			//{
			//    Console.WriteLine("Not found :(");
			//    Console.ReadLine();
			//    return;
			//}






			//MsgBoardImage image = MsgBoardText.Render("ERR !!! MSP_Stage !!!!!! 0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz");
			//_view = new MsgBoardView(image, 23.0, MsgBoardView.ScrollType.WRAP);
			//_fx = new BottomLineFX(1.5);

			//Thread renderThread = new Thread(new ThreadStart(Program.ImageDriver))
			//    {
			//        IsBackground = true,
			//        Name = "MsgBoard Render Thread",
			//    };
			//renderThread.Start();


			while (true)
			{
				Console.WriteLine("Input a new msg: ");

				string message = Console.ReadLine();

				if (string.IsNullOrEmpty(message))
					break;

				msgBoard.SetSource(new ImageScroller(MsgBoardText.Render(message), 20.0, ImageScroller.ScrollType.WRAP));

				if (message.Contains('!'))
					msgBoard.SetFX(new InverseFX(2.0));
				else
					msgBoard.SetFX(null);
			}


			Console.WriteLine("Done.");
			Console.ReadLine();
		}
	}
}
