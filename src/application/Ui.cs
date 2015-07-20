/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Glippy.Core;
using Glippy.Core.Api;
using Glippy.Core.Extensions;
using Glippy.XHotkeys;
using Mono.Unix;
using Mono.Unix.Native;	
using System.Runtime.InteropServices;

namespace Glippy.Application
{	
	/// <summary>
	/// Glippy UI.
	/// </summary>
	internal class Ui
	{
		/// <summary>
		/// GTK menu, main heart of UI.
		/// </summary>
		private Gtk.Menu menu;
				
		/// <summary>
		/// Regex used to filter preview begining.
		/// </summary>
		private System.Text.RegularExpressions.Regex previewRegex;
		
		/// <summary>
		/// Temporary mouse item.
		/// </summary>
		private Item temporaryMouseItem;
		
		/// <summary>
		/// Flag indicates whether temporary mouse should be ignored.
		/// </summary>
		private bool ignoreTemporaryItem;
		
		/// <summary>
		/// Timer which restores mouse content.
		/// </summary>
		private Timer mouseContentRestorationTimer;
		
		/// <summary>
		/// Mouse content restoration timer lock.
		/// </summary>
		private object mouseContentRestorationTimerLock;
		
		/// <summary>
		/// Gets or sets plugins list.
		/// </summary>
		public List<IPlugin> Plugins { get; set; }
		
		/// <summary>
		/// Gets or sets trays list.
		/// </summary>
		public List<ITray> Trays { get; set; }
		
		/// <summary>
		/// Gets or sets preferences window.
		/// </summary>		
		public PreferencesWindow PreferencesWindow { get; set; }
		
		/// <summary>
		/// Gets or sets edit content window.
		/// </summary>
		public EditContentWindow EditContentWindow { get; set; }
		
		/// <summary>
		/// Creates new instance of Ui class.
		/// </summary>
		public Ui()
		{
			this.mouseContentRestorationTimerLock = new object();
			this.previewRegex = new System.Text.RegularExpressions.Regex(@"^\S*\s", System.Text.RegularExpressions.RegexOptions.Compiled);
						
			Hotkeys.Instance.RegisterHotkey(Settings.Keys.Hotkeys.Menu, new Hotkey((Gdk.ModifierType)Settings.Instance[Settings.Keys.Hotkeys.MenuModifiers].AsInteger(),
																					(Gdk.Key)Settings.Instance[Settings.Keys.Hotkeys.MenuKey].AsInteger(),
			                                           								Settings.Instance[Settings.Keys.Hotkeys.Menu].AsBoolean(),
			                                           								this.ShowMenu));			
			Hotkeys.Instance.RegisterHotkey(Settings.Keys.Hotkeys.CopyMouse, new Hotkey((Gdk.ModifierType)Settings.Instance[Settings.Keys.Hotkeys.CopyMouseModifiers].AsInteger(),
																					(Gdk.Key)Settings.Instance[Settings.Keys.Hotkeys.CopyMouseKey].AsInteger(),
			                                           								Settings.Instance[Settings.Keys.Hotkeys.CopyMouse].AsBoolean(),
			                                           								this.SetTemporaryMouseItemAsContent));						                                           											                                           							
			Hotkeys.Instance.Enabled = true;
						
			this.Plugins = new List<IPlugin>();
			this.Trays = new List<ITray>();			
			
			Clipboard.Instance.LockMouseClipboard = Settings.Instance[Settings.Keys.Hotkeys.CopyMouse].AsBoolean();
			Clipboard.Instance.ClipboardChanged += this.OnClipboardChanged;
			Settings.Instance.SettingChanged += this.OnSettingChanged;					
			this.RebuildMenu();			
			
			string[] plugins = Settings.Instance[Settings.Keys.Plugins.List].AsString().Split('|');
			List<string> parsed_plugins = new List<string>();
			
			foreach (string plug in plugins.OrderBy(p => p))
			{
				if (string.IsNullOrWhiteSpace(plug))
					continue;
				
				try
				{
					IBase plugin = Core.Plugins.LoadPlugin(plug);			
					Type t = plugin.GetType();
					
					if (t.GetInterface(typeof(IPlugin).Name) != null)
					{
						((IPlugin)plugin).Load();
						this.Plugins.Add((IPlugin)plugin);
						
					}
					else if (t.GetInterface(typeof(ITray).Name) != null)
					{
						((ITray)plugin).Load(this.Menu, this.RebuildMenu);
						this.Trays.Add((ITray)plugin);						
					}
					
					parsed_plugins.Add(plug);
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, this.GetType());
				}
			}		
			
			Settings.Instance[Settings.Keys.Plugins.List] = string.Join("|", parsed_plugins);			
		}
			
