/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GConf;
using Glippy.Core.Extensions;

namespace Glippy.Core
{	
	/// <summary>
	/// Settings manager.
	/// </summary>
	public sealed partial class Settings
	{			
		/// <summary>
		/// Settings instance.
		/// </summary>
		private static readonly Settings instance = new Settings();

		/// <summary>
		/// GConf client.
		/// </summary>
		private Client client;			
		
		/// <summary>
		/// Settings dictionary.
		/// </summary>
		private Dictionary<string, Setting> settings;
		
		/// <summary>
		/// Setting changed event.
		/// </summary>
		public event SettingChanged SettingChanged;
				
		/// <summary>
		/// Gets instance of Options.
		/// </summary>
		public static Settings Instance
		{
			get { return instance; }
		}
				
		/// <summary>
		/// Constructs options class, adds basic options and reads them from GConfig.
		/// </summary>
		private Settings()
		{
			this.settings = new Dictionary<string, Setting>();
						
			this.settings.Add(Keys.Core.StartAtLogin, new Setting("/apps/glippy/core/start_at_login", SettingTypes.Boolean, false));
			this.settings.Add(Keys.Core.KeyboardClipboard, new Setting("/apps/glippy/core/keyboard_clipboard", SettingTypes.Boolean, true));
			this.settings.Add(Keys.Core.MouseClipboard, new Setting("/apps/glippy/core/mouse_clipboard", SettingTypes.Boolean, true));
			this.settings.Add(Keys.Core.SynchronizeClipboards, new Setting("/apps/glippy/core/synchronize_clipboards", SettingTypes.Boolean, true));
			this.settings.Add(Keys.Core.SaveHistoryOnExit, new Setting("/apps/glippy/core/save_history_on_exit", SettingTypes.Boolean, false));
			this.settings.Add(Keys.Core.PasteOnSelection, new Setting("/apps/glippy/core/paste_on_selection", SettingTypes.Boolean, false));
			this.settings.Add(Keys.Core.Images, new Setting("/apps/glippy/core/images", SettingTypes.Boolean, false));
			this.settings.Add(Keys.Core.SupportedFilesAsImages, new Setting("/apps/glippy/core/supported_files_as_images", SettingTypes.Boolean, false));
			
			this.settings.Add(Keys.UI.Size, new Setting("/apps/glippy/ui/size", SettingTypes.Integer, false));
			this.settings.Add(Keys.UI.LabelLength, new Setting("/apps/glippy/ui/label_length", SettingTypes.Integer, false));
			this.settings.Add(Keys.UI.ReverseOrder, new Setting("/apps/glippy/ui/reverse_order", SettingTypes.Boolean, true));
			this.settings.Add(Keys.UI.PasteIcon, new Setting("/apps/glippy/ui/paste_icon", SettingTypes.Boolean, false));
			this.settings.Add(Keys.UI.ShowAbout, new Setting("/apps/glippy/ui/show_about", SettingTypes.Boolean, true));
			this.settings.Add(Keys.UI.ShowEditClipboard, new Setting("/apps/glippy/ui/show_edit_clipboard", SettingTypes.Boolean, true));
			this.settings.Add(Keys.UI.ShowQuit, new Setting("/apps/glippy/ui/show_quit", SettingTypes.Boolean, true));
			
			this.settings.Add(Keys.Hotkeys.Menu, new Setting("/apps/glippy/hotkeys/menu", SettingTypes.Boolean, false));
			this.settings.Add(Keys.Hotkeys.MenuModifiers, new Setting("/apps/glippy/hotkeys/menu_modifiers", SettingTypes.Integer, false));
			this.settings.Add(Keys.Hotkeys.MenuKey, new Setting("/apps/glippy/hotkeys/menu_key", SettingTypes.Integer, false));
			this.settings.Add(Keys.Hotkeys.CopyMouse, new Setting("/apps/glippy/hotkeys/copy_mouse", SettingTypes.Boolean, false));
			this.settings.Add(Keys.Hotkeys.CopyMouseModifiers, new Setting("/apps/glippy/hotkeys/copy_mouse_modifiers", SettingTypes.Integer, false));
			this.settings.Add(Keys.Hotkeys.CopyMouseKey, new Setting("/apps/glippy/hotkeys/copy_mouse_key", SettingTypes.Integer, false));
						
			this.settings.Add(Keys.Plugins.List, new Setting("/apps/glippy/plugins/list", SettingTypes.String, false));
			
			this.client = new Client();
			this.ReadGConfValues();			
			this.client.AddNotify("/apps/glippy", this.OnGConfChanged);					
		}
		
		/// <summary>
		/// Gets or sets setting value.
		/// </summary>
		/// <param name="key">Option key.</param>
		public object this [string key]
		{
			get
			{
				try
				{
					return this.settings[key].Value;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, this.GetType());
					return null;
				}				
			}
			set
			{
				try
				{					
					Setting o = this.settings[key];					
					o.Value = value;
					this.client.Set(o.Key, value);
															
					if (this.SettingChanged != null)
						this.SettingChanged(this, new SettingChangedArgs(o.Key, o.RequiresMenuRebuild));					
				}				
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, this.GetType());
				}				
			}
		}
		
		/// <summary>
		/// Registers new option.
		/// </summary>
		/// <param name="key">Key used to access value.</param>
		/// <param name="gconfKey">Path to key in GConf.</param>
		/// <param name="valueType">Value type.</param>
		/// <param name="requiresMenuRebuild">If true, SettingChanged event propagates information whether menu rebuild is required.</param>
		/// <returns>True, if succeeded.</returns>
		public bool RegisterSetting(string key, string gconfKey, SettingTypes valueType, bool requiresMenuRebuild)
		{
			try
			{
				Setting o = new Setting(gconfKey, valueType, requiresMenuRebuild);
				this.settings.Add(key, o);
				this.ReadGConfValue(o);				
			}
			catch (Exception ex)
			{
#if DEBUG				
				Tools.PrintInfo(ex, this.GetType());
#endif
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// Emits SettingChanged event on demand.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		public void EmitSettingChanged(object sender, SettingChangedArgs args)
		{
			if (this.SettingChanged != null)
				this.SettingChanged(sender, args);
		}
		
		/// <summary>
		/// Restores default values for all registered keys.
		/// </summary>
		public void RestoreDefaults()
		{
			foreach (KeyValuePair<string, Setting> setting in this.settings)
			{
				this.client.Unset(setting.Value.Key);
			}
		}
		
		/// <summary>
		/// Updates value in option.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnGConfChanged(object sender, NotifyEventArgs args)
		{
			try
			{
				this.settings.First(o => o.Value.Key == args.Key).Value.Value = args.Value;
			}
			catch (Exception ex)
			{
#if DEBUG
				Tools.PrintInfo(ex, this.GetType());
#endif
			}
		}
		
		/// <summary>
		/// Reads configuration from GConf.
		/// </summary>
		private void ReadGConfValues()
		{
			foreach (KeyValuePair<string, Setting> option in this.settings)
			{				
				this.ReadGConfValue(option.Value);
			}
		}		
		
		/// <summary>
		/// Reads single option value from GConf.
		/// </summary>
		/// <param name="setting">Option to set.</param>
		private void ReadGConfValue(Setting setting)
		{
			try
			{
				setting.Value = this.client.Get(setting.Key);				 				
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
				this.client.Unset(setting.Key);				
			}
		}			
	}
}
