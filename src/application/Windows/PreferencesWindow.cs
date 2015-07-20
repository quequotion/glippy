/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Linq;
using Glippy.Core;
using Glippy.Core.Api;
using Glippy.Core.Extensions;
using Glippy.XHotkeys;
using Gtk;
using Mono.Unix;

namespace Glippy.Application
{
	/// <summary>
	/// Preferences window.
	/// </summary>
	internal partial class PreferencesWindow : Window
	{		
		/// <summary>
		/// The content of the desktop file.
		/// </summary>
		private const string desktopFileContent = "[Desktop Entry]\nEncoding=UTF-8\nName=Glippy\nExec=glippy -s\nIcon=glippy\nType=Application\nCategories=Utility";
		
		/// <summary>
		/// The desktop file path.
		/// </summary>
		private const string desktopFileName = "glippy.desktop";
		
		/// <summary>
		/// If true, event are handled.
		/// </summary>
		private bool handleEvents;
	
		/// <summary>
		/// Original pages count.
		/// </summary>
		private int originalPages;
		
		/// <summary>
		/// User interface.
		/// </summary>
		private Ui ui;
				
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="ui">User interface.</param>
		public PreferencesWindow(Ui ui) : base(WindowType.Toplevel)
		{
			this.Build();
			this.originalPages = this.notebook.NPages;
			this.ui = ui;
			
			try
			{
				this.Icon = IconTheme.Default.LoadIcon("glippy", 128, IconLookupFlags.GenericFallback);
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
			}
			
			this.handleEvents = false;
			
			this.startAtLogin.Active = Core.Settings.Instance[Core.Settings.Keys.Core.StartAtLogin].AsBoolean();
			this.keyboardClipboard.Active = Core.Settings.Instance[Core.Settings.Keys.Core.KeyboardClipboard].AsBoolean();
			this.images.Active = Core.Settings.Instance[Core.Settings.Keys.Core.Images].AsBoolean();
			this.supportedFilesAsImages.Active = Core.Settings.Instance[Core.Settings.Keys.Core.SupportedFilesAsImages].AsBoolean();
			this.mouseClipboard.Active = Core.Settings.Instance[Core.Settings.Keys.Core.MouseClipboard].AsBoolean();
			this.synchronizeClipboards.Active = Core.Settings.Instance[Core.Settings.Keys.Core.SynchronizeClipboards].AsBoolean();
			this.saveHistoryOnExit.Active = Core.Settings.Instance[Core.Settings.Keys.Core.SaveHistoryOnExit].AsBoolean();
			this.pasteItemContentOnSelection.Active = Core.Settings.Instance[Core.Settings.Keys.Core.PasteOnSelection].AsBoolean();
			
			this.size.Value = Core.Settings.Instance[Core.Settings.Keys.UI.Size].AsInteger();
			this.labelLength.Value = Core.Settings.Instance[Core.Settings.Keys.UI.LabelLength].AsInteger();
			this.reverseOrder.Active = Core.Settings.Instance[Core.Settings.Keys.UI.ReverseOrder].AsBoolean();
			this.pasteIcon.Active = Core.Settings.Instance[Core.Settings.Keys.UI.PasteIcon].AsBoolean();
			this.showAbout.Active = Core.Settings.Instance[Core.Settings.Keys.UI.ShowAbout].AsBoolean();
			this.showEditClipboard.Active = Core.Settings.Instance[Core.Settings.Keys.UI.ShowEditClipboard].AsBoolean();
			this.showQuit.Active = Core.Settings.Instance[Core.Settings.Keys.UI.ShowQuit].AsBoolean();			
			
			this.menuHotkey.Active = Core.Settings.Instance[Core.Settings.Keys.Hotkeys.Menu].AsBoolean();
			this.menuHotkeyText.Sensitive = this.buttonGrabMenuHotkey.Sensitive = this.menuHotkey.Active;
			this.menuHotkeyText.Text = Hotkeys.Instance[Core.Settings.Keys.Hotkeys.Menu].ToString();
			this.copyMouseHotkey.Active = this.mouseClipboard.Active && Core.Settings.Instance[Core.Settings.Keys.Hotkeys.CopyMouse].AsBoolean();
			this.copyMouseHotkeyText.Sensitive = this.buttonGrabCopyMouseHotkey.Sensitive = this.copyMouseHotkey.Active;
			this.copyMouseHotkeyText.Text = Hotkeys.Instance[Core.Settings.Keys.Hotkeys.CopyMouse].ToString();
			
			this.synchronizeClipboards.Sensitive = this.keyboardClipboard.Active && this.mouseClipboard.Active && !this.copyMouseHotkey.Active;
			this.images.Sensitive = this.keyboardClipboard.Active;
			this.supportedFilesAsImages.Sensitive = this.images.Sensitive && this.images.Active;
			this.handleEvents = true;
			
			this.LoadPluginsTreeview();
			
			foreach (IPlugin plug in this.ui.Plugins)
			{
				if (plug.PreferencesPage != null)
					this.notebook.AppendPage(plug.PreferencesPage, new Label(plug.Name));				
			}
			
			this.Destroyed += (s, e) => this.Purge();
		}		
		
