/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using AppIndicator;
using Glippy.Core;
using Glippy.Core.Api;
using Mono.Unix;

namespace Glippy.Indicator
{
	/// <summary>
	/// Application indicator for Glippy.
	/// </summary>
	public class Indicator : ITray
	{
		/// <summary>
		/// Application indicator.
		/// </summary>
		private ApplicationIndicator indicator;
		
		/// <summary>
		/// Function which rebuilds menu.
		/// </summary>
		private MenuFunc RebuildMenu;
		
		/// <summary>
		/// Gets plugin name. Used in plugins tab and as label of preferences page.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// Gets plugin description.
		/// </summary>
		public string Description { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the Indicator class.
		/// </summary>
		public Indicator()
		{
			this.Name = Catalog.GetString("Indicator");
			this.Description = Catalog.GetString("Ubuntu indicator icon.");
		}
		
		/// <summary>
		/// Loads tray plugin.
		/// </summary>
		/// <param name="menu">Menu reference.</param>		
		/// <param name="rebuildMenuFunc">Rebuild menu function.</param>		
		public void Load(Gtk.Menu menu, MenuFunc rebuildMenuFunc)
		{
			this.RebuildMenu = rebuildMenuFunc;			
			this.indicator = new ApplicationIndicator("glippy", EnvironmentVariables.PanelIcon, Category.Other);
			this.indicator.Status = Status.Active;
			this.indicator.Menu = menu;			
		}
		
		/// <summary>
		/// Unloads plugin.
		/// </summary>
		public void Unload()
		{
			this.indicator.Status = Status.Passive;
			this.indicator = null;			
			System.GC.Collect(); // HACK: Ugly way to remove appindicator icon instantly.
		}
		
		/// <summary>
		/// Handles Clipboard's ClipboardChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		public void OnClipboardChanged(object sender, ClipboardChangedArgs args)
		{
			this.RefreshMenu();			
		}
		
		/// <summary>
		/// Handles SettingChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		public void OnSettingChanged(object sender, SettingChangedArgs args)
		{
			if (args.RequiresMenuRebuild)
				this.RefreshMenu();
		}
		
		/// <summary>
		/// Handles MenuRebuilt event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="menu">Newly rebuilt menu.</param>
		public void OnMenuRebuilt(object sender, Gtk.Menu menu)
		{
			this.indicator.Menu = menu;			
			Tools.RunPendingGtkEvents();
		}
		
		/// <summary>
		/// Rebuilds and refreshes the menu.
		/// </summary>
		private void RefreshMenu()
		{
			this.indicator.Menu = this.RebuildMenu();						
		}
	}
}
