/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Glippy.Core;
using Glippy.Core.Api;
using Glippy.Core.Extensions;
using Mono.Unix;

namespace Glippy.Snippets
{
	/// <summary>
	/// Snippets plugin.
	/// </summary>
	public class Snippets : IPlugin
	{
		/// <summary>
		/// Snippets list.
		/// </summary>
		private Gtk.ListStore list;
		
		/// <summary>
		/// Menu item submenu;
		/// </summary>
		private Gtk.Menu submenu;
		
		/// <summary>
		/// Gets or sets edit snippet window.
		/// </summary>
		internal EditSnippetWindow EditSnippetWindow { get; set; }
						
		/// <summary>
		/// Gets plugin name. Used in plugins tab and as label of preferences page.
		/// </summary>		
		public string Name { get; private set; }
		
		/// <summary>
		/// Gets plugin description.
		/// </summary>
		public string Description { get; private set; }
		
		/// <summary>
		/// Gets menu item which is put in menu.
		/// </summary>		
		public Gtk.MenuItem MenuItem
		{
			get
			{ 
				if (!Settings.Instance[SettingsKeys.Enable].AsBoolean())
					return null;
				
				Gtk.MenuItem menuitem = new Gtk.MenuItem(Catalog.GetString("_Snippets"));
				menuitem.Submenu = this.submenu;
				return menuitem;
			}
		}
		
		/// <summary>
		/// Gets container, which is attached to preferences page.
		/// </summary>
		public Gtk.Container PreferencesPage
		{
			get	{ return new SnippetsPreferencesPage(this, this.list); }
		}
		
		/// <summary>
		/// The name of the snippets file.
		/// </summary>
		public static readonly string FileName = "snippets.xml";		
		
		/// <summary>
		/// Initializes a new instance of the Snippets class.
		/// </summary>
		public Snippets()
		{
			this.Name = Catalog.GetString("Snippets");
			this.Description = Catalog.GetString("Store small parts of reusable texts.");
		}
		
		/// <summary>
		/// Loads snippets from file.
		/// </summary>
		/// <param name="list">Snippets container.</param>
		/// <returns>List of snippets.</returns>
		public static Gtk.ListStore LoadFromFile()
		{		
			Gtk.ListStore list = new Gtk.ListStore(typeof(Snippet));			
			StringBuilder text = new StringBuilder();						
			Snippet snippet;
			
			try
			{
				if (!Directory.Exists(EnvironmentVariables.ConfigPath))
					Directory.CreateDirectory(EnvironmentVariables.ConfigPath);
				
				using (XmlTextReader reader = new XmlTextReader(EnvironmentVariables.ConfigPath + FileName))
				{					
					reader.Read();
					
					while (reader.Read())
					{
						text.Remove(0, text.Length);
					
						if (reader.Name == "label")
						{								
							snippet = new Snippet();
							reader.Read();							
							text.Append(reader.Value).Replace("&lt;", "<").Replace("&gt;", ">");							
							snippet.Label = text.ToString();							
							for (int i = 0; i < 4; i++)
								reader.Read();
							text.Remove(0, text.Length).Append(reader.Value).Replace("&lt;", "<").Replace("&gt;", ">");							
							snippet.Content = text.ToString();
							list.AppendValues(snippet);
							reader.Read();							
						}						
					}								
				}				
			}			
			catch (FileNotFoundException ex)
			{
				Tools.PrintInfo(ex, typeof(Snippets));
				SaveToFile(new Gtk.ListStore(typeof(Snippet)));
			}
			catch (Exception ex)
			{					
				Tools.PrintInfo(ex, typeof(Snippets));
			}			
			
			return list;
		}

		/// <summary>
		/// Saves snippets to file.
		/// </summary>
		/// <param name="list">List of snippets.</param>
		public static void SaveToFile(Gtk.ListStore list)
		{						
			StringBuilder label = new StringBuilder();
			StringBuilder text = new StringBuilder();			
			
			try
			{
				if (!Directory.Exists(EnvironmentVariables.ConfigPath))
					Directory.CreateDirectory(EnvironmentVariables.ConfigPath);
				
				using (XmlTextWriter writer = new XmlTextWriter(EnvironmentVariables.ConfigPath + FileName, Encoding.UTF8))
				{										
					writer.Formatting = Formatting.Indented;			
					writer.WriteProcessingInstruction ("xml", "version='1.0'");
					writer.WriteStartElement(string.Format("{0}s", "snippet"));
							
					Snippet snippet;
					
					foreach (object[] o in list)
					{		
						snippet = (Snippet)o[0];
						label.Remove(0, label.Length).Append(snippet.Label).Replace("<", "&lt;").Replace(">", "&gt;");
						text.Remove(0, text.Length).Append(snippet.Content).Replace("<", "&lt;").Replace(">", "&gt;");
						
						writer.WriteStartElement("snippet");
							writer.WriteStartElement("label");							
								writer.WriteString(label.ToString());
							writer.WriteEndElement();
							writer.WriteStartElement("text");							
								writer.WriteString(text.ToString());
							writer.WriteEndElement();
						writer.WriteEndElement();													
					}
					
					writer.WriteEndElement();
					writer.Close();					
				}
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, typeof(Snippets));
			}
		}
		
