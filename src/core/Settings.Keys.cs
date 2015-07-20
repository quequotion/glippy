/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

namespace Glippy.Core
{
	/// <summary>
	/// Settings manager.
	/// </summary>
	public sealed partial class Settings
	{
		/// <summary>
		/// Keys used to access options.
		/// </summary>
		public static class Keys
		{
			/// <summary>
			/// Core keys.
			/// </summary>
			public static class Core
			{
				/// <summary>
				/// Start at login key.
				/// </summary>
				public static readonly string StartAtLogin = "Core.StartAtLogin";
				
				/// <summary>
				/// Keyboard clipboard key.
				/// </summary>
				public static readonly string KeyboardClipboard = "Core.KeyboardClipboard";
				
				/// <summary>
				/// Mouse clipboard key.
				/// </summary>
				public static readonly string MouseClipboard = "Core.MouseClipboard";
				
				/// <summary>
				/// Images key.
				/// </summary>
				public static readonly string Images = "Core.Images";
				
				/// <summary>
				/// Supported files as images key.
				/// </summary>
				public static readonly string SupportedFilesAsImages = "Core.SupportedFilesAsImages";
				
				/// <summary>
				/// Synchronize clipboards key.
				/// </summary>
				public static readonly string SynchronizeClipboards = "Core.SynchronizeClipboards";
				
				/// <summary>
				/// Save history on exit key.
				/// </summary>
				public static readonly string SaveHistoryOnExit = "Core.SaveHistoryOnExit";
				
				/// <summary>
				/// Paste on selection key.
				/// </summary>
				public static readonly string PasteOnSelection = "Core.PasteOnSelection";
			}
			
			/// <summary>
			/// UI keys.
			/// </summary>
			public static class UI
			{				
				/// <summary>
				/// Size key.
				/// </summary>
				public static readonly string Size = "UI.Size";
				
				/// <summary>
				/// Length of the label key.
				/// </summary>
				public static readonly string LabelLength = "UI.LabelLength";
				
				/// <summary>
				/// Reverse order key.
				/// </summary>
				public static readonly string ReverseOrder = "UI.ReverseOrder";
				
				/// <summary>
				/// Paste icon key.
				/// </summary>
				public static readonly string PasteIcon = "UI.PasteIcon";
				
				/// <summary>
				/// Show about key.
				/// </summary>
				public static readonly string ShowAbout = "UI.ShowAbout";
				
				/// <summary>
				/// Show edit clipboard key.
				/// </summary>
				public static readonly string ShowEditClipboard = "UI.ShowEditClipboard";
				
				/// <summary>
				/// Show quit key.
				/// </summary>
				public static readonly string ShowQuit = "UI.ShowQuit";
			}			
			
			/// <summary>
			/// Hotkeys keys.
			/// </summary>
			public static class Hotkeys
			{
				/// <summary>
				/// Menu key.
				/// </summary>
				public static readonly string Menu = "Hotkeys.Menu";
				
				/// <summary>
				/// Menu modifiers key.
				/// </summary>
				public static readonly string MenuModifiers = "Hotkeys.MenuModifiers";
				
				/// <summary>
				/// Menu key key.
				/// </summary>
				public static readonly string MenuKey = "Hotkeys.MenuKey";
				
				/// <summary>
				/// Copy for mouse key.
				/// </summary>
				public static readonly string CopyMouse = "Hotkeys.CopyMouse";
				
				/// <summary>
				/// Copy for mouse modifiers key.
				/// </summary>
				public static readonly string CopyMouseModifiers = "Hotkeys.CopyMouseModifiers";
				
				/// <summary>
				/// Copy for mouse key key.
				/// </summary>
				public static readonly string CopyMouseKey = "Hotkeys.CopyMouseKey";
			}
			
			/// <summary>
			/// Plugins keys.
			/// </summary>
			public static class Plugins
			{
				/// <summary>
				/// List key.
				/// </summary>
				public static readonly string List = "Plugins.List";
			}
		}		
	}
}