		/// <summary>
		/// Gets GTK menu instance.
		/// </summary>
		public Gtk.Menu Menu
		{
			get { return this.menu; }
		}
		
		/// <summary>
		/// Creates menu based on Clipboard history content.
		/// </summary>
		public Gtk.Menu RebuildMenu()
		{
			Item item;
			bool at_end = !Settings.Instance[Settings.Keys.UI.ReverseOrder].AsBoolean();
			bool keyboard = Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean();
			bool mouse = Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean();
			bool synchronize = Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean();
						
										
			if (this.menu != null)
			{
				foreach (Gtk.Widget w in this.menu.Children)
				{
					menu.Remove(w);
					
					if (w is Gtk.MenuItem)
						((Gtk.MenuItem)w).Submenu = null;
						
					w.Destroy();
					w.Dispose();
				}
			
				this.menu.Destroy();
				this.menu.Dispose();								
			}
				
			this.menu = new Gtk.Menu();
			
			if (synchronize)
			{
				item = Clipboard.Instance.KeyboardItem;
				
				if (item != null)				
					this.menu.AddMenuItem(UnicodeCharacters.Scissors + " " + item.Label, at_end, this.OnPreviewItemActivated);
				else
					this.menu.AddMenuItem(UnicodeCharacters.Scissors, at_end, sensitive: false);
				
			}
			else
			{
				if (keyboard)
				{
					item = Clipboard.Instance.KeyboardItem;
					
					if (item != null)
						this.menu.AddMenuItem(UnicodeCharacters.Keyboard + " " + item.Label, at_end, this.OnPreviewItemActivated);
					else
						this.menu.AddMenuItem(UnicodeCharacters.Keyboard, at_end, sensitive: false);
				}
				
				if (mouse)
				{					
					item = Clipboard.Instance.MouseItem;
					
					if (item != null)
						this.menu.AddMenuItem(UnicodeCharacters.Cursor + " " + item.Label, at_end, this.OnPreviewItemActivated);
					else
						this.menu.AddMenuItem(UnicodeCharacters.Cursor, at_end, sensitive: false);
				}								
			}
								
			if (Clipboard.Instance.Items.Count > 0)
			{	
				bool separator_added = false;
				
				foreach (Item i in Clipboard.Instance.Items)
				{
					if (((synchronize || keyboard) && i == Clipboard.Instance.KeyboardItem) || (mouse && i == Clipboard.Instance.MouseItem))
						continue;
				
					if (!separator_added)
					{
						separator_added = true;
						this.menu.AddWidget(new Gtk.SeparatorMenuItem(), at_end);
					}
							
					this.menu.AddMenuItem(i.Label, at_end, this.OnItemActivated);
				}
								
			}
			
			this.menu.AddWidget(new Gtk.SeparatorMenuItem());
			this.menu.AddMenuItem(Catalog.GetString("_Clear clipboard"), true, (s, e) => Clipboard.Instance.Clear(), true);
			
			if (Settings.Instance[Settings.Keys.UI.ShowEditClipboard].AsBoolean())			
				this.menu.AddMenuItem(Catalog.GetString("_Edit current content"), true,
					(s, e) =>
					{
						if (this.EditContentWindow == null)	
						{
							this.EditContentWindow = new EditContentWindow(this);
							this.EditContentWindow.ShowAll();
						}
						else
						{
							this.EditContentWindow.Present();
						}
					}, true);
			
			this.menu.AddWidget(new Gtk.SeparatorMenuItem());
			
			List<Gtk.MenuItem> plugin_menuitems = new List<Gtk.MenuItem>();
			
			foreach (IPlugin plugin in this.Plugins)
			{
				Gtk.MenuItem mi = plugin.MenuItem;
				
				if (mi != null)
					plugin_menuitems.Add(mi);
			}						

			if (plugin_menuitems.Count() > 0)
			{
				foreach (Gtk.MenuItem mi in plugin_menuitems)
				{
					this.menu.AddWidget(mi);
				}
			
				this.menu.AddWidget(new Gtk.SeparatorMenuItem());
			}
			
			this.menu.AddMenuItem(Catalog.GetString("_Preferences"), true,
				(s, e) =>
				{
					if (this.PreferencesWindow == null)
					{
						this.PreferencesWindow = new PreferencesWindow(this);
						this.PreferencesWindow.ShowAll();
					}
					else
					{
						this.PreferencesWindow.Present();
					}
				}, true);
										
			if (Settings.Instance[Settings.Keys.UI.ShowAbout].AsBoolean())			
				this.menu.AddMenuItem(Catalog.GetString("_About"), true, (s, e) => AboutWindow.Show(), true);
			
			if (Settings.Instance[Settings.Keys.UI.ShowQuit].AsBoolean())			
				this.menu.AddMenuItem(Catalog.GetString("_Quit"), true, (s, e) => Syscall.kill(Syscall.getpid(), Signum.SIGTERM), true);
						
			this.menu.ShowAll();
			this.OnMenuRebuilt(this, this.menu);
			return this.menu;
		}
		
