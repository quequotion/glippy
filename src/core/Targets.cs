 /* 
  * Glippy
  * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
  * The program is distributed under the terms of the GNU General Public License Version 3.
  * See LICENCE for details.
  */

using System.Collections.Generic;
using Gdk;

namespace Glippy.Core
{
	/// <summary>
	/// Clipboard targets and atoms.
	/// </summary>
	public static class Targets
	{
		/// <summary>
		/// HTML target.
		/// </summary>
		public static readonly string Html = "text/html";
		
		/// <summary>
		/// GNOME copied file target.
		/// </summary>
		public static readonly string File = "x-special/gnome-copied-files";
		
		/// <summary>
		/// UTF string target.
		/// </summary>
		public static readonly string UtfString = "UTF8_STRING";
		
		/// <summary>
		/// String target.
		/// </summary>
		public static readonly string String = "STRING";		
		
		/// <summary>
		/// Targets for clipboard.
		/// </summary>
		public static Dictionary<string, Atom> Atoms = new Dictionary<string, Atom>()
		{
			{ Html, Atom.Intern(Html, false) },
			{ File, Atom.Intern(File, false) },
			{ UtfString, Atom.Intern(UtfString, false) },
			{ String, Atom.Intern(String, false) }						
		};				
	}
}
