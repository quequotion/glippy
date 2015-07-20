/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using Gdk;

namespace Glippy.Core
{
	/// <summary>
	/// Delegate used to pass menu methods.
	/// </summary>
	public delegate Gtk.Menu MenuFunc();	
	
	/// <summary>
	/// Delegate for ClipboardChanged event handler.
	/// </summary>
	public delegate void ClipboardChanged(object sender, ClipboardChangedArgs args);	
	
	/// <summary>
	/// Delegate for SettingChanged event handler.
	/// </summary>
	public delegate void SettingChanged(object sender, SettingChangedArgs args);
	
	/// <summary>
	/// Clipboard changed arguments.
	/// </summary>
	public class ClipboardChangedArgs
	{
		/// <summary>
		/// Gets atom which represents clipboard which has been changed.
		/// </summary>
		public Atom Clipboard { get; private set; }
		
		/// <summary>
		/// Gets previous clipboard item.
		/// </summary>
		public Item OldValue { get; private set; }
		
		/// <summary>
		/// Gets current clipboard item.
		/// </summary>
		public Item NewValue { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the ClipboardChangedArgs class.
		/// </summary>
		/// <param name="clipboard">Atom of clipboard which has been changed.</param>
		/// <param name="oldValue">Previous clipboard item.</param>
		/// <param name="newValue">Current clipboard item.</param>
		public ClipboardChangedArgs(Atom clipboard, Item oldValue, Item newValue)
		{
			this.Clipboard = clipboard;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}
	
	/// <summary>
    /// Setting changed arguments.
    /// </summary>
	public class SettingChangedArgs
	{
		/// <summary>
		/// Gets the key which has been changed.
		/// </summary>
		public string Key { get; private set; }
		
		/// <summary>
		/// If set, menu needs to be rebuilt because of setting change.
		/// </summary>
		public bool RequiresMenuRebuild { get; private set; }
		
		/// <summary>
		/// Creates new instance of SettingChangedArgs.
		/// </summary>
		/// <param name="key">Key which has been changed.</param>
		/// <param name="requiresMenuRebuild">Indicates whether menu needs to be rebuilt.</param>		
		public SettingChangedArgs(string key, bool requiresMenuRebuild)
		{
			this.Key = key;
			this.RequiresMenuRebuild = requiresMenuRebuild;
		}
	}
}
