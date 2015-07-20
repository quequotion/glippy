/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;

namespace Glippy.Core
{
	/// <summary>
	/// Bunch of useful tools.
	/// </summary>
	public static class Tools
	{			
		/// <summary>
		/// Prints formatted information on console.
		/// </summary>
		/// <param name="ex">Exception.</param>
		/// <param name="sender">Exception sender.</param>
		public static void PrintInfo(Exception ex, Type sender)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.Write(sender != null ? string.Format("[Exception:{0}:{1}] ", ex.GetType().ToString(), sender.ToString()) : string.Format("[Exception:{0}] ", ex.GetType().ToString()));			
			Console.ResetColor();			
			Console.WriteLine(ex.Message);			
#if DEBUG
			Console.WriteLine("\nCall stack:");
			Console.WriteLine(ex.StackTrace);
			Console.WriteLine();
#endif
			if (ex.InnerException == null)
				return;
			
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.Write(sender != null ? string.Format("[Inner Exception:{0}:{1}] ", ex.GetType().ToString(), sender.ToString()) : string.Format("[Inner Exception:{0}] ", ex.GetType().ToString()));			
			Console.ResetColor();
			Console.WriteLine(ex.InnerException.Message);			
#if DEBUG
			Console.WriteLine("\nCall stack:");
			Console.WriteLine(ex.InnerException.StackTrace);
			Console.WriteLine();
#endif
		}		
		
		/// <summary>
		/// Prints formatted information on console.
		/// </summary>
		/// <param name="text">Information text.</param>
		/// <param name="sender">Print request sender.</param>
		public static void PrintInfo(string text, Type sender)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write(sender == null ? "[Info]" : "[Info:" + sender.ToString() + "] ");
			Console.ResetColor();
			Console.WriteLine(text);			
		}
		
		/// <summary>
		/// Iterates over GTK pending events and runs iterations.
		/// </summary>
		public static void RunPendingGtkEvents()
		{
			while (Gtk.Application.EventsPending())
			{
				Gtk.Application.RunIteration();
			}
		}
	}
}

