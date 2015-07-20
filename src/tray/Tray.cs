/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Glippy.Core;
using Glippy.Core.Api;
using Gtk;
using Mono.Unix;

namespace Glippy.Tray
{
	/// <summary>
	/// Status tray icon class.
	/// </summary>
	public class Tray : ITray
	{
		/// <summary>
		/// Status icon.
		/// </summary>
		private StatusIcon statusIcon;
		
		/// <summary>
		/// Menu reference.
		/// </summary>
		private Menu menu;
		
		/// <summary>
		/// Function which rebuilds menu.
		/// </summary>
		private MenuFunc RebuildMenu;
		
		/// <summary>
		/// Gets plugin name.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// Gets plugin description.
		/// </summary>
		public string Description { get; private set; }
				
		/// <summary>
		/// Creates new instance of Tray class.
		/// </summary>
		public Tray()
		{						
			this.Name = Catalog.GetString("Tray icon");
			this.Description = Catalog.GetString("Notification area icon.");
		}
		
		/// <summary>
		/// Loads tray plugin.
		/// </summary>
		/// <param name="menu">Menu reference.</param>		
		/// <param name="rebuildMenuFunc">Rebuild menu function.</param>		
		public void Load(Menu menu, MenuFunc rebuildMenuFunc)
		{
			this.menu = menu;
			this.RebuildMenu = rebuildMenuFunc;
			this.statusIcon = new StatusIcon();
			this.statusIcon.IconName = EnvironmentVariables.PanelIcon;
			this.statusIcon.Activate += this.OnStatusIconActivated;					
		}
		
		/// <summary>
		/// Unloads tray plugin.
		/// </summary>
		public void Unload()
		{
			this.statusIcon.Visible = false;
			this.statusIcon = null;			
		}
		
		/// <summary>
		/// Rebuilds and shows menu after icon activation.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="args">Event arguments.</param>
		private void OnStatusIconActivated(object sender, EventArgs args)
		{
			this.menu = this.RebuildMenu();
			this.menu.Popup();	
		}
		
		/// <summary>
		/// Handles Clipboard's ClipboardChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		public void OnClipboardChanged(object sender, ClipboardChangedArgs args)
		{
		}
		
		/// <summary>
		/// Handles SettingChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		public void OnSettingChanged(object sender, SettingChangedArgs args)
		{
		}
		
		public void OnMenuRebuilt(object sender, Gtk.Menu menu)
		{		
		}
	}
}