		/// <summary>
		/// Loads plugin.
		/// </summary>
		public void Load()
		{
			this.list = LoadFromFile();
			this.list.RowChanged += (s, e) =>
			{
				this.BuildMenu();
				Settings.Instance.EmitSettingChanged(this, new SettingChangedArgs(SettingsKeys.Enable, true));
				SaveToFile(this.list);
			};			
			this.list.RowDeleted += (s, e) =>
			{
				this.BuildMenu();
				Settings.Instance.EmitSettingChanged(this, new SettingChangedArgs(SettingsKeys.Enable, true));
				SaveToFile(this.list);
			};						
			this.BuildMenu();
			
			Settings.Instance.RegisterSetting(SettingsKeys.Enable, "/apps/glippy/snippets/enable", SettingTypes.Boolean, true);			
			Settings.Instance.RegisterSetting(SettingsKeys.PasteOnSelection, "/apps/glippy/snippets/paste_on_selection", SettingTypes.Boolean, false);
		}
		
		/// <summary>
		/// Releases all resource used by the Screenshot object.
		/// </summary>
		public void Dispose()
		{
			if (this.EditSnippetWindow != null)
			{
				this.EditSnippetWindow.Destroy();
				this.EditSnippetWindow.Dispose();
			}					
			
			this.list.Dispose();					
		}
		
		/// <summary>
		/// Creates or presents snippet edit window.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnMakeSnippetMenuItemActivated(object sender, EventArgs args)
		{
			if (this.EditSnippetWindow == null)
			{
				Item item = Clipboard.Instance.Items.FirstOrDefault(i => i.IsText);				
				this.EditSnippetWindow = new EditSnippetWindow(this, this.list, item != null ? item.Text : string.Empty);
				this.EditSnippetWindow.ShowAll();				
			}
			else
			{
				this.EditSnippetWindow.Present();
			}
		}
		
		/// <summary>
		/// Creates menu from snippets list.
		/// </summary>
		/// <param name="menuItem">Menu item.</param>				
		private void BuildMenu()
		{
			if (this.submenu != null)
			{
				foreach (Gtk.Widget w in this.submenu)
				{
					this.submenu.Remove(w);
					w.Destroy();
					w.Dispose();
				}
				
				this.submenu.Destroy();
				this.submenu.Dispose();
			}
						
			Snippet snippet;
			Gtk.MenuItem mi;
			this.submenu = new Gtk.Menu();
			
			foreach (object[] o in this.list)
			{
				snippet = ((Snippet)o[0]);
				string content = snippet.Content;
				mi = new Gtk.MenuItem(snippet.Label);
				this.submenu.Append(mi);
				mi.Activated += (s, e) =>
				{
					Item item = new Item(content);					
					Clipboard.Instance.SetAsContent(item);
					
					if (Settings.Instance[SettingsKeys.PasteOnSelection].AsBoolean())
						XHotkeys.Hotkeys.Instance.Paste();
				};
				mi.ButtonReleaseEvent += (s, e) =>
				{
					Item item = new Item(content);					
					Clipboard.Instance.SetAsContent(item);
					
					if (Settings.Instance[SettingsKeys.PasteOnSelection].AsBoolean())
						XHotkeys.Hotkeys.Instance.Paste();
				};				
			}
			
			if (!this.list.IsEmpty())
				this.submenu.Append(new Gtk.SeparatorMenuItem());
				
			mi = new Gtk.MenuItem(Catalog.GetString("_Make snippet from current content"));
			this.submenu.Append(mi);
			mi.Activated += this.OnMakeSnippetMenuItemActivated;
			mi.ButtonReleaseEvent += this.OnMakeSnippetMenuItemActivated;									
		}			
	}
}
