/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using Glippy.Core;
using Glippy.Core.Api;
using Mono.Unix;

namespace Glippy.Upload
{
	/// <summary>
	/// Upload plugin.
	/// </summary>
	public class Upload : IPlugin
	{
		/// <summary>
		/// Gets or sets upload window.
		/// </summary>
		internal UploadWindow UploadWindow { get; set; }
		
		/// <summary>
		/// Gets plugin name.
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
				Gtk.MenuItem mi = new Gtk.MenuItem(Catalog.GetString("_Upload"));
				mi.Activated += (s, e) =>
				{
					if (this.UploadWindow == null)
					{
						this.UploadWindow = new UploadWindow(this);
						this.UploadWindow.ShowAll();
					}
					else
					{
						this.UploadWindow.Present();
					}				
				};
				mi.Sensitive = Core.Clipboard.Instance.KeyboardItem != null || Core.Clipboard.Instance.MouseItem != null;
				
				return mi;
			}
		}
		
		/// <summary>
		/// Gets container, which is attached to preferences page.
		/// </summary>
		public Gtk.Container PreferencesPage
		{
			get	{ return new UploadPreferencesPage(); }
		}
		
		/// <summary>
		/// Initializes a new instance of the Upload class.
		/// </summary>
		public Upload()
		{
			this.Name = Catalog.GetString("Upload");
			this.Description = Catalog.GetString("Upload text to Pastebin and images to Imgur.");
		}		
		
		/// <summary>
		/// Loads plugin.
		/// </summary>
		public void Load()
		{			
			Settings.Instance.RegisterSetting(SettingsKeys.PastebinUserKey, "/apps/glippy/upload/pastebin_userkey", SettingTypes.String, false);			
			System.Net.ServicePointManager.Expect100Continue = false; // GTFO
		}
		
		/// <summary>
		/// Releases all resource used by the Upload object.
		/// </summary>
		public void Dispose()
		{
			if (this.UploadWindow != null)
			{
				this.UploadWindow.Destroy();
				this.UploadWindow.Dispose();
			}			
		}
	}
}
