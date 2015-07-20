/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Glippy.Core.Extensions;

namespace Glippy.UrlShortener
{
	/// <summary>
	/// URL shortener preferences page.
	/// </summary>
	internal partial class UrlShortenerPreferencesPage : Gtk.Bin
	{
		/// <summary>
		/// If false, event aren't handled.
		/// </summary>
		private bool handleEvents; 
		
		/// <summary>
		/// Initializes a new instance of the UrlShortenerPreferencesPage class.
		/// </summary>
		public UrlShortenerPreferencesPage()
		{
			this.Build();
			this.handleEvents = false;
			this.shortener.Active = Core.Settings.Instance[SettingsKeys.UrlShortener].AsInteger();
			this.bitlyUsername.Text = Core.Settings.Instance[SettingsKeys.BitLyUsername].AsString();
			this.bitlyApiKey.Text = Core.Settings.Instance[SettingsKeys.BitLyApiKey].AsString();
			this.handleEvents = true;
		}		
		
		/// <summary>
		/// Changes selected URL shortener
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		private void OnShortenerChanged(object sender, EventArgs e)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[SettingsKeys.UrlShortener] = this.shortener.Active;
		}
		
		/// <summary>
		/// Changes bit.ly login.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		private void OnBitlyUsernameChanged(object sender, EventArgs e)
		{
			if (!this.handleEvents)
				return;

			Core.Settings.Instance[SettingsKeys.BitLyUsername] = this.bitlyUsername.Text;			
		}
		
		/// <summary>
		/// Changes bit.ly API key.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		private void OnBitlyApiKeyChanged(object sender, EventArgs e)
		{
			if (!this.handleEvents)
				return;

			Core.Settings.Instance[SettingsKeys.BitLyApiKey] = this.bitlyApiKey.Text;
		}
	}
}
