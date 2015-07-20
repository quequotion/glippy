/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Threading;
using Glippy.Core.Extensions;
using Gtk;
using Mono.Unix;

namespace Glippy.Upload
{	
	/// <summary>
	/// Upload preferences page.
	/// </summary>
	internal partial class UploadPreferencesPage : Bin
	{
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		public UploadPreferencesPage()
		{
			this.Build();
			this.SetPastebinSensitivity(!string.IsNullOrWhiteSpace(Core.Settings.Instance[SettingsKeys.PastebinUserKey].AsString()));
		}
		
		/// <summary>
		/// Gets pastebin user key.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonPastebinLoginClicked(object sender, EventArgs e)
		{
			MessageDialog dialog;
			
			if (this.pastebinUsername.Text.Length == 0 || this.labelPastebinPassword.Text.Length == 0)
			{
				dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false, Catalog.GetString("Username and password are required."));
				dialog.Run();
				dialog.Destroy();
				dialog.Dispose();
			}	
			else
			{
				this.buttonPastebinLogin.Sensitive = false;
				ThreadPool.QueueUserWorkItem((state) =>
                {
					string key = Uploaders.Pastebin.GetUserKey(this.pastebinUsername.Text, this.pastebinPassword.Text);
					
					Application.Invoke((s, ev) =>
                    {
						if (key.StartsWith("Bad") || key.StartsWith(Catalog.GetString("Error: ")))
						{						
							this.SetPastebinSensitivity(false);
							dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false, Catalog.GetString("Invalid username or password."));
							dialog.Run();
							dialog.Destroy();
							dialog.Dispose();							
						}
						else
						{
							Core.Settings.Instance[SettingsKeys.PastebinUserKey] = key;
							dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, false, Catalog.GetString("Login successful. From now you can upload texts to your Pastebin account."));
							dialog.Run();
							dialog.Destroy();
							dialog.Dispose();
							this.SetPastebinSensitivity(true);
						}									
					});					
				});
			}
		}
		
		/// <summary>
		/// Clears stored pastebin user key.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonPastebinClearClicked(object sender, EventArgs e)
		{
			Core.Settings.Instance[SettingsKeys.PastebinUserKey] = string.Empty;
			this.SetPastebinSensitivity(false);
		}
		
		/// <summary>
		/// Sets pastebin widgets sensitivity.
		/// </summary>
		/// <param name="userkeyExists">Userkey exists.</param>
		private void SetPastebinSensitivity(bool userkeyExists)
		{
			this.pastebinUsername.Sensitive = this.pastebinPassword.Sensitive = this.buttonPastebinLogin.Sensitive = !userkeyExists;
			this.buttonClear.Sensitive = userkeyExists;			
		}
	}
}

