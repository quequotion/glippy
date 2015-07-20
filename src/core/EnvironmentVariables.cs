/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using Glippy.Core.Extensions;

namespace Glippy.Core
{
	/// <summary>
	/// Enviroment variables.
	/// </summary>
	public static class EnvironmentVariables
	{
		/// <summary>
		/// Applicatin version.
		/// </summary>
		public const string Version = "0.6";
		
		/// <summary>
		/// Config path.
		/// </summary>
		public static readonly string ConfigPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/.local/share/glippy/";		
		
		/// <summary>
		/// Panel icon name.
		/// </summary>
		public static readonly string PanelIcon = Settings.Instance[Settings.Keys.UI.PasteIcon].AsBoolean() ? "edit-paste" : "glippy-panel";
	}
}
