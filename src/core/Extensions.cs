/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

namespace Glippy.Core.Extensions
{		
	/// <summary>
	/// Gtk/Gdk extensions.
	/// </summary>
	public static class GtkExtensions
	{
		/// <summary>
		/// Adds Gtk.MenuItem to Gtk.Menu.
		/// </summary>
		/// <param name="menu">Gtk.Menu object.</param>
		/// <param name="label">Menu item label.</param>
		/// <param name="atEnd">Whether item will be added to begining or end of menu.</param>
		/// <param name="handler">Activated event handler.</param>		
		/// <param name="sensitive">Whether item is sensitive.</param>
		public static Gtk.MenuItem AddMenuItem(this Gtk.Menu menu, string label, bool atEnd = true, System.EventHandler handler = null, bool sensitive = true)
		{
			Gtk.MenuItem menuitem = new Gtk.MenuItem(label);
			menu.AddWidget(menuitem, atEnd, handler);
			menuitem.Sensitive = sensitive;
			
			return menuitem;
		}
		
		/// <summary>
		/// Adds Gtk.Widget to Gtk.Menu.
		/// </summary>
		/// <param name="menu">Gtk.Menu object.</param>
		/// <param name="widget">Gtk.Widget object.</param>
		/// <param name="atEnd">Whether item will be added to begining or end of menu.</param>
		/// <param name="handler">Activated event handler.</param>		
		public static Gtk.Widget AddWidget(this Gtk.Menu menu, Gtk.Widget widget, bool atEnd = true, System.EventHandler handler = null)
		{		
			if (atEnd)
				menu.Append(widget);
			else
				menu.Insert(widget, 0);
		
			if (handler != null && widget is Gtk.MenuItem)
				((Gtk.MenuItem)widget).Activated += handler;
			
			return widget;
		}		
		
		/// <summary>
		/// Determines whether list is empty.
		/// </summary>
		/// <returns>True if this list is empty, false otherwise.</returns>
		/// <param name="list">ListStore instance.</param>
		public static bool IsEmpty(this Gtk.ListStore list)
		{
			foreach (object[] o in list)
			{
				return false;
			}
			
			return true;
		}				
	}
	
	/// <summary>
	/// Object extensions.
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// Converts value to boolean.
		/// </summary>
		/// <param name="o">Object instance.</param>
		/// <returns>Boolean or false.</returns>
		public static bool AsBoolean(this object o)
		{
			return o is bool ? (bool)o : false;
		}
		
		/// <summary>
		/// Converts object to integer.
		/// </summary>
		/// <param name="o">Object instance.</param>
		/// <returns>Integer or zero.</returns>
		public static int AsInteger(this object o)
		{
			return o is int ? (int)o : 0;
		}
		
		/// <summary>
		/// Converts setting value to string.
		/// </summary>
		/// <param name="o">Object instance.</param>
		/// <returns>String or empty.</returns>
		public static string AsString(this object o)
		{
			return o is string ? o.ToString() : string.Empty;
		}			
	}	
	
	/// <summary>
	/// GConf extensions.
	/// </summary>
	public static class GConfExtensions
	{
		/// <summary>
		/// Unsets key 
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="key">Key.</param>
		public static void Unset(this GConf.Client client, string key)
		{
			using (System.Diagnostics.Process p = new System.Diagnostics.Process())
			{
				p.StartInfo.FileName = "gconftool";
				p.StartInfo.Arguments = "-u " + key;
				p.Start();		
				p.WaitForExit();
			}
		}
	}	
}
