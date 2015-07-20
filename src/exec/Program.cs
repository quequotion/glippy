/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Glippy.Application;
using Glippy.Core;
using Glippy.Core.Api;
using Glippy.Core.Extensions;
using Mono.Unix;
using Mono.Unix.Native;

namespace Glippy
{	
	/// <summary>
	/// Program class. All your base are belong to us.
	/// </summary>
	internal class Program
	{		
		/// <summary>
		/// Creates instance of program.
		/// </summary>
		private Program()
		{
			AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
			new Gnome.Program(asm.Name, asm.Version.ToString(), Gnome.Modules.UI, new string[0]);
			Gnome.Global.MasterClient().SaveYourself += this.OnSaveYourself;
			Gnome.Global.MasterClient().Die += this.OnDie;			
						
			ThreadPool.QueueUserWorkItem((o) =>
			{				
				UnixSignal[] signals = new UnixSignal[] { new UnixSignal(Signum.SIGTERM), new UnixSignal(Signum.SIGINT) };
				UnixSignal.WaitAny(signals);	
				Gtk.Application.Invoke((s, args) =>
				{
					this.OnSaveYourself(this, null);
					this.OnDie(this, null);
				});		
			});
			
			Glippy.Application.Application.Initialize();
		}
				
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main(string[] args)
		{
			Gtk.Application.Init();				
			Mono.Unix.Catalog.Init("glippy", AppDomain.CurrentDomain.BaseDirectory + "../../share/locale");			
			
			foreach (string arg in args)
			{
				if (arg == "-s" || arg == "--sleep")
				{
					Sleep();
				}
				else if (arg == "-h" || arg == "--help")
				{
					PrintHelp();
					Environment.Exit(0);
				}
				else if (arg == "-r" || arg == "--restore-default-settings")
				{
					RestoreDefaultSettings();
					Console.WriteLine("Default configuration has been restored.");
					Environment.Exit(0);
				}				
			}
			
			Kill();			
			SetProcessName("glippy");			
			new Program();		
			Gtk.Application.Run();			
		}
		
		/// <summary>
		/// Restores default settings.
		/// </summary>
		private static void RestoreDefaultSettings()
		{
			foreach (IBase plugin in Plugins.LoadAllPlugins())
			{
				Type t = plugin.GetType();
				
				try
				{
					if (t.GetInterface(typeof(IPlugin).Name) != null)
						((IPlugin)plugin).Load();										
					else if (t.GetInterface(typeof(ITray).Name) != null)
						((ITray)plugin).Load(null, null);					
				}
				catch { }
			}
			
			Settings.Instance.RestoreDefaults();			
		}
		
		/// <summary>
		/// Prints help.
		/// </summary>
		private static void PrintHelp()
		{
			Assembly asm = Assembly.GetExecutingAssembly();			
			Console.Write("Glippy ");
			Console.WriteLine(asm.GetName().Version.ToString() + " - " + (asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute).Description);
			Console.WriteLine((asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute).Copyright);
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine("  --help, -h\t\t\t\tPrint this help.");
			Console.WriteLine("  --restore-default-settings, -r\tTry to load all plugins and restore default settings.");
			Console.WriteLine("  --sleep, -s\t\t\t\tSleep 3 seconds before run.");
			Console.WriteLine();
		}
		
		/// <summary>
		/// Sleeps for 3 seconds.
		/// </summary>
		private static void Sleep()
		{
			Thread.Sleep(3000);
		}
		
		/// <summary>
		/// Sends SIGTERM to all other Glippy instances.
		/// </summary>
		private static void Kill()
		{
			using (Process process = new Process())
			{
				process.StartInfo.FileName = "killall";
				process.StartInfo.Arguments = "glippy";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.Start();
				string output = process.StandardError.ReadToEnd();
				process.WaitForExit();
				
				if (string.IsNullOrEmpty(output))
					Thread.Sleep(666);				
			}
		}
		
		/// <summary>
		/// Sets the name of the process. From Banshee: Banshee.Base/Utilities.cs.
		/// </summary>
		/// <param name="name">Name.</param>
		private static void SetProcessName(string name)
		{
			if (prctl (15 /* PR_SET_NAME */, System.Text.Encoding.ASCII.GetBytes(string.Format("{0}\0", name)), System.IntPtr.Zero, System.IntPtr.Zero, System.IntPtr.Zero) != 0)
				throw new ApplicationException (string.Format("Error setting process name: {0}", Mono.Unix.Native.Stdlib.GetLastError()));
		}
		
		/// <summary>
		/// Saves history before application quits.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnSaveYourself(object sender, Gnome.SaveYourselfArgs args)
		{	
			if (Settings.Instance[Settings.Keys.Core.SaveHistoryOnExit].AsBoolean())			
				History.Save(Clipboard.Instance.Items);
		}
		
		/// <summary>
		/// Cleans up and terminates application.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnDie(object sender, EventArgs args)
		{						
			Environment.Exit(0);
		}
		
		/// <summary>
		/// Prctl from libc.
		/// </summary>
		[System.Runtime.InteropServices.DllImport("libc")]
		private static extern int prctl(int option, byte[] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);
	}
}
