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

			_device = MsgBoardDevice.Create();
			if (_device == null)
			{
				Console.WriteLine("Not found :(");
				Console.ReadLine();
				return;
			}

			MsgBoardImage image = MsgBoardText.Render("ERR !!! MSP_Stage !!!!!! 0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz");
			_view = new MsgBoardView(image, 23.0, MsgBoardView.ScrollType.WRAP);
			_fx = new BottomLineFX(1.0);

			Thread renderThread = new Thread(new ThreadStart(Program.ImageDriver))
				{
					IsBackground = true,
					Name = "MsgBoard Render Thread",
				};
			renderThread.Start();


			while (true)
			{
				Console.WriteLine("Input a new msg: ");

				string message = Console.ReadLine();

				if (string.IsNullOrEmpty(message))
					_view = null;
				else
					_view = new MsgBoardView(MsgBoardText.Render(message), 20.0, MsgBoardView.ScrollType.WRAP);
			}


			//Stopwatch sw = Stopwatch.StartNew();
			//long curMs = sw.ElapsedTicks;
			//MsgBoardImage curImage = null;
			//while (true)
			//{
			//    long ms = sw.ElapsedMilliseconds;

			//    if (curImage == null || view.AdvanceScroll((ms - curMs) * 0.001))
			//        curImage = view.GetCurrentScreenView();

			//    curMs = ms;

			//    //curImage.WriteToConsole();
			//    device.WriteScreen(curImage);

			//    Thread.Sleep(50);
			//}


			Console.WriteLine("Done.");
			Console.ReadLine();
		}


		static MsgBoardView _view = null;

		static MsgBoardDevice _device = null;
		static MsgBoardImage _image = null;
		static IMsgBoardFX _fx = null;
		static void ImageDriver()
		{
			Stopwatch sw = Stopwatch.StartNew();
			long curMs = sw.ElapsedTicks;

			while (true)
			{
				long ms = sw.ElapsedMilliseconds;
				double dt = (ms - curMs) * 0.001;
				curMs = ms;

				if (_view != null)
				{
					_fx.Advance(dt);

					if (_image == null || _view.AdvanceScroll(dt))
					{
						_image = _view.GetCurrentScreenView();
						_fx.ApplyFX(_image);
					}

					_device.WriteScreen(_image);
				}

				Thread.Sleep(50);
			}
		}
	}
}