		/// <summary>
		/// Cleans window.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>		
		private void Purge()
		{
			this.ui.PreferencesWindow = null;
			this.Dispose();
		}
		
		/// <summary>
		/// Handles button close clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>		
		private void OnButtonCloseClicked(object sender, EventArgs args)
		{
			this.Destroy();			
		}
		
		/// <summary>
		/// Closes window on Escape press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnKeyPressEvent(object sender, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape)
				this.Destroy();
		}
		
		#region Base settings handlers

		/// <summary>
		/// Handles start at login toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnStartAtLoginToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.StartAtLogin] = this.startAtLogin.Active;
			
			try
			{
				string home_dir = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				
				if (!System.IO.Directory.Exists(home_dir + "/.config"))
					System.IO.Directory.CreateDirectory(home_dir + "/.config");
				
				if (!System.IO.Directory.Exists(home_dir + "/.config/autostart"))
					System.IO.Directory.CreateDirectory(home_dir + "/.config/autostart");
				
				bool file_exists = System.IO.File.Exists(home_dir + "/.config/autostart/" + desktopFileName);
				
				if (this.startAtLogin.Active && !file_exists)
				{				
					using (System.IO.StreamWriter writer = new System.IO.StreamWriter(home_dir + "/.config/autostart/" + desktopFileName))
					{
						writer.Write(desktopFileContent);																
					}									
				}								
				else if (file_exists)
				{
					System.IO.File.Delete(home_dir + "/.config/autostart/" + desktopFileName);					
				}				
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
			}								
		}
		
		/// <summary>
		/// Handles keyboard clipboard toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnKeyboardClipboardToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.KeyboardClipboard] = this.keyboardClipboard.Active;			
			this.synchronizeClipboards.Sensitive = this.keyboardClipboard.Active && this.mouseClipboard.Active && !this.copyMouseHotkey.Active;
			this.images.Sensitive = this.keyboardClipboard.Active;
			this.supportedFilesAsImages.Sensitive = this.images.Sensitive && this.images.Active;
			
			if (this.synchronizeClipboards.Active)
				this.synchronizeClipboards.Active = false;
		}
		
		/// <summary>
		/// Handles images toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnImagesToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.Images] = this.images.Active;
			this.supportedFilesAsImages.Sensitive = this.images.Active;
		}
		
		/// <summary>
		/// Handles supported files as images toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnSupportedFilesAsImagesToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.SupportedFilesAsImages] = this.supportedFilesAsImages.Active;
		}		
		
		/// <summary>
		/// Handles mouse clipboard toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnMouseClipboardToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.MouseClipboard] = this.mouseClipboard.Active;			
			
			if (!this.mouseClipboard.Active)
			{
				if (this.copyMouseHotkey.Active)
					this.copyMouseHotkey.Active = false;
				
				if (this.synchronizeClipboards.Active)
					this.synchronizeClipboards.Active = false;
			}
			
			this.copyMouseHotkey.Sensitive = this.mouseClipboard.Active;
			this.synchronizeClipboards.Sensitive = this.keyboardClipboard.Active && this.mouseClipboard.Active;
		}
		
		/// <summary>
		/// Handles synchronize clipboards toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnSynchronizeClipboardsToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
							
			Core.Settings.Instance[Core.Settings.Keys.Core.SynchronizeClipboards] = this.synchronizeClipboards.Active;
			this.copyMouseHotkey.Sensitive = !this.synchronizeClipboards.Active;			
		}
		
		/// <summary>
		/// Handles save history on exit toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnSaveHistoryOnExitToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.SaveHistoryOnExit] = this.saveHistoryOnExit.Active;
		}
		
		/// <summary>
		/// Handles paste item content on selection toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnPasteitemContentOnSelectionToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.Core.PasteOnSelection] = this.pasteItemContentOnSelection.Active;			
		}
		
		#endregion Base settings handlers
		
		#region Interface handlers	
		
		/// <summary>
		/// Handles size value changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnSizeValueChanged(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.Size] = (int)this.size.Value;			
		}
		
		/// <summary>
		/// Handles label length value changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnLabelLengthValueChanged(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.LabelLength] = (int)this.labelLength.Value;			
		}
		
		/// <summary>
		/// Handles reverse order toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnReverseOrderToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.ReverseOrder] = this.reverseOrder.Active;
		}					
		
		/// <summary>
		/// Handles paste icon toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnPasteIconToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.PasteIcon] = this.pasteIcon.Active;
		}
		
		/// <summary>
		/// Handles show about toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnShowAboutToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.ShowAbout] = this.showAbout.Active;			
		}
		
		/// <summary>
		/// Handles show edit clipboard toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnShowEditClipboardToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.ShowEditClipboard] = this.showEditClipboard.Active;			
		}
		
		/// <summary>
		/// Handles show quit toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnShowQuitToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[Core.Settings.Keys.UI.ShowQuit] = this.showQuit.Active;			
		}
		
		#endregion Interface handlers				
		
		#region Hotkeys handlers
		
		/// <summary>
		/// Handles menu hotkey toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnMenuHotkeyToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			this.menuHotkeyText.Sensitive = this.menuHotkey.Active;
			this.buttonGrabMenuHotkey.Sensitive = this.menuHotkey.Active;
			Core.Settings.Instance[Core.Settings.Keys.Hotkeys.Menu] = this.menuHotkey.Active;
			Hotkeys.Instance[Core.Settings.Keys.Hotkeys.Menu].Enabled = this.menuHotkey.Active;
			Hotkeys.Instance.Refresh();
		}			
		
		/// <summary>
		/// Handles button grab menu hotkey clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonGrabMenuHotkeyClicked(object sender, EventArgs args)
		{
			this.buttonGrabCopyMouseHotkey.Sensitive = false;
			this.buttonGrabMenuHotkey.Sensitive = false;
			
			Hotkeys.Instance.GrabKeyboard((s, e) =>
			{
				this.buttonGrabCopyMouseHotkey.Sensitive = true;
				this.buttonGrabMenuHotkey.Sensitive = true;
				
				if (e == null || Hotkeys.Instance.IsKeyCombinationInUse(e.Modifiers, e.Key))
					return;			
				
				Hotkey hotkey = new Hotkey(e.Modifiers, e.Key, true, Hotkeys.Instance[Core.Settings.Keys.Hotkeys.Menu].OnHotkeyPressed);				
				Hotkeys.Instance.UnregisterHotkey(Core.Settings.Keys.Hotkeys.Menu);
				Hotkeys.Instance.RegisterHotkey(Core.Settings.Keys.Hotkeys.Menu, hotkey);
				this.menuHotkeyText.Text = hotkey.ToString();
				
				Core.Settings.Instance[Core.Settings.Keys.Hotkeys.MenuModifiers] = (int)e.Modifiers;
				Core.Settings.Instance[Core.Settings.Keys.Hotkeys.MenuKey] = (int)e.Key;
			});
		}
		
		/// <summary>
		/// Handles copy for mouse hotkey toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnCopyMouseHotkeyToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Clipboard.Instance.LockMouseClipboard = this.copyMouseHotkey.Active;
			this.synchronizeClipboards.Sensitive = this.keyboardClipboard.Active && this.mouseClipboard.Active && !this.copyMouseHotkey.Active;
			this.copyMouseHotkeyText.Sensitive = this.copyMouseHotkey.Active;
			this.buttonGrabCopyMouseHotkey.Sensitive = this.copyMouseHotkey.Active;
			Core.Settings.Instance[Core.Settings.Keys.Hotkeys.CopyMouse] = this.copyMouseHotkey.Active;
			Hotkeys.Instance[Core.Settings.Keys.Hotkeys.CopyMouse].Enabled = this.copyMouseHotkey.Active;
			Hotkeys.Instance.Refresh();
		}
		
		/// <summary>
		/// Handles button grab mouse hotkey clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonGrabCopyMouseHotkeyClicked(object sender, EventArgs args)
		{
			this.buttonGrabCopyMouseHotkey.Sensitive = false;
			this.buttonGrabMenuHotkey.Sensitive = false;
			
			Hotkeys.Instance.GrabKeyboard((s, e) =>
			{
				this.buttonGrabCopyMouseHotkey.Sensitive = true;
				this.buttonGrabMenuHotkey.Sensitive = true;
				
				if (e == null || Hotkeys.Instance.IsKeyCombinationInUse(e.Modifiers, e.Key))
					return;			
				
				Hotkey hotkey = new Hotkey(e.Modifiers, e.Key, true, Hotkeys.Instance[Core.Settings.Keys.Hotkeys.CopyMouse].OnHotkeyPressed);				
				Hotkeys.Instance.UnregisterHotkey(Core.Settings.Keys.Hotkeys.CopyMouse);
				Hotkeys.Instance.RegisterHotkey(Core.Settings.Keys.Hotkeys.CopyMouse, hotkey);
				this.copyMouseHotkeyText.Text = hotkey.ToString();
				
				Core.Settings.Instance[Core.Settings.Keys.Hotkeys.CopyMouseModifiers] = (int)e.Modifiers;
				Core.Settings.Instance[Core.Settings.Keys.Hotkeys.CopyMouseKey] = (int)e.Key;
			});
		}		
		
		#endregion Hotkeys handlers
		
		#region Plugins handlers
		
		/// <summary>
		/// Plugin treeview container.
		/// </summary>
		private class PluginTreeviewContainer
		{
			public bool Enabled { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }			
			public string DllName { get; set; }
			public IBase Plugin { get; set; }			
		}
		
		/// <summary>
		/// Reloads plugin notebook preferences pages.
		/// </summary>
		private void ReloadPluginPreferencesPages()
		{
			while (this.notebook.NPages > this.originalPages)
			{
				this.notebook.RemovePage(-1);				
			}			
			
			foreach (IPlugin plugin in this.ui.Plugins)
			{
				this.notebook.AppendPage(plugin.PreferencesPage, new Label(plugin.Name));
			}
			
			this.notebook.ShowAll();
		}
		
		/// <summary>
		/// Loads the plugins treeview.
		/// </summary>
		private void LoadPluginsTreeview()
		{
			ListStore store = new ListStore(typeof(PluginTreeviewContainer));
			
			foreach (IBase plug in Plugins.LoadAllPlugins())
			{
				bool loaded = true;
				IBase loaded_plug = this.ui.Plugins.FirstOrDefault(p => p.GetType().Assembly.GetName().Name == plug.GetType().Assembly.GetName().Name);
				
				if (loaded_plug == null)
					loaded_plug = this.ui.Trays.FirstOrDefault(p => p.GetType().Assembly.GetName().Name == plug.GetType().Assembly.GetName().Name);
				
				if (loaded_plug == null)
				{
					loaded_plug = plug;
					loaded = false;
				}
				
				store.AppendValues(new PluginTreeviewContainer()
	        	{
					Enabled = loaded,
					Name = plug.Name,
					Description = plug.Description,
					DllName = plug.GetType().Assembly.GetName().Name,
					Plugin = loaded_plug
				});
			}
						
			TreeViewColumn column = new TreeViewColumn();
			column.Title = Catalog.GetString("Enabled");
			CellRendererToggle toggle_cell = new CellRendererToggle();		
			
			toggle_cell.Toggled += (s, e) =>
			{
				TreeIter iter;
				store.GetIter(out iter, new TreePath(e.Path));		 
				PluginTreeviewContainer item = (PluginTreeviewContainer)store.GetValue(iter, 0);
				item.Enabled = !((CellRendererToggle)s).Active;
				Type t = item.Plugin.GetType();
				string[] plugins = Core.Settings.Instance[Core.Settings.Keys.Plugins.List].AsString().Split('|');
				string name = t.Assembly.GetName().Name;
				
				((CellRendererToggle)s).Active = !((CellRendererToggle)s).Active;
				
				if (((CellRendererToggle)s).Active)
				{
					try
					{
						if (t.GetInterface(typeof(IPlugin).Name) != null)
						{
							IPlugin plugin = (IPlugin)item.Plugin;
							
							plugin.Load();
							this.ui.Plugins.Add(plugin);
							this.ui.Plugins = this.ui.Plugins.OrderBy(p => p.Name).ToList();													
							this.ui.RebuildMenu();							
							this.ReloadPluginPreferencesPages();
						}
						else if (t.GetInterface(typeof(ITray).Name) != null)
						{
							ITray plugin = (ITray)item.Plugin;								
							plugin.Load(this.ui.Menu, this.ui.RebuildMenu);
							this.ui.Trays.Add((ITray)item.Plugin);
						}											
						
						if (!plugins.Contains(name))
							Core.Settings.Instance[Core.Settings.Keys.Plugins.List] = plugins[0] != string.Empty ? string.Join("|", plugins) + "|" + name : name;
					}
					catch (Exception ex)
					{
						Tools.PrintInfo(ex, this.GetType());
					
						if (plugins.Contains(name))
							Core.Settings.Instance[Core.Settings.Keys.Plugins.List] = string.Join("|", plugins.Where(p => p != name).ToArray());					
					}					
				}
				else
				{
					try
					{
						if (t.GetInterface(typeof(IPlugin).Name) != null)
						{
							IPlugin plugin = (IPlugin)item.Plugin;
							this.ui.Plugins.Remove(plugin);
							plugin.Dispose();
							this.ui.RebuildMenu();							
							this.ReloadPluginPreferencesPages();
						}
						else if (t.GetInterface(typeof(ITray).Name) != null)
						{
							ITray plugin = (ITray)item.Plugin;
							this.ui.Trays.Remove(plugin);
							plugin.Unload();
						}
					}
					catch (Exception ex)
					{
						Tools.PrintInfo(ex, this.GetType());	
					}
					finally
					{
						if (plugins.Contains(name))
							Core.Settings.Instance[Core.Settings.Keys.Plugins.List] = string.Join("|", plugins.Where(p => p != name).ToArray());					
					}
				}
				
				store.EmitRowChanged(new TreePath(e.Path), iter);
			};
			column.PackStart(toggle_cell, true);			
			column.SetCellDataFunc(toggle_cell, delegate (TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
			{
				((CellRendererToggle)cell).Active = ((PluginTreeviewContainer)model.GetValue(iter, 0)).Enabled;
			});
			
			this.treeviewPlugins.AppendColumn(column);
			
			column = new TreeViewColumn();
			column.Title = Catalog.GetString("Name");
			CellRendererText text_cell = new CellRendererText();
			column.PackStart(text_cell, true);
			column.SetCellDataFunc(text_cell, delegate (TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
            {
				((CellRendererText)cell).Text = ((PluginTreeviewContainer)model.GetValue(iter, 0)).Name;
			});
			this.treeviewPlugins.AppendColumn(column);
			
			column = new TreeViewColumn();
			column.Title = Catalog.GetString("Description");
			text_cell = new CellRendererText();
			column.PackStart(text_cell, true);
			column.SetCellDataFunc(text_cell, delegate (TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
            {
				((CellRendererText)cell).Text = ((PluginTreeviewContainer)model.GetValue(iter, 0)).Description;
			});					
			this.treeviewPlugins.AppendColumn(column);
			
			this.treeviewPlugins.Model = store;
			this.treeviewPlugins.ShowAll();			
		}						
		#endregion Plugins			
	}
}
