/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Glippy.Core;
using Glippy.Core.Api;
using Glippy.Core.Extensions;
using Mono.Unix;

namespace Glippy.Actions
{
	/// <summary>
	/// Snippets plugin.
	/// </summary>
	public class Actions : IPlugin
	{
		/// <summary>
		/// Snippets list.
		/// </summary>
		private Gtk.ListStore list;
		
		/// <summary>
		/// Menu item submenu.
		/// </summary>
		private Gtk.Menu submenu;
		
		/// <summary>
		/// Gets or sets edit action window.
		/// </summary>
		internal EditActionWindow EditActionWindow { get; set; }
				
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
				
				Gtk.MenuItem menuitem = new Gtk.MenuItem(Catalog.GetString("_Actions"));
				menuitem.Sensitive = !this.list.IsEmpty() && Clipboard.Instance.Items.Any(i => i.Text != null);
				
				if (menuitem.Sensitive)
					menuitem.Submenu = this.submenu;	
				
				return menuitem;
			}
		}
		
		/// <summary>
		/// Gets container, which is attached to preferences page.
		/// </summary>
		public Gtk.Container PreferencesPage
		{
			get	{ return new ActionsPreferencesPage(this, this.list); }
		}
		
		/// <summary>
		/// The name of the actions file.
		/// </summary>
		public static readonly string FileName = "actions.xml";		
		
		/// <summary>
		/// Initializes a new instance of the Snippets class.
		/// </summary>
		public Actions()
		{
			this.Name = Catalog.GetString("Actions");
			this.Description = Catalog.GetString("Execute actions on current clipboard content.");
		}
		
		/// <summary>
		/// Loads actions from file.
		/// </summary>
		/// <param name="list">Snippets container.</param>
		/// <returns>List of snippets.</returns>
		public static Gtk.ListStore LoadFromFile()
		{		
			Gtk.ListStore list = new Gtk.ListStore(typeof(Action));
			
			StringBuilder text = new StringBuilder();						
			Action action;
			
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
							action = new Action();
							reader.Read();							
							text.Append(reader.Value).Replace("&lt;", "<").Replace("&gt;", ">");							
							action.Label = text.ToString();							
							for (int i = 0; i < 4; i++)
								reader.Read();
							text.Remove(0, text.Length).Append(reader.Value).Replace("&lt;", "<").Replace("&gt;", ">");							
							action.Content = text.ToString();
							list.AppendValues(action);
							reader.Read();							
						}						
					}								
				}				
			}	
			catch (FileNotFoundException ex)
			{
				Tools.PrintInfo(ex, typeof(Actions));
				SaveToFile(new Gtk.ListStore(typeof(Action)));
			}		
			catch (Exception ex)
			{					
				Tools.PrintInfo(ex, typeof(Actions));
			}			
			
			return list;
		}

		/// <summary>
		/// Saves actions to file.
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
					writer.WriteStartElement(string.Format("{0}s", "action"));
							
					Action action;
					
					foreach (object[] o in list)
					{		
						action = (Action)o[0];
						label.Remove(0, label.Length).Append(action.Label).Replace("<", "&lt;").Replace(">", "&gt;");
						text.Remove(0, text.Length).Append(action.Content).Replace("<", "&lt;").Replace(">", "&gt;");
						
						writer.WriteStartElement("action");
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
				Tools.PrintInfo(ex, typeof(Actions));
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
			Settings.Instance.RegisterSetting(SettingsKeys.Enable, "/apps/glippy/actions/enable", SettingTypes.Boolean, true);			
		}
		
		/// <summary>
		/// Releases all resource used by the Actions object.
		/// </summary>
		public void Dispose()
		{
			if (this.EditActionWindow != null)
			{
				this.EditActionWindow.Destroy();
				this.EditActionWindow.Dispose();
			}
			
			this.list.Dispose();			
		}
		
		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="cmd">Command line.</param>
		/// <param name="item">Item which content will be replaced in command.</param>
		private static void ExecuteCommand(string cmd, Item item)
		{
			try
			{
				string[] lines = cmd.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				
				foreach (string line in lines)
				{
					string[] words = line.Split(' ');
					
					if (words.Length < 2)
						continue;
					
					using (Process process = new Process())
					{
						process.StartInfo.FileName = words[0];
						
						StringBuilder sb = new StringBuilder();
						
						for (int i = 1; i < words.Length; i++)
						{
							sb.Append(words[i]).Append(" ");
						}
						
						sb.Replace("%s", "\"" + item.Text.Replace("\"", "\\\"") + "\"");
						
						process.StartInfo.Arguments = sb.ToString();
						process.Start();							
					}
				}																
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, typeof(Actions));
			}
		}
		
		/// <summary>
		/// Creates menu from actions list.
		/// </summary>
		private void BuildMenu()
		{			
			if (this.submenu != null)
			{
				foreach (Gtk.Widget w in this.submenu.Children)
				{
					this.submenu.Remove(w);
					w.Destroy();
					w.Dispose();
				}
				
				this.submenu.Destroy();
				this.submenu.Dispose();
			}
			
			Action action;
			Gtk.MenuItem mi;
			this.submenu = new Gtk.Menu();
			
			foreach (object[] o in this.list)
			{
				action = ((Action)o[0]);
				string content = action.Content;
				mi = new Gtk.MenuItem(action.Label);
				this.submenu.Append(mi);
				mi.Activated += (s, e) => ExecuteCommand(content, Clipboard.Instance.Items.FirstOrDefault(i => i.IsText));
				mi.ButtonReleaseEvent += (s, e) => ExecuteCommand(content, Clipboard.Instance.Items.FirstOrDefault(i => i.IsText));				
			}			
		}			
	}
}
