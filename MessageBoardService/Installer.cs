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
using System.Collections;
using System.Runtime.InteropServices;

namespace MessageBoardService
{
	// to install:
	// copy to local directory on server that you want to install it on
	// Find InstallUtil (probably in the C:\Windows\Microsoft.NET\Framework\v4.0.#\InstallUtil.exe)
	// run at a command prompt:  C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil powertools.rtpservice.exe

	[RunInstaller(true)]
	public partial class Installer : System.Configuration.Install.Installer
	{
		private ServiceInstaller _serviceInstaller;
		private ServiceProcessInstaller _serviceProcessInstaller;

		public Installer()
		{
			_serviceInstaller = new ServiceInstaller();
			_serviceInstaller.StartType = ServiceStartMode.Automatic;

			_serviceProcessInstaller = new ServiceProcessInstaller();
			_serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

			Installers.Add(_serviceInstaller);
			Installers.Add(_serviceProcessInstaller);
		}


		/// <summary>
		/// Setup the command line path for the service, based off of http://stackoverflow.com/questions/773678/how-to-get-name-of-windows-service-from-inside-the-service-itself
		/// </summary>
		/// <param name="stateSaver"></param>
		public override void Install(IDictionary stateSaver)
		{
			ConfigureInstaller(stateSaver);
			base.Install(stateSaver);
		}

		/// <summary>
		/// Read the parameters looking for Environment
		/// </summary>
		/// <param name="savedState"></param>
		private void ConfigureInstaller(IDictionary savedState)
		{
			_serviceInstaller.ServiceName = "MessageBoarddisplay";
			_serviceInstaller.DisplayName = "MessageBoardDisplay";
		}

		public override void Rollback(IDictionary savedState)
		{
			ConfigureInstaller(savedState);
			base.Rollback(savedState);
		}

		public override void Uninstall(IDictionary savedState)
		{
			ConfigureInstaller(savedState);
			base.Uninstall(savedState);
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr OpenSCManager(
			string lpMachineName,
			string lpDatabaseName,
			uint dwDesiredAccess);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr OpenService(
			IntPtr hSCManager,
			string lpServiceName,
			uint dwDesiredAccess);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct QUERY_SERVICE_CONFIG
		{
			public uint dwServiceType;
			public uint dwStartType;
			public uint dwErrorControl;
			public string lpBinaryPathName;
			public string lpLoadOrderGroup;
			public uint dwTagId;
			public string lpDependencies;
			public string lpServiceStartName;
			public string lpDisplayName;
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool QueryServiceConfig(
			IntPtr hService,
			IntPtr lpServiceConfig,
			uint cbBufSize,
			out uint pcbBytesNeeded);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ChangeServiceConfig(
			IntPtr hService,
			uint dwServiceType,
			uint dwStartType,
			uint dwErrorControl,
			string lpBinaryPathName,
			string lpLoadOrderGroup,
			IntPtr lpdwTagId,
			string lpDependencies,
			string lpServiceStartName,
			string lpPassword,
			string lpDisplayName);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseServiceHandle(
			IntPtr hSCObject);

		private const uint SERVICE_NO_CHANGE = 0xffffffffu;
		private const uint SC_MANAGER_ALL_ACCESS = 0xf003fu;
		private const uint SERVICE_ALL_ACCESS = 0xf01ffu;
	}
}
