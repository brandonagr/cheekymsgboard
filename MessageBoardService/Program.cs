using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;

namespace MessageBoardService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			if (Debugger.IsAttached)
			{
				Console.WriteLine("Starting up service...");

				MessageBoardService.RunFromConsole();

				Console.WriteLine("Enter to terminate app...");
				Console.ReadLine();
			}
			else
			{
				ServiceBase.Run(new MessageBoardService());
			}
		}
	}
}
