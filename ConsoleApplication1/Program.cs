using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HIDLib;
using System.Threading;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			MessageBoardDevice device = MessageBoardDevice.Create();

			if (device == null)
			{
				Console.WriteLine("Not found :(");
				Console.ReadLine();
				return;
			}

			Console.WriteLine("Created!");

			while (true)
			{
				device.ScreenRandom();

				//Thread.Sleep(66); //~15 hz
				Thread.Sleep(200);
			}


			Console.ReadLine();
		}
	}
}
