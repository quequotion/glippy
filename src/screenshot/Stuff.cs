/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System.Diagnostics;

namespace Glippy.Screenshot
{
	/// <summary>
	/// Stuff goes here.
	/// </summary>
	internal static class Stuff
	{
		/// <summary>
		/// Gets architecture of system (executes uname).
		/// </summary>
		/// <returns>Architecture type.</returns>
		public static Architectures Architecture()
		{			
			using (Process process = new Process())
			{
				process.StartInfo.FileName = "uname";
				process.StartInfo.Arguments = "-m";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;			
				process.Start();			
				process.WaitForExit();
				
				switch (process.StandardOutput.ReadLine())
				{
					case "x86_64":
						return Architectures.X86_64;
					
					default:
						return Architectures.X86;
				}									
			}
		}		
	}	
}
