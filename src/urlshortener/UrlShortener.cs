/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System.Linq;
using System.Threading;
using Glippy.Core;
using Glippy.Core.Api;
using Glippy.Core.Extensions;
using Gtk;
using Mono.Unix;

namespace Glippy.UrlShortener
{
	/// <summary>
	/// URL shortener plugin.
	/// </summary>
	public class UrlShortener : IPlugin
	{
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
				Gtk.MenuItem mi = new Gtk.MenuItem(Catalog.GetString("S_horten URL"));
				mi.Activated += (s, e) =>
				{
					ThreadPool.QueueUserWorkItem((state) => 
                    {
						Core.Item i = Core.Clipboard.Instance.Items.FirstOrDefault();												
						
						if (i == null)
							return;
						
						string result_text = null;
						
						switch ((ShortenerService)Core.Settings.Instance[SettingsKeys.UrlShortener].AsInteger())
						{
							case ShortenerService.BitLy:
								result_text = Shorteners.BitLy.Shorten(i.Text, Core.Settings.Instance[SettingsKeys.BitLyUsername].AsString(), Core.Settings.Instance[SettingsKeys.BitLyApiKey].AsString());						
								break;
								
							case ShortenerService.TinyURL:
								result_text = Shorteners.TinyURL.Shorten(i.Text);
								break;
							
							default:
								return;
						}
						
						MessageType type = MessageType.Info;
						
						if (result_text.StartsWith(Catalog.GetString("Error: ")))
							type = MessageType.Error;							
						else
							result_text = Catalog.GetString("Shortened URL:") + "\n" + result_text;
						
						Application.Invoke((gs, ge) =>					
                      	{
							MessageDialog dialog = new MessageDialog(null, DialogFlags.Modal, type, ButtonsType.Ok, false, result_text);
							dialog.WindowPosition = WindowPosition.CenterAlways;							
							dialog.SkipTaskbarHint = false;
							dialog.KeepAbove = true;
							dialog.UseMarkup = true;
							dialog.Run();					
							dialog.Destroy();
							dialog.Dispose();								
						});						
					});
					
				};
								
				Core.Item item = Core.Clipboard.Instance.Items.FirstOrDefault();
				mi.Sensitive = item != null ? (item.IsText && (item.Text.StartsWith("http://") || item.Text.StartsWith("https://") || item.Text.StartsWith("www."))) : false;
				
				if ((ShortenerService)Core.Settings.Instance[SettingsKeys.UrlShortener].AsInteger() == ShortenerService.BitLy)
				{
					mi.Sensitive = mi.Sensitive
						&& !string.IsNullOrWhiteSpace(Core.Settings.Instance[SettingsKeys.BitLyUsername].AsString())
						&& !string.IsNullOrWhiteSpace(Core.Settings.Instance[SettingsKeys.BitLyApiKey].AsString());
				}
				
				return mi;
			}
		}
		
		/// <summary>
		/// Gets container, which is attached to preferences page.
		/// </summary>
		public Gtk.Container PreferencesPage
		{
			get	{ return new UrlShortenerPreferencesPage(); }
		}
		
		/// <summary>
		/// Initializes a new instance of the UrlShortener class.
		/// </summary>
		public UrlShortener()
		{
			this.Name = Catalog.GetString("URL shortener");
			this.Description = Catalog.GetString("Shorten URLs with bit.ly or TinyURL.");
		}		
		
		/// <summary>
		/// Loads plugin.
		/// </summary>
		public void Load()
		{			
			Core.Settings.Instance.RegisterSetting(SettingsKeys.UrlShortener, "/apps/glippy/urlshortener/shortener", SettingTypes.Integer, true);
			Core.Settings.Instance.RegisterSetting(SettingsKeys.BitLyUsername, "/apps/glippy/urlshortener/bitly_username", SettingTypes.String, true);
			Core.Settings.Instance.RegisterSetting(SettingsKeys.BitLyApiKey, "/apps/glippy/urlshortener/bitly_apikey", SettingTypes.String, true);
			System.Net.ServicePointManager.Expect100Continue = false; // GTFO
		}
		
		/// <summary>
		/// Releases all resource used by the URL shortener object.
		/// </summary>
		public void Dispose()
		{						
		}
	}
}