		/// <summary>
		/// Re-creates and shows menu.
		/// </summary>
		public void ShowMenu()
		{
			this.RebuildMenu();
			this.menu.Popup();					
		}
			
		/// <summary>
		/// Sets temporary mouse item as clipboard content.
		/// </summary>
		private void SetTemporaryMouseItemAsContent()
		{
			if (this.temporaryMouseItem == null)
				return;
				
			this.ignoreTemporaryItem = true;
			Clipboard.Instance.LockMouseClipboard = false;
			Clipboard.Instance.SetAsMouseContent(this.temporaryMouseItem);
			this.temporaryMouseItem = null;
		}	
			
		/// <summary>
		/// Stores temporary mouse item.
		/// </summary>
		/// <param name="args">Event arguments.</param>
		private void StoreTemporaryMouseItem(ClipboardChangedArgs args)
		{
			lock (this.mouseContentRestorationTimerLock)
			{			
				if (args == null || args.Clipboard != Gdk.Selection.Primary)
					return;
					
				if (!Clipboard.Instance.LockMouseClipboard)
					Clipboard.Instance.LockMouseClipboard = true;				
					
				if (this.ignoreTemporaryItem)
				{
					this.ignoreTemporaryItem = false;
					return;
				}
				
				if (args.OldValue == null || args.NewValue == null || args.OldValue.Text == args.NewValue.Text)
					return;
				
				if (this.temporaryMouseItem != null	&& this.temporaryMouseItem.Text == args.OldValue.Text)
					return;
				
				if (this.mouseContentRestorationTimer != null) 
				{
					this.mouseContentRestorationTimer = new Timer((o) =>
					{
						lock (this.mouseContentRestorationTimerLock)
						{
							Gtk.Application.Invoke((s, e) => Clipboard.Instance.SetAsMouseContent((Item)o));
							this.mouseContentRestorationTimer.Dispose();
							this.mouseContentRestorationTimer = null;
						}				
					}, args.OldValue, 1000, Timeout.Infinite);				
				}							
				
				this.temporaryMouseItem = args.NewValue;
			}
		}
									
		/// <summary>
		/// Handles preview item activation event. Sets item content to clipboard.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnPreviewItemActivated(object sender, EventArgs args)
		{
			try
			{
				Item item = Clipboard.Instance.Items.First(i => i.Label == this.previewRegex.Replace(((Gtk.Label)((Gtk.MenuItem)sender).Children.First(m => m is Gtk.Label)).Text, string.Empty));
				this.ignoreTemporaryItem = true;
				Clipboard.Instance.SetAsContent(item);
				
				if (Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean() && Settings.Instance[Settings.Keys.Core.PasteOnSelection].AsBoolean())
					Hotkeys.Instance.Paste();							
			}			
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
			}
		}
		
		/// <summary>
		/// Handles history item activation event. Sets item content to clipboard.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="args">Event arguments.</param>
		private void OnItemActivated(object sender, EventArgs args)
		{ 
			try
			{
				Item item = Clipboard.Instance.Items.First(i => i.Label == ((Gtk.Label)((Gtk.MenuItem)sender).Children.First(m => m is Gtk.Label)).Text);
				this.ignoreTemporaryItem = true;
				Clipboard.Instance.SetAsContent(item);
				
				if (Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean() && Settings.Instance[Settings.Keys.Core.PasteOnSelection].AsBoolean())
					Hotkeys.Instance.Paste();							
			}
			catch (Exception ex)
			{						
				Tools.PrintInfo(ex, this.GetType());
			}
		}			
		
		/// <summary>
		/// Sends Clipboard's ClipboardChanged event to tray plugins.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnClipboardChanged(object sender, ClipboardChangedArgs args)
		{
			if (Settings.Instance[Settings.Keys.Hotkeys.CopyMouse].AsBoolean())
				this.StoreTemporaryMouseItem(args);
		
			foreach (ITray tray in this.Trays)
			{
				tray.OnClipboardChanged(sender, args);
			}
		}
		
		/// <summary>
		/// Sends Settings SettingChanged event to tray plugins.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>		
		private void OnSettingChanged(object sender, SettingChangedArgs args)
		{
			foreach (ITray tray in this.Trays)
			{
				tray.OnSettingChanged(sender, args);
			}
		}
		
		private void OnMenuRebuilt(object sender, Gtk.Menu menu)
		{
			foreach (ITray tray in this.Trays)
			{
				tray.OnMenuRebuilt(sender, menu);
			}
		}
	}
}
