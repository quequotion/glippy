/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Gtk;

namespace Glippy.Core.Api
{			
	/// <summary>
	/// Base interface for plugins. Required.
	/// </summary>
	public interface IBase
	{
		/// <summary>
		/// Gets plugin name. Used in plugins tab and as label of preferences page.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets plugin description.
		/// </summary>
		string Description { get; }
	}
	
	/// <summary>
	/// Interface for regular plugins.
	/// </summary>
	public interface IPlugin : IBase, IDisposable
	{	
		/// <summary>
		/// Gets menu item which is put in menu.
		/// </summary>		
		MenuItem MenuItem { get; }		
		
		/// <summary>
		/// Gets container, which is attached to preferences page.
		/// </summary>
		Container PreferencesPage { get; }		
		
		/// <summary>
		/// Loads plugin.
		/// </summary>
		void Load();			
	}	
	
	/// <summary>
	/// Interface for tray plugins.
	/// </summary>
	public interface ITray : IBase
	{
		/// <summary>
		/// Loads tray plugin.
		/// </summary>
		/// <param name="menu">Menu reference.</param>		
		/// <param name="rebuildMenuFunc">Rebuild menu function.</param>
		void Load(Menu menu, MenuFunc rebuildMenuFunc);		
		
		/// <summary>
		/// Unloads plugin.
		/// </summary>
		void Unload();
		
		/// <summary>
		/// Handles Clipboard's ClipboardChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		void OnClipboardChanged(object sender, ClipboardChangedArgs args);			
		
		/// <summary>
		/// Handles SettingChanged event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		void OnSettingChanged(object sender, SettingChangedArgs args);			
		
		/// <summary>
		/// Handles MenuRebuilt event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="menu">Newly rebuilt menu.</param>
		void OnMenuRebuilt(object sender, Gtk.Menu menu);	
	}
}
