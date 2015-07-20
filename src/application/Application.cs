/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using Glippy.Core;
using Glippy.Core.Extensions;

namespace Glippy.Application
{
	/// <summary>
	/// Glippy application class.
	/// </summary>
	public static class Application
	{
		/// <summary>
		/// Initializes Glippy user interface.
		/// </summary>
		public static void Initialize()
		{
			if (Settings.Instance[Settings.Keys.Core.SaveHistoryOnExit].AsBoolean())
				Clipboard.Instance.Load(History.Load());
			
			new Ui();						
		}			
	}
}
