using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using MessageBoardLib;
using System.IO;
using System.Reflection;
using MessageBoardLib.Screens;

namespace MessageBoardService
{
	/// <summary>
	/// Service that run the messag eboard
	/// </summary>
	public partial class MessageBoardService : ServiceBase
	{
		#region Declarations

		/// <summary>
		/// The message board object
		/// </summary>
		MsgBoardAsyncDriver _msgBoard;

		#endregion

		#region Initialization

		/// <summary>
		/// Default constructor
		/// </summary>
		public MessageBoardService()
		{ }

		/// <summary>
		/// Start the service
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStart(string[] args)
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			_msgBoard = new MsgBoardAsyncDriver(new Clock());

			_msgBoard.SetBrightness(MsgBoardDevice.Brightness.DIM);
			_msgBoard.BeginDriver();
		}

		/// <summary>
		/// Stop the service
		/// </summary>
		protected override void OnStop()
		{
			_msgBoard.StopDriver();
		}

		/// <summary>
		/// Call this if starting from a test console
		/// </summary>
		public static void RunFromConsole()
		{
			new MessageBoardService().OnStart(null);
		}

		#endregion
	}
}
