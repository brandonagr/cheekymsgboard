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

			MsgBoardAsyncDriver msgBoard = new MsgBoardAsyncDriver(MsgBoardText.Render("ERRORS"), 20.0);

			msgBoard.BeginDriver();



			Thread.Sleep(5000);


			msgBoard.StopDriver();


			Thread.Sleep(1000);





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

				msgBoard.SetSourceImage(MsgBoardText.Render(message), 20.0);
			}


			Console.WriteLine("Done.");
			Console.ReadLine();
		}
	}
}
